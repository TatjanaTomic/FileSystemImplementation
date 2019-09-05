using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    //TODO: Dodati mogucnost za mkdir i create da se kreiraju na unesenoj putanji
    //TODO: Kao dodatnu komandu mogu dodati find !!!!!!
    class Directory
    {
        public string directoryName;
        public readonly int directoryId;
        public string directoryPath;
        public DateTime dateCreated;

        public Directory(string name, int id,  DateTime dateC, string path = "root/")
        {
            directoryName = name.Replace('.', '-'); //Ovo nije pametno jer ako vec postoji folder sa nazivom folder-1 a mi pokusamo dodati folder.1, proci ce, ali meni se ne da jos i to dodavati 
            directoryId = id;
            directoryPath = path + directoryName;
            dateCreated = dateC;
        }

        internal void WriteToFile()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            string contentOfFS = reader.ReadToEnd();
            reader.Close();

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.Write(firstLine + '\n');
            writer.Write("dir~" + directoryId + "~" + directoryName + "~" + directoryPath + "~" + dateCreated.ToString() + "~");
            writer.Write('\n' + contentOfFS);
            writer.Close();

        }
    }
}
