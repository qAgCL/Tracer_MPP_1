using System.Collections.Concurrent;


namespace TracerLib
{
    public class TraceResult
    {
        public ConcurrentDictionary<int, TheardTraceResult> Theards = new ConcurrentDictionary<int, TheardTraceResult>();
    }
}
