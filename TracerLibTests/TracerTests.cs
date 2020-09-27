using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TracerLib;
using System.Threading;

namespace UnitTestProject2
{
    [TestClass]
    public class TracerTests
    {
        static Tracer tracer;
        [TestInitialize]

        public void TestInit() {
            tracer = new Tracer();
        }

        static public class TestEZ
        {
            static public void Test()
            {
                tracer.StartTrace();
                Thread.Sleep(10);
                tracer.StopTrace();
            }
        }
        static public class RecursionTest
        {
            static public void Test(int i)
            {
                tracer.StartTrace();
                i++;
                Thread.Sleep(10);
                if (i < 2)
                {
                    Test(i);
                }
                tracer.StopTrace();
            }
        }

        static public class InsertTest
        {
            static public void InsertTestMethod()
            {
                tracer.StartTrace();
                TestEZ.Test();
                Thread.Sleep(10);
                tracer.StopTrace();
            }
        }
        static public class InsertRecursionTest
        {
            static public void Test(int i)
            {
                tracer.StartTrace();
                i++;
                TestEZ.Test();
                if (i < 2)
                {
                    Test(i);
                }
                tracer.StopTrace();
            }
        }
        static void CheckAreEqual(MethodTraceResult expected, MethodTraceResult actual) {
            Assert.AreEqual(expected.MethodClassName, actual.MethodClassName);
            Assert.AreEqual(expected.MethodName, actual.MethodName);
            Assert.AreEqual(expected.Methods.Count, actual.Methods.Count);
            Assert.IsNotNull(actual.MethodExecuteTime);
        }
        [TestMethod]
        public void TestEZMethod()
        {
            TestEZ.Test();
            var actual = tracer.GetTraceResult().Theards[Thread.CurrentThread.ManagedThreadId].Methods[0];
            var expected = new MethodTraceResult();
            expected.MethodName = "Test";
            expected.MethodClassName = "TestEZ";
            CheckAreEqual(expected, actual);
        }
        [TestMethod]
        public void TestRowMethod()
        {
            TestEZ.Test();
            TestEZ.Test();
            TestEZ.Test();
            var actual = tracer.GetTraceResult().Theards[Thread.CurrentThread.ManagedThreadId].Methods.Count;
            var expected = new MethodTraceResult();
            expected.Methods.Add(new MethodTraceResult());
            expected.Methods.Add(new MethodTraceResult());
            expected.Methods.Add(new MethodTraceResult());
            Assert.AreEqual(expected.Methods.Count, actual);
        }
        [TestMethod]
        public void TestRecMethod()
        {
            RecursionTest.Test(0);
            var actual = tracer.GetTraceResult().Theards[Thread.CurrentThread.ManagedThreadId].Methods[0];
            var expected = new MethodTraceResult();
            expected.MethodName = "Test";
            expected.MethodClassName = "RecursionTest";
            expected.Methods.Add(new MethodTraceResult());
            expected.Methods[0].MethodClassName = "RecursionTest";
            expected.Methods[0].MethodName = "Test";
            CheckAreEqual(expected, actual);
            CheckAreEqual(expected.Methods[0], actual.Methods[0]);
        }
        [TestMethod]
        public void InsertTestMethod()
        {
            InsertTest.InsertTestMethod();
            var actual = tracer.GetTraceResult().Theards[Thread.CurrentThread.ManagedThreadId].Methods[0];
            var expected = new MethodTraceResult();
            expected.MethodName = "InsertTestMethod";
            expected.MethodClassName = "InsertTest";
            expected.Methods.Add(new MethodTraceResult());
            expected.Methods[0].MethodClassName = "TestEZ";
            expected.Methods[0].MethodName = "Test";
            CheckAreEqual(expected, actual);
            CheckAreEqual(expected.Methods[0], actual.Methods[0]);
        }
        [TestMethod]
        public void InsertTestRecMethod()
        {
            InsertRecursionTest.Test(0);
            var actual = tracer.GetTraceResult().Theards[Thread.CurrentThread.ManagedThreadId].Methods[0];
            var expected = new MethodTraceResult();
            expected.MethodName = "Test";
            expected.MethodClassName = "InsertRecursionTest";
            expected.Methods.Add(new MethodTraceResult());
            expected.Methods[0].MethodClassName = "TestEZ";
            expected.Methods[0].MethodName = "Test";
            expected.Methods.Add(new MethodTraceResult());
            expected.Methods[1].MethodClassName = "InsertRecursionTest";
            expected.Methods[1].MethodName = "Test";
            expected.Methods[1].Methods.Add(new MethodTraceResult());
            expected.Methods[1].Methods[0].MethodName = "Test";
            expected.Methods[1].Methods[0].MethodClassName = "TestEZ";
            CheckAreEqual(expected, actual);
            CheckAreEqual(expected.Methods[0], actual.Methods[0]);
            CheckAreEqual(expected.Methods[1], actual.Methods[1]);
            CheckAreEqual(expected.Methods[1].Methods[0], actual.Methods[1].Methods[0]);
        }
    }
}
