using System;
using System.Collections.Generic;
using System.Reflection;

namespace Api.Config
{
    public class DictionaryUtil
    {
        /// <summary>
        /// 把Model转换为DataRow
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mod"></param>
        /// <returns></returns>
        public static T ToModel<T>(Dictionary<string, string> dict)
        {           
            T obj = default(T);
            obj = Activator.CreateInstance<T>();
           
            foreach (KeyValuePair<string, string> item in dict)
            {
                PropertyInfo prop = obj.GetType().GetProperty(item.Key);
                if (!string.IsNullOrEmpty(item.Value))
                {
                    object value = item.Value;                    
                    Type itemType = Nullable.GetUnderlyingType(prop.PropertyType) == null ? prop.PropertyType : Nullable.GetUnderlyingType(prop.PropertyType);
                    //根据Model类字段的真实类型进行转换
                    prop.SetValue(obj, Convert.ChangeType(value, itemType), null);
                }
            }
            return obj;
        }

        public static Dictionary<string, string> ToDictionary(Object obj)
        {
            var cfgItemProperties = obj.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            var dict = new Dictionary<string, string>();
            foreach (PropertyInfo item in cfgItemProperties)
            {
                string name = item.Name;
                object value = item.GetValue(obj, null);
                if (value != null && (item.PropertyType.IsValueType || item.PropertyType.Name.StartsWith("String")) && !string.IsNullOrWhiteSpace(value.ToString()))
                {
                    dict.Add(name, value.ToString());
                }
            }
            return dict;
        }
    }
}
