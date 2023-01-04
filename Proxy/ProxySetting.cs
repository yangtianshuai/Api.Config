using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Config.Proxy
{
    public class ProxySetting
    {
        public ProxySetting()
        {
            hosts = new List<ProxyHost>();
            routes = new List<string>();
            excepts = new List<string>();
        }
        public List<ProxyHost> hosts { get; internal set; }
        public List<string> routes { get; internal set; }
        public List<string> excepts { get; internal set; }
    }
}
