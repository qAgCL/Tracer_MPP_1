using System.Collections.Generic;

namespace TracerLib
{
    public class MethodTraceResult
    {
        public string MethodName;
        public string MethodClassName;
        public long MethodExecuteTime;
        public List<MethodTraceResult> Methods = new List<MethodTraceResult>();
    }
}
