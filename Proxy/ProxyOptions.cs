using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Api.Config.Proxy
{
    public class ProxyOptions
    {
        private ProxySetting _setting;
        private readonly ConcurrentDictionary<string, long> _rates = new ConcurrentDictionary<string, long>();
        public ProxyOptions()
        {
        }

        public ProxyOptions(ProxySetting setting)
        {
            _setting = setting;
        }

        public void Set(ProxySetting setting)
        {
            _setting = setting;
        }
       
        public ProxyOptions Host(List<ProxyHost> hosts)
        {
            _setting.hosts = hosts;
            return this;
        }

        public ProxyOptions Routes(List<string> routes)
        {
            _setting.routes = routes;
            return this;
        }

        public ProxyOptions Except(List<string> excepts)
        {
            _setting.excepts = excepts;
            return this;
        }

        public string GetHost(string path)
        {
            var except = _setting.excepts.FirstOrDefault(t => t == path);
            if (except != null)
            {
                return null;
            }
            var route = _setting.routes.FirstOrDefault(t => t == path);
            var hosts = _setting.hosts.Where(t => t.routes.Contains(path)).ToList();
            if (hosts.Count() == 0 && route != null)
            {
                hosts = _setting.hosts;
            }
            if (hosts.Count() == 0)
            {
                return null;
            }
            var step = new Random().Next(hosts.Count);
            var host = hosts[step].host;
            //速率限制

            return host;
        }

        public void AddRate(string host)
        {
            _rates[host]++;
        }
    }
}