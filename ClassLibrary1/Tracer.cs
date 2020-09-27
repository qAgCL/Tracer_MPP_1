using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Concurrent;

namespace TracerLib
{
        public class Tracer : ITracer
        {
            private ConcurrentDictionary<int, List<Stopwatch>> StackExuc = new ConcurrentDictionary<int, List<Stopwatch>>();
            private TraceResult TraceInfo = new TraceResult();
            private ConcurrentDictionary<int, int> MethodStack = new ConcurrentDictionary<int, int>();
            private void Method(int idTheard, string MethodName, string ClassName)
            {
                List<MethodTraceResult> ListMethod = new List<MethodTraceResult>();
                ListMethod = TraceInfo.Theards[idTheard].Methods;
                for (int i = 1; i < MethodStack[idTheard]; i++)
                {
                    ListMethod = ListMethod[ListMethod.Count - 1].Methods;
                }
                MethodTraceResult InfoMethod = new MethodTraceResult();
                InfoMethod.MethodName = MethodName;
                InfoMethod.MethodClassName = ClassName;
                ListMethod.Add(InfoMethod);
            }
            public void StartTrace()
            {
                StackTrace stackTrace = new StackTrace();
                StackFrame[] stackFrames = stackTrace.GetFrames();
                StackFrame callingFrame = stackFrames[1];
                MethodInfo method = (MethodInfo)callingFrame.GetMethod();
                string MethodName = method.Name;
                string ClassMethodName = method.DeclaringType.Name;
                TheardTraceResult TheardCur = new TheardTraceResult();
                int idTheard = Thread.CurrentThread.ManagedThreadId;
                if (TraceInfo.Theards.TryAdd(idTheard, TheardCur))
                {
                    TraceInfo.Theards[idTheard].TheardID = idTheard;
                    MethodStack.TryAdd(idTheard, 0);
                    StackExuc.TryAdd(idTheard, new List<Stopwatch>());
                }
                MethodStack[idTheard]++;
                StackExuc[idTheard].Add(new Stopwatch());
                Method(idTheard, MethodName, ClassMethodName);
                StackExuc[idTheard][StackExuc[idTheard].Count - 1].Start();
            }
            public void StopTrace()
            {
                int idTheard = Thread.CurrentThread.ManagedThreadId;
                StackExuc[idTheard][StackExuc[idTheard].Count - 1].Stop();
                List<MethodTraceResult> ListMethod = new List<MethodTraceResult>();
                ListMethod = TraceInfo.Theards[idTheard].Methods;
                for (int i = 1; i < MethodStack[idTheard]; i++)
                {
                    ListMethod = ListMethod[ListMethod.Count - 1].Methods;
                }
                ListMethod[ListMethod.Count - 1].MethodExecuteTime = StackExuc[idTheard][StackExuc[idTheard].Count - 1].ElapsedMilliseconds;
                StackExuc[idTheard].Remove(StackExuc[idTheard][StackExuc[idTheard].Count - 1]);
                MethodStack[idTheard]--;
            }
            public TraceResult GetTraceResult()
            {
                foreach (KeyValuePair<int, TheardTraceResult> theard in TraceInfo.Theards)
                {
                    long time = 0;
                    foreach (MethodTraceResult Method in theard.Value.Methods)
                    {
                        time += Method.MethodExecuteTime;
                    }
                    theard.Value.ExecuteTime=time;
                }
                return TraceInfo;
            }
    }
}
