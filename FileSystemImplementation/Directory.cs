using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    //TODO: Dodati mogucnost za mkdir i create da se kreiraju na unesenoj putanji
    //DONE: Ograniciti duzinu naziva - KADA BUDEM PRAVILA MakeFile() treba da provjerim duzinu prije poziva checkName()
    //TODO: Sadrzaj direktorijuma treba cuvati u listi, a ne statickom nizu stringova!!!
    //TODO: Kao dodatnu komandu mogu dodati find !!!!!!
    //DONE: Promijeni putanje direktorijuma - path+name i onda u update i load data reba promijeniti uslov "~root/~" jer nece nikad nista mecirati
    class Directory
    {
        public string directoryName;
        public readonly int directoryId;
        public string directoryPath;
        public int directoryDepth;
        public DateTime dateCreated;
        public string[] contentOfDir = { " " };

        public Directory(string name, int id,  DateTime dateC, string path = "root/", int depth = 1)
        {
            directoryName = name;
            directoryId = id;
            directoryPath = path + name;
            directoryDepth = depth;
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
            foreach (var str in contentOfDir)
                writer.Write(str + "~");
            writer.Write('\n' + contentOfFS);
            writer.Close();

        }
    }
}
