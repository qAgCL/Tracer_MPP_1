using System.IO;
using TracerLib;
namespace ConsoleOut
{
    interface ISir
    {
        Stream Serialize(TraceResult TraceResult);
    }
}
