using System.Xml;
using System.IO;
using System.Collections.Generic;
using TracerLib;

namespace ConsoleOut
{
    class XmlSir : ISir
    {
        public Stream Serialize(TraceResult TraceResult)
        {
            XmlDocument XMLDoc = new XmlDocument();
            System.IO.Stream stream = new System.IO.MemoryStream();
            XmlDeclaration XMLDec = XMLDoc.CreateXmlDeclaration("1.0", "utf-8", null);
            XMLDoc.AppendChild(XMLDec);
            XmlElement XmlRoot = XMLDoc.CreateElement("root");
            XMLDoc.AppendChild(XmlRoot);
            foreach (KeyValuePair<int, TheardTraceResult> theard in TraceResult.Theards)
            {
                XmlElement XmlTheardElement = XMLDoc.CreateElement("theard");
                XmlTheardElement.SetAttribute("id", theard.Value.TheardID.ToString());
                XmlTheardElement.SetAttribute("time", theard.Value.ExecuteTime.ToString()+"ms");
                GetInfo(theard.Value.Methods, XMLDoc, XmlTheardElement);
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
}
