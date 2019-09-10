using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{ 
    class Directory
    {
        public string directoryName;
        public readonly int directoryId;
        public string directoryPath;
        public DateTime dateCreated;

        public Directory(string name, int id,  DateTime dateC, string path = "root/")
        {
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            directoryName = name.Replace('.', '-'); //Ovo nije pametno jer ako vec postoji folder sa nazivom folder-1 a mi pokusamo dodati folder.1, proci ce, ali meni se ne da jos i to dodavati 
            directoryId = id;
            directoryPath = path + directoryName;
            dateCreated = dateC;
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
            writer2.Write("dir~" + directoryId + "~" + directoryName + "~" + directoryPath + "~" + dateCreated.ToString() + "~" + '\n');
            writer2.Close();

            BinaryWriter writer3 = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Append));
            for (int i = start; i < content.Length; i++)
                writer3.Write(content[i]);
            writer3.Close();
            /*
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            string contentOfFS = reader.ReadToEnd(); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            reader.Close();

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.Write(firstLine + '\n');
            writer.Write("dir~" + directoryId + "~" + directoryName + "~" + directoryPath + "~" + dateCreated.ToString() + "~");
            writer.Write('\n' + contentOfFS);
            writer.Close();
            */
        }
    }
}
