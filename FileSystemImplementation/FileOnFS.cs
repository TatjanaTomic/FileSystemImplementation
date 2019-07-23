using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    class FileOnFS
    {
        public string fileName;
        public readonly int fileId;
        public string filePath;
        public int fileDepth;
        public DateTime dateCreated;
        public string[] information = { " " };

        public FileOnFS(string name, int id, DateTime dateC, string path = "root/")
        {
            fileName = name;
            fileId = id;
            filePath = path + "/" + name;
            dateCreated = dateC;
        }

        internal void WriteToFile()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            string contentOfFS = reader.ReadToEnd();
            reader.Close();

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.WriteLine(firstLine);
            writer.Write("dir~" + fileId + "~" + fileName + "~" + filePath + "~" + dateCreated.ToString() + "~");
            foreach (var str in information)
                writer.Write(str + "~");
            writer.Write("\r\n" + contentOfFS);
            writer.Close();
        }
    }
}
