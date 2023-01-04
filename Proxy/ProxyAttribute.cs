using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Config.Proxy
{
    public class ProxyAttribute
    {
        private List<string> _hosts;
        public ProxyAttribute(params string[] hosts)
        {
            _hosts = new List<string>();
            _hosts.AddRange(hosts);
        }

        public List<string> Get()
        {
            return _hosts;
        }
    }
}
