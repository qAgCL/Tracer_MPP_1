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
        static Tracer tracer = new Tracer() ;
        static void Main(string[] args)
        {
            
            //Thread myThread = new Thread(new ThreadStart(Theard));
            //myThread.Start(); // запускаем поток


            Thread Thread1 = new Thread(new ThreadStart(TheardTest1));
            Thread1.Start(); 

            Thread Thread2 = new Thread(new ThreadStart(TheardTest2));
            Thread2.Start();

            MetWithoutTracer metWithoutTracer = new MetWithoutTracer(tracer);

            metWithoutTracer.Without();
            Foobar foobar = new Foobar(tracer);
            foobar.FoobarMethod();
            metWithoutTracer.Without();
            metWithoutTracer.Without();
            
            Thread.Sleep(1000);

            XmlSir xmlSir = new XmlSir();
            XmlOutPut xmlOutPut = new XmlOutPut();
            JsonSir jsonSir = new JsonSir();
            JsonOutPut jsonOutPut = new JsonOutPut();

            xmlOutPut.ConsoleOut(xmlSir.Serialize(tracer.GetTraceResult()));
            jsonOutPut.ConsoleOut(jsonSir.Serialize(tracer.GetTraceResult()));

            xmlOutPut.FileOut(xmlSir.Serialize(tracer.GetTraceResult()), @"test.xml");
            jsonOutPut.FileOut(jsonSir.Serialize(tracer.GetTraceResult()), @"test.txt");
            Console.ReadLine();
        }
        public static void TheardTest1()
        {
            RecursionTest recursion = new RecursionTest(tracer);
            recursion.Recursion(2);
            Foobar foobar = new Foobar(tracer);
            foobar.FoobarMethod();
            recursion.Recursion(3);
            Bar bar = new Bar(tracer);
            bar.BarMethod();
        }
        public static void TheardTest2()
        {
            FooRecTest foorec = new FooRecTest(tracer);
            foorec.FooRec(1);

            Foobar foobar = new Foobar(tracer);
            foobar.FoobarMethod();
        }
    }

    public class MetWithoutTracer
    {
        private ITracer _tracer;
        private Foo _Foo;
        internal MetWithoutTracer(ITracer tracer)
        {
            _tracer = tracer;
            _Foo = new Foo(_tracer);
        }
        public void Without()
        {
            _Foo.FooMethod();
        }
    }
    public class FooRecTest
    {
        private Foo _foo;
        private ITracer _tracer;
        internal FooRecTest(ITracer tracer)
        {
            _tracer = tracer;
            _foo = new Foo(_tracer);
        }

        public void FooRec(int i)
        {
            _tracer.StartTrace();
            i++;
            _foo.FooMethod();
            if (i < 5) FooRec(i);
            _tracer.StopTrace();
        }
    }
    public class RecursionTest
    {
        private ITracer _tracer;
        internal RecursionTest(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void Recursion(int i)
        {
            _tracer.StartTrace();
            i++;
            Thread.Sleep(4);
            if (i < 5) Recursion(i);
            _tracer.StopTrace();
        }
    }
    public class Foobar
    {
        private Foo _bar;
        private ITracer _tracer;
        internal Foobar(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Foo(_tracer);
        }
        public void FoobarMethod()
        {
            _tracer.StartTrace();
            _bar.FooMethod();
            Thread.Sleep(5);
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
        public void FooMethod()
        {
            _tracer.StartTrace();
            _bar.BarMethod();
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
        public void BarMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(4);
            _tracer.StopTrace();
        }
    }


}