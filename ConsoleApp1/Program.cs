using System;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            TraceResult test = new TraceResult();
            var theard1 = new TheardTraceResult();
            var theard2 = new TheardTraceResult();
            test.Theards.Add(theard1);
            test.Theards.Add(theard2);
            theard1.Methods.Add(new MethodTraceResult());
            test.Theards[0].Methods[0].Methods.Add(new MethodTraceResult());
            test.Theards[0].Methods[0].Methods[0].Methods.Add(new MethodTraceResult());
            XmlSir.Serialize(test);

            StreamReader reader = new StreamReader(JsonSir.Serialize(test));
            string text = reader.ReadToEnd();
            Console.WriteLine(text);
            Console.ReadLine();
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
            foreach (TheardTraceResult theard in TraceResult.Theards)
            {
                XmlElement XmlTheardElement = XMLDoc.CreateElement("theard");
                XmlTheardElement.SetAttribute("id", i.ToString());
                GetInfo(theard.Methods, XMLDoc, XmlTheardElement);
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
    public class testim { };
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
    }
    public class TraceResult
    {
        public List<TheardTraceResult> Theards = new List<TheardTraceResult>();
    }
    public interface ITracer
    {
        void StartTrace();
        void StopTrace();
        TraceResult GetTraceResult();
    }

    public interface ISir
    {
        Stream Serialize(TraceResult TraceResult);
    }
  
}
