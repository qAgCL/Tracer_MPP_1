using System;
using System.Xml;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Reflection;
using System.Collections.Concurrent;

namespace ConsoleApp1
{
    class Program
    {
        static Tracer test = new Tracer();
        static void Main(string[] args)
        {
            

           
            Thread myThread = new Thread(new ThreadStart(Theard));
            myThread.Start(); // запускаем поток
            Test test1 = new Test(test);
            test1.Rec(1);
            Thread.Sleep(500);
            XmlSir xmlSir = new XmlSir();
            XmlOutPut xmlOutPut = new XmlOutPut();

            xmlOutPut.ConsoleOut(xmlSir.Serialize(test.GetTraceResult()));
            Console.ReadLine();
            }

        public static void Theard()
        {
            Test test1 = new Test(test);
            test1.Rec(2);
        }
    }
    public class Test
    {
        private ITracer _tracer;
        internal Test(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void Rec ( int i)
        {
            _tracer.StartTrace();
            i++;
            Thread.Sleep(4);
            if (i < 4) Rec(i);
            _tracer.StopTrace();
        }
    }
    public class Foo
    {
        private Bar _bar;
        private ITracer _tracer;
        internal Foo(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Bar(_tracer);
        }
        public void MyMethod()
        {
            _tracer.StartTrace();
            _bar.InnerMethod();
            Thread.Sleep(5);    
            _tracer.StopTrace();
        }
    }

    public class Bar
    {
        private ITracer _tracer;
        internal Bar(ITracer tracer)
        {
            _tracer = tracer;
        }
        public void InnerMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(4);
            _tracer.StopTrace();
        }
    }
    public class Tracer: ITracer
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
        public void StartTrace() {
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
        public void StopTrace() {
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
            public TraceResult GetTraceResult( )
            {
                return TraceInfo;
            }
            
    }
    public class JsonSir: ISir
    {
        public Stream Serialize(TraceResult TraceResult)
        {
            string buffer = JsonConvert.SerializeObject(TraceResult, Newtonsoft.Json.Formatting.Indented);
            byte[] byteArray = Encoding.UTF8.GetBytes(buffer);
            System.IO.Stream stream = new System.IO.MemoryStream(byteArray);
            return stream;
        }
    }
    public class XmlSir: ISir
    {
        public Stream Serialize(TraceResult TraceResult)
        {
            XmlDocument XMLDoc = new XmlDocument();
            System.IO.Stream stream = new System.IO.MemoryStream();
            XmlDeclaration XMLDec = XMLDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XMLDoc.AppendChild(XMLDec);
            XmlElement XmlRoot = XMLDoc.CreateElement("root");
            XMLDoc.AppendChild(XmlRoot);
            int i = 1;
            foreach (KeyValuePair<int,TheardTraceResult> theard in TraceResult.Theards)
            {
                XmlElement XmlTheardElement = XMLDoc.CreateElement("theard");
                XmlTheardElement.SetAttribute("id", theard.Value.TheardID.ToString());
                GetInfo(theard.Value.Methods, XMLDoc, XmlTheardElement);
                i++;
                XmlRoot.AppendChild(XmlTheardElement);
            }
            XMLDoc.Save(stream);
            return stream;
        }
        static void GetInfo(List<MethodTraceResult> Methods, XmlDocument XmlDoc, XmlElement XmlMethod)
        {
            foreach (MethodTraceResult Method in Methods)
            {
                XmlElement XmlMethodElement = XmlDoc.CreateElement("method");
                XmlMethodElement.SetAttribute("name", Method.MethodName);
                XmlMethodElement.SetAttribute("time", Method.MethodExecuteTime.ToString() + "ms");
                XmlMethodElement.SetAttribute("class", Method.MethodClassName);
                if (Method.Methods.Count > 0)
                {
                    GetInfo(Method.Methods, XmlDoc, XmlMethodElement);
                }
                XmlMethod.AppendChild(XmlMethodElement);
            }
        }
    }
    public class XmlOutPut: IOutPut
    {
        public void ConsoleOut(Stream stream)
        {
            XmlDocument XMLDoc = new XmlDocument();
            stream.Position = 0;
            XMLDoc.Load(stream);
            XMLDoc.Save(Console.Out);
        }
        public void FileOut(Stream stream,string FileName)
        {
            XmlDocument XMLDoc = new XmlDocument();
            stream.Position = 0;
            XMLDoc.Load(stream);
            XMLDoc.Save(FileName);
        }
    }
    public class JsonOutPut: IOutPut
    {
        public void ConsoleOut(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string JSON = reader.ReadToEnd();
            Console.WriteLine(JSON);
        }

        public void FileOut(Stream stream,string FileName)
        {
            StreamReader reader = new StreamReader(stream);
            string JSON = reader.ReadToEnd();
            using (StreamWriter sw = new StreamWriter(FileName, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(JSON);
            }
        }
    }
    
    public class MethodTraceResult
    {
        public string MethodName;
        public string MethodClassName;
        public long MethodExecuteTime;
        public List<MethodTraceResult> Methods = new List<MethodTraceResult>();
    }
    public class TheardTraceResult
    {
        public List<MethodTraceResult> Methods = new List<MethodTraceResult>();
        public int TheardID;
    }
    public class TraceResult
    {
        //public List<TheardTraceResult> Theards = new List<TheardTraceResult>();
        public ConcurrentDictionary<int, TheardTraceResult> Theards = new ConcurrentDictionary<int, TheardTraceResult>();
    }
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        TraceResult GetTraceResult();
    }
    public interface IOutPut
    {
        void ConsoleOut(Stream stream);
        void FileOut(Stream stream, string FileName);
    }
    public interface ISir
    {
        Stream Serialize(TraceResult TraceResult);
    }
  
}
