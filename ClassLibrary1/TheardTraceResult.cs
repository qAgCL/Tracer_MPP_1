using System.Collections.Generic;

namespace TracerLib
{
    public class TheardTraceResult
    {
        public List<MethodTraceResult> Methods = new List<MethodTraceResult>();
        public int TheardID;
        public long ExecuteTime;
    }
}
