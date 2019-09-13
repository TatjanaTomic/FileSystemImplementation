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

        /// <summary>
        /// Funkcija upisuje u FileSystem.bin datoteku podatke o kreiranom fajlu u vidu jednog zapisa u MFT tabeli
        /// MFT = Master File Table
        /// Prepisuje prvu liniju binarne datoteke, zapim upisuje novi zapis, potom prepisuje ostatak sadrzaja datoteke
        /// </summary>
        /// <returns>Funkcija vraca true/false u zavisnosti od toga da li je bilo moguće upisati podatke u datoteku</returns>
        internal bool WriteToMFT()
        {
            byte[] content = File.ReadAllBytes("FileSystem.bin");

            string mftRecord = "file~" + fileId.ToString()
                                 + "~" + fileName
                                 + "~" + filePath
                                 + "~" + dateCreated.ToString()
                                 + "~" + initialSize.ToString();

            if (content.Length + mftRecord.Length + 1 > FileSystem.freeSpace)
            {
                Console.WriteLine("Greska - nije moguce dodati datoteku jer je memorija fajl sistema popunjena.");
                return false;
            }

            BinaryWriter writer1 = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            for (int i = 0; i < 51; i++)
                writer1.Write(content[i]);
            writer1.Close();

            StreamWriter writer2 = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Append));
            writer2.Write(mftRecord + '\n');
            writer2.Close();

            BinaryWriter writer3 = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Append));
            for (int i = 51; i < content.Length; i++)
                writer3.Write(content[i]);
            writer3.Close();

            return true;
        }
    }
}
