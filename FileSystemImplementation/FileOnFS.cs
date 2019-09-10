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
            byte[] content = File.ReadAllBytes("FileSystem.bin");
            int start = 0;
            for (int i = 0; i < content.Length; i++)
            {
                if (content[i] == '\n')
                {
                    start = i + 1;
                    break;
                }
            }

            BinaryWriter writer1 = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            for (int i = 0; i < start; i++)
                writer1.Write(content[i]);
            writer1.Close();

            StreamWriter writer2 = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Append));
            writer2.Write("file~" + fileId + "~" + fileName + "~" + filePath + "~" + dateCreated.ToString() + "~" + initialSize + '\n');
            writer2.Close();

            BinaryWriter writer3 = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Append));
            for (int i = start; i < content.Length; i++)
                writer3.Write(content[i]);
            writer3.Close();
            /*
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            string contentOfFS = reader.ReadToEnd();
            reader.Close();

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            writer.Write(firstLine + '\n');
            writer.Write("file~" + fileId + "~" + fileName + "~" + filePath + "~" + dateCreated.ToString() + "~" + initialSize);
            writer.Write('\n' + contentOfFS);
            writer.Close();
            */
        }
    }
}
