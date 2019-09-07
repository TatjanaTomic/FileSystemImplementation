using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    //file~id~naziv~putanja~datum~velicina
    class FileOnFS
    {
        public string fileName;
        public readonly int fileId;
        public string filePath;
        public DateTime dateCreated;
        public int initialSize;

        public FileOnFS(string name, int id, DateTime dateC, string path = "root/", int size = 0)
        {
            fileName = name;
            fileId = id;
            filePath = path + name;
            dateCreated = dateC;
            initialSize = size;
        }

        internal void WriteToFile()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            string contentOfFS = reader.ReadToEnd();
            reader.Close();

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.Write(firstLine + '\n');
            writer.Write("file~" + fileId + "~" + fileName + "~" + filePath + "~" + dateCreated.ToString() + "~" + initialSize);
            writer.Write('\n' + contentOfFS);
            writer.Close();
        }
    }
}
