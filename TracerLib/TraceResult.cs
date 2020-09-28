using System.Collections.Concurrent;


namespace TracerLib
{
    public class TraceResult
    {
        public ConcurrentDictionary<int, TheardTraceResult> Theards { get; internal set; } = new ConcurrentDictionary<int, TheardTraceResult>();
    }
}
