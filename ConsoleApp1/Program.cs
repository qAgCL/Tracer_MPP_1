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
        static void Main(string[] args)
        {

            Tracer test = new Tracer();
            //Foo test2 = new Foo(test);
            //test2.MyMethod();
            Test asd = new Test(test);
            asd.Rec(1);
            XmlSir xmlSir = new XmlSir();
            XmlOutPut xmlOutPut = new XmlOutPut();
            xmlOutPut.ConsoleOut(xmlSir.Serialize(test.GetTraceResult()));
    
            Console.ReadLine();
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
            _tracer.StopTrace();
        }
    }
    public class Tracer: ITracer
    {

        private Stopwatch ExecTime = new Stopwatch();
        private static TraceResult TraceInfo = new TraceResult();
        private static ConcurrentDictionary<int, int> MethodStack = new ConcurrentDictionary<int, int>();
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
            }
            MethodStack[idTheard]++;
            Method(idTheard, MethodName, ClassMethodName);
            ExecTime.Start();
        }
        public void StopTrace() {
            ExecTime.Stop();
            int idTheard = Thread.CurrentThread.ManagedThreadId;
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
        public int MethodExecuteTime;
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
