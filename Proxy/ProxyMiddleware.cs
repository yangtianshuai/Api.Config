using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Storage.Client;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Api.Config.Proxy
{
	public class ProxyMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ProxyOptions _proxyOptions;
		private ISessionService _session;

		public ProxyMiddleware(RequestDelegate next
			, ProxyOptions proxyOptions
			, ISessionService session)
		{
			_next = next;
			_proxyOptions = proxyOptions;
			_session = session;
		}

		public async Task Invoke(HttpContext context)
		{
			var url = context.Request.Path.ToUriComponent();
			var host = _proxyOptions.GetHost(url);
			if (string.IsNullOrEmpty(host) || host == "*")
			{
				await _next.Invoke(context);
				return;
			}
			var token = context.GetToken();
			Session session = null;
			if (!string.IsNullOrEmpty(token))
            {
				session = await _session.GetAsync<Session>(token);
			}			
			await context.ProxyAsync(host, session);			
		}
	}
}
