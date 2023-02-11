using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Config.Net
{
    public class HttpConfig
    {
        public int TimeOut { get; set; } = 10;

        public bool SuccessOnly { get; set; } = true;
        public Dictionary<string, List<string>> Proxys { get; set; } = new Dictionary<string, List<string>>();

    }
}
