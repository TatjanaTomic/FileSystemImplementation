using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    class FileSystem
    {
        public static readonly int MAX_SIZE_OF_FILE_SYSTEM = 20 * 1024 * 1024; //maksimalna velicina fajl sistema u bajtovima (20MB)
        public static readonly int MAX_SIZE_OF_FILE = 64 * 1024; //maksimalna velicina jedne datoteke u bajtovima (64kB)
        public static readonly int MIN_SIZE_OF_BLOCK = 5; //minimalna velicina jednog bloka za cuvanje sadrzaja datoteke
        public static int numberOfFiles = 0;
        public static int numberOfDirectorys = 0;
        public static int initialSizeOfFS = 0;

        //podaci za datoteku
        public string fileName;
        public readonly int fileId;
        public string filePath;
        public int fileDepth;
        public DateTime dateCreated;
        public DateTime lastTimeModified;
        public DateTime lastTimeOpened;

        //podaci za direktorijum
        public string directoryName;
        public readonly int directoryId;
        public string directoryPath;
        public int directoryDepth;
        public DateTime dirDateCreated;
        public DateTime dirLastTimeModified;
        public DateTime dirLastTimeOpened;

        public void ExecuteFileSystem()
        {

            if (!File.Exists("FileSystem.bin"))
            {
                CreateFileSystem();
            }


        }

        private void CreateFileSystem()
        {
            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.OpenOrCreate));
            
            writer.Write('H'); writer.Write('O'); writer.Write('M'); writer.Write('E');
            writer.Write('~');
            writer.Write(numberOfDirectorys);
            writer.Write('~');
            writer.Write(numberOfFiles);
            writer.Write('~');
            writer.Write(initialSizeOfFS);
            writer.Write('\n');
            writer.Close();
        }
    }
}

//string path = Directory.GetCurrentDirectory(); //putanja do bin foldera projekta