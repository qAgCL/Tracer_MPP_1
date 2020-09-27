using System;
using System.Xml;
using System.IO;
namespace ConsoleOut
{
    class XmlOutPut : IOutPut
    {
        public void ConsoleOut(Stream stream)
        {
            XmlDocument XMLDoc = new XmlDocument();
            stream.Position = 0;
            XMLDoc.Load(stream);
            XMLDoc.Save(Console.Out);
        }
        public void FileOut(Stream stream, string FileName)
        {
            XmlDocument XMLDoc = new XmlDocument();
            stream.Position = 0;
            XMLDoc.Load(stream);
            XMLDoc.Save(FileName);
        }
    }

}
