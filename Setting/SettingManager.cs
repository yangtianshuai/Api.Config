using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Xml;

namespace Api.Config.Setting
{
    internal class SettingManager
    {        
        private readonly ConcurrentDictionary<string, object> _values = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 原始值
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 清空网关数据
        /// </summary>
        public void Clear()
        {
            _values.Clear();
        }

        /// <summary>
        /// 是否存在指定参数名
        /// </summary>
        /// <param name="key">参数名</param>
        /// <returns></returns>
        public bool Exists(string key) => _values.ContainsKey(key);

        public object Get(string key)
        {
            if (Exists(key))
            {
                return _values[key];
            }
            return null;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns></returns>
        private bool Add(string key, object value)
        {
            Text = null;
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key", "参数名不能为空");
            }

            if (value is null || string.IsNullOrEmpty(value.ToString()))
            {
                return false;
            }

            if (Exists(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.TryAdd(key, value);
            }

            return true;
        }

        /// <summary>
        /// 将Json格式数据转成Setting数据
        /// </summary>
        /// <param name="json">json数据</param>
        /// <returns></returns>
        public void FromJson(string json)
        {
            try
            {
                Clear();
                if (!string.IsNullOrEmpty(json))
                {
                    var jObject = JObject.Parse(json);
                    foreach (var item in jObject)
                    {
                        Add(item.Key, item.Value);
                        if (item.Value.Type == JTokenType.Object)
                        {
                            LoadJson(item.Key, item.Value);
                        }                       
                    }
                }
            }
            finally
            {
                Text = json;
            }
        }

        private void LoadJson(string key ,JToken token)
        {
            foreach (JProperty item in token)
            {
                var _key = $"{key}:{item.Name}";
                Add(_key, item.Value);
                if (item.Value.Type == JTokenType.Object)
                {
                    //加载后来项目
                    LoadJson(_key, item.Value);
                }
            }            
        }

        /// <summary>
        /// 将Xml格式数据转换为Setting数据
        /// </summary>
        /// <param name="xml">Xml数据</param>
        /// <returns></returns>
        public void FromXml(string xml)
        {
            try
            {
                Clear();
                if (!string.IsNullOrEmpty(xml))
                {
                    var xmlDoc = new XmlDocument()
                    {
                        XmlResolver = null
                    };
                    xmlDoc.LoadXml(xml);
                    var xmlElement = xmlDoc.DocumentElement;
                    var nodes = xmlElement.ChildNodes;
                    foreach (XmlNode node in nodes)
                    {                                              
                        Add(node.Name, node);
                        if (node.ChildNodes.Count > 0)
                        {
                            LoadXml(node.Name, node);
                        }
                    }
                }
            }
            finally
            {
                Text = xml;
            }
        }

        private void LoadXml(string key, XmlNode xml)
        {           
            foreach (XmlNode node in xml.ChildNodes)
            {
                Add(node.Name, node);
                if (node.ChildNodes.Count > 0)
                {
                    LoadXml(node.Name, node);
                }
            }
        }
    }
}
