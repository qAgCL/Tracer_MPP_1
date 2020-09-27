using System.IO;

namespace ConsoleOut
{
    interface IOutPut
    {
        void ConsoleOut(Stream stream);
        void FileOut(Stream stream, string FileName);
    }
}
