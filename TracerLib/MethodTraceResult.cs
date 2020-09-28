using System.Collections.Generic;

namespace TracerLib
{
    public class MethodTraceResult
    {
        public string MethodName { get; internal set; }
        public string MethodClassName { get; internal set; }
        public long MethodExecuteTime { get; internal set; }
        public List<MethodTraceResult> Methods { get; internal set; } = new List<MethodTraceResult>();
    }
}
