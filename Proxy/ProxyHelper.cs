using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Api.Config.Proxy
{
    public static partial class HttpExtention
	{
		private static string proxy_session = "Proxy-Session";

		public async static Task ProxyAsync(this HttpContext context, string host, Session session = null)
		{
			var url = context.Request.Path.ToUriComponent();

			var query = context.Request.QueryString.Value;

			var uri = new Uri(host + url + query);
			var request = CopyRequest(context, uri);
			if (session != null)
			{
				request.Headers.Add(proxy_session, JsonConvert.SerializeObject(session));
			}
			request.Headers.SetOpenSign(context.GetOpenSign());

			var handler = new HttpClientHandler();
			if (uri.AbsoluteUri.StartsWith("https://"))
			{
				handler.ClientCertificateOptions = ClientCertificateOption.Manual;
				handler.ServerCertificateCustomValidationCallback = (se, cert, chain, sslerror) => { return true; };
			}

			var remote = await new HttpClient(handler).SendAsync(request);
			//remote.EnsureSuccessStatusCode();

			var rsp = context.Response;
			foreach (var header in remote.Headers)
			{
				if (header.Key == "Transfer-Encoding")
				{
					continue;
				}
				if (!rsp.Headers.ContainsKey(header.Key))
				{
					rsp.Headers.Add(header.Key, header.Value.ToArray());
				}
			}
			rsp.ContentType = remote.Content.Headers.ContentType?.ToString();
			rsp.ContentLength = remote.Content.Headers.ContentLength;
			if (rsp.ContentLength > 0)
			{
				var body = await remote.Content.ReadAsStringAsync();
				var content = await remote.Content.ReadAsStreamAsync();
				using (content)
				{
					await content.CopyToAsync(rsp.Body);
				}
			}

		}

		private static HttpRequestMessage CopyRequest(HttpContext context, Uri targetUri)
		{
			var req = context.Request;
			var requestMessage = new HttpRequestMessage()
			{
				Method = new HttpMethod(req.Method),				
				RequestUri = targetUri,
			};			
			if (req.ContentLength > 0)
            {
                requestMessage.Content = new StreamContent(req.Body);
			}
            foreach (var header in req.Headers)
			{
				requestMessage.Content?.Headers.TryAddWithoutValidation(header.Key, header.Value.ToArray());
			}
			//requestMessage.Headers.Host = targetUri.Host;
			return requestMessage;
		}

		/// <summary>
		/// 获取代理Session
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public static T ProxySession<T>(this HttpContext context)
        {			
            if (context.Request.Headers.ContainsKey(proxy_session))
            {
				var session = context.Request.Headers[proxy_session].ToString();
				return JsonConvert.DeserializeObject<T>(session);
            }
			return default(T);
		}

		public static string GetRoute(this HttpContext context)
		{
			string route = context.Request.Path.Value;
			var vitual_path = context.GetVirtualPath();
			if (!string.IsNullOrEmpty(vitual_path))
			{
				route = route.Replace(vitual_path, "");
			}
			return route;
		}
	}
}
