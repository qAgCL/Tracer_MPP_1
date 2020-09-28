using System.Collections.Generic;

namespace TracerLib
{
    public class TheardTraceResult
    {
        public List<MethodTraceResult> Methods { get; internal set; } = new List<MethodTraceResult>();
        public int TheardID { get; internal set; }
        public long ExecuteTime { get; internal set; }
    }
}
