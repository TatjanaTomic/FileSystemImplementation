using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    //TODO: Ograniciti duzinu naziva
    //TODO: Pocetni sadrzaj direktorijuma treba da bude space a ne prazan string kasnije zbog meciranja
    //TODO: Sadrzaj direktorijuma treba cuvati u listi a ne statickom nizu stringova!!!
    //DONE: Kod update-a moram dodavati u listu (sadrzaj root foldera) ako vec ne postoji ta datoteka/direktorijum
    //DONE: Promijeni putanje direktorijuma - path+name i onda u update i load data reba promijeniti uslov "~root/~" jer nece nikad nista mecirati
    class Directory
    {
        public string directoryName;
        public readonly int directoryId;
        public string directoryPath;
        public int directoryDepth;
        public DateTime dirDateCreated;
        public DateTime dirLastTimeModified;
        public DateTime dirLastTimeOpened;
        public string[] contentOfDir = { " " };

        public Directory(string name, int id,  DateTime dateC, DateTime dateM, DateTime dateO, string path = "root/", int depth = 1)
        {
            directoryName = name;
            directoryId = id;
            directoryPath = path + name;
            directoryDepth = depth;
            dirDateCreated = dateC;
            dirLastTimeModified = dateM;
            dirLastTimeOpened = dateO;
        }

        internal void WriteToFile()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            string contentOfFS = reader.ReadToEnd();
            reader.Close();

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.WriteLine(firstLine);
            writer.Write("dir~" + directoryId + "~" + directoryName + "~" + directoryPath + "~" + directoryDepth + "~");
            writer.Write(dirDateCreated.ToString() + "~" + dirLastTimeModified.ToString() + "~" + dirLastTimeOpened.ToString() + "~");
            foreach (var str in contentOfDir)
                writer.Write(str + "~");
            writer.Write("\r\n" + contentOfFS);
            writer.Close();

        }
    }
}
