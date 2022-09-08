using Api.Config.Core;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace Api.Config.Setting
{
    public class SettingContext
    {
        private static SettingManager _manager;

        internal SettingContext()
        {
            _manager = new SettingManager();
        }

        internal SettingManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new SettingManager();
                }
                return _manager;
            }
        }

        public SettingType SettingType { get; set; }

        public void Load(string config)
        {
            switch (SettingType)
            {
                case SettingType.Json:
                    _manager.FromJson(config);
                    break;
                case SettingType.Xml:
                    _manager.FromXml(config);
                    break;
                default:
                    break;
            }           
        }        

        public T GetValue<T>(string key)
        {
            var type = typeof(T);
            if (_manager.Get(key) == null)
            {
                return default(T);
            }
            if (SettingType == SettingType.Json)
            {                
                return ((JToken)_manager.Get(key)).ToObject<T>();
            }
            if (SettingType == SettingType.Xml)
            {
                return ((XmlNode)_manager.Get(key)).ToObject<T>();
            }
            return default(T);
        }
    }
}
