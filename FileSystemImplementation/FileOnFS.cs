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
        public DateTime lastTimeModified;
        public DateTime lastTimeOpened;
        public string[] information = { " " };

        public FileOnFS(string name, int id, DateTime dateC, DateTime dateM, DateTime dateO, string path = "root/", string depth = "1")
        {
            fileName = name;
            fileId = id;
            filePath = path + name;
            fileDepth = Int32.Parse(depth);
            dateCreated = dateC;
            lastTimeModified = dateM;
            lastTimeOpened = dateO;
        }

        internal void WriteToFile()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            string contentOfFS = reader.ReadToEnd();
            reader.Close();

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.WriteLine(firstLine);
            writer.Write("dir~" + fileId + "~" + fileName + "~" + filePath + "~" + fileDepth + "~");
            writer.Write(dateCreated.ToString() + "~" + lastTimeModified.ToString() + "~" + lastTimeOpened.ToString() + "~");
            foreach (var str in information)
                writer.Write(str + "~");
            writer.Write("\r\n" + contentOfFS);
            writer.Close();
        }
    }
}
