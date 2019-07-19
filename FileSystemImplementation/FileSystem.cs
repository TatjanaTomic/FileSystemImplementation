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

        public static int EXIT = 0;

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

                Console.Write("HOME>");

                string line = Console.ReadLine();
                string[] words = line.Split(' ');

                switch (words[0])
                {
                    case "-1":
                        Environment.Exit(EXIT);
                        break;

                    case "help":
                        Help(words[1]);
                        break;
                }
            
        }

        private void Help(string method)
        {
            switch(method)
            {
                case "list":
                    Console.WriteLine("Lista svih ocija: mkdir create put get ls cp mv rename echo cat rm stat");
                    Console.WriteLine("Za detaljno uputstvo unesite \"help naziv_opcije\"\n");
                    break;

                case "mkdir":
                    Console.WriteLine("Kreira novi direktorijum na zadatoj putanji, prilikom poziva potrebno je navesti naziv_komande naziv_direktorijuma\n");
                    break;

                case "create":
                    Console.WriteLine("Kreira novu datoteku na zadatoj putanji, prilikom poziva potrebno je navesti naziv_komande naziv_datoteke\n");
                    break;

                case "put":
                    Console.WriteLine("Omogucava dodavanje datoteke sa fajl sistema racunara na novi fajl sistem, prilikom poziva potrebno je navesti naziv_komande izvorna_putanja krajnja_putanja\n");
                    break;

                case "get":
                    Console.WriteLine("Omogucava dodavanje datoteke sa novog fajl sistema na fajl sistema racunara, prilikom poziva potrebno je navesti naziv_komande izvorna_putanja krajnja_putanja\n");
                    break;

                case "ls":
                    Console.WriteLine("Ispisuje sadrzaj direktorijuma na trenutnoj putanji\n");
                    break;

                case "cp":
                    Console.WriteLine("Kreira kopiju datoteke na trenutnoj putanji\n");
                    break;

                case "mv":
                    Console.WriteLine("Omogucava premjestanje datoteke sa trenutne putanje na zadatu lokaciju, prilikom poziva potrebno je navesti naziv_komande naziv_fajla krajnja_putanja\n");
                    break;

                case "rename":
                    Console.WriteLine("Omogucava promjenu naziva datoteke ili diretorijuma na trenutnoj putanji. U slucaju promjene naziva datoteke potrebno je navesti naziv_komande trenutni_naziv_datoteke novi_naziv_datoteke. U slucaju promjene naziva direktorijuma potrebno je navesti naziv_komande novi_naziv_direktorijuma\n");
                    break;

                case "echo":
                    Console.WriteLine("Omogucava upis tekstualnog sadrzaja u datoteku, prilikom poziva poptreno je navesti naziv_komande naziv_datoteke tekstualni_sadrzaj\n");
                    break;

                case "cat":
                    Console.WriteLine("Omogucava prikaz sadrzaja tekstualne datoteke\n");
                    break;

                case "rm":
                    Console.WriteLine("Omogucava brisanje datoteke ili direktorijuma sa trenutne putanje. U slucaju brisanja kompletnog brisanja sadrzaja direktorijuma potrebno je navesti \"rm - r\". Prilikom poziva potrebno je navesti naziv_komande naziv_dat/dir\n");
                    break;

                case "stat":
                    Console.WriteLine("Omogucava ispis podataka o datoteci, prilikom poziva potrebno je navesti naziv_komande naziv_datoteke\n");
                    break;

                default:
                    Console.WriteLine("Greska - nepostojeca opcija! Za listu svih opcija unesite \"help list\"\n");
                    break;


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