using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Altered.Shared.Extensions
{
    public static class XmlSerializerExtensions
    {
        public static string Serialize(this XmlSerializer xmlSerializer, object o, Encoding encoding = null)
        {
            encoding = encoding ?? Encoding.UTF8;
            using (var stream = new MemoryStream())
            {
                xmlSerializer.Serialize(stream, o);
                var bytes = stream.ToArray();
                var str = encoding.GetString(bytes);
                return str;
            }
        }
    }
}
