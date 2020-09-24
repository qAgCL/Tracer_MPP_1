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
using TracerLib;
namespace ConsoleOut
{
    class Program
    {
        static Tracer tracer = new Tracer();
        static void Main(string[] args)
        {
            Test test1 = new Test(tracer);
            test1.Rec(-1);
            XmlSir xmlSir = new XmlSir();
            XmlOutPut xmlOutPut = new XmlOutPut();

            xmlOutPut.ConsoleOut(xmlSir.Serialize(tracer.GetTraceResult()));
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

        public void Rec(int i)
        {
            _tracer.StartTrace();
            i++;
            Thread.Sleep(4);
            if (i < 4) Rec(i);
            _tracer.StopTrace();
        }
    }
}