using System.Net.Http;

namespace Api.Config.Net
{
    public class HttpClientFactory
    {
        private int _httpClientCount;

        private readonly HttpClient[] _httpClientArray;

        private int _index = -1;

        private readonly object _lock = new object();

        private static readonly HttpClientFactory _instance = new HttpClientFactory();

        public static HttpClientFactory Instance
        {
            get
            {
                return _instance;
            }
        }

        public HttpClientFactory(int httpClientCount = 50)
        {
            _httpClientCount = httpClientCount;
            _httpClientArray = new HttpClient[_httpClientCount];
            for (int i = 0; i < _httpClientCount; i++)
            {
                _httpClientArray[i] = new HttpClient();
            }
        }

        public HttpClient Get()
        {
            lock (_lock)
            {
                _index++;
                if (_index == _httpClientCount)
                {
                    _index = 0;
                }
                return _httpClientArray[_index];
            }
        }
    }
}
