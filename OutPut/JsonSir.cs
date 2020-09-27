using System.Text;
using Newtonsoft.Json;
using System.IO;
using TracerLib;
namespace ConsoleOut
{
    class JsonSir : ISir
    {
        public Stream Serialize(TraceResult TraceResult)
        {
            string buffer = JsonConvert.SerializeObject(TraceResult, Newtonsoft.Json.Formatting.Indented);
            byte[] byteArray = Encoding.UTF8.GetBytes(buffer);
            System.IO.Stream stream = new System.IO.MemoryStream(byteArray);
            return stream;
        }
    }

}
