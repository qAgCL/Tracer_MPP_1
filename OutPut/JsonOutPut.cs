using System;
using System.IO;
namespace ConsoleOut
{
    class JsonOutPut : IOutPut
    {
        public void ConsoleOut(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string JSON = reader.ReadToEnd();
            Console.WriteLine(JSON);
        }

        public void FileOut(Stream stream, string FileName)
        {
            StreamReader reader = new StreamReader(stream);
            string JSON = reader.ReadToEnd();
            using (StreamWriter sw = new StreamWriter(FileName, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(JSON);
            }
        }
    }
}
