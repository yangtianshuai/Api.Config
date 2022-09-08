using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Api.Config.Core
{
    public static class XmlExtension
    {
        public static T ToObject<T>(this XmlNode node)
        {
            var type = typeof(T);
            XmlSerializer xmldes = new XmlSerializer(type);
            
            using (System.IO.MemoryStream mem = new MemoryStream(Encoding.Default.GetBytes(node.OuterXml)))
            {
                using (XmlReader reader = XmlReader.Create(mem))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(T));
                    return (T)formatter.Deserialize(reader);
                }
            }          
        }
    }
}
