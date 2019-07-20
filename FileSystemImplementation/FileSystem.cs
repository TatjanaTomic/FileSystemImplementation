using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    class FileSystem
    {
        public static readonly int MAX_SIZE_OF_FILE_SYSTEM = 20 * 1024 * 1024; //maksimalna velicina fajl sistema u bajtovima (20MB)
        public static readonly int MAX_SIZE_OF_FILE = 64 * 1024; //maksimalna velicina jedne datoteke u bajtovima (64kB)
        public static readonly int MIN_SIZE_OF_BLOCK = 5; //minimalna velicina jednog bloka za cuvanje sadrzaja datoteke
        public static readonly int initialUsedSpace = 30; //prilikom kreiranja fajl sistema velicina je 30 bajta zbog upisivanja inicijalnih vrijednosti o fajl sistemu
        public static int numberOfFiles = 0;
        public static int numberOfDirectories = 0;
        public static int usedSpace;
        public static int freeSpace;
        public static int EXIT = 0;

        //podaci za datoteku
        public string fileName;
        public readonly int fileId;
        public string filePath;
        public int fileDepth;
        public DateTime dateCreated;
        public DateTime lastTimeModified;
        public DateTime lastTimeOpened;


        public void ExecuteFileSystem()
        {

            if (!File.Exists("FileSystem.bin"))
            {
                CreateFileSystem();
            }

            while (true)
            {
                Console.Write("HOME>");

                string line = Console.ReadLine();
                string[] words = line.Split(' ');

            
                switch (words[0])
                {
                    case "exit":
                        Environment.Exit(EXIT);
                        break;

                    case "help":
                        try
                        {
                            Help(words[1]);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Za pomoc pri radu unesite \"help list\"\n");
                        }
                        break;

                    case "df":
                        try
                        {
                            bool flag = (words[1] == "-h"); 
                            if (flag)
                            {
                                GetRootInformation(); //df -h je za detaljan prikaz
                            }
                            else // df pa nesto sto nije -h je pogresna komanda
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help df\" za pomoc\n");
                            }
                        }
                        catch(Exception) //ako je doslo do izuzetka, to znaci da je uneseno samo df
                        {
                            GetRootInformation(1); //df je za prikaz slobodnog prostora

                        }
                        break;

                    case "mkdir":
                        try
                        {
                            //MakeDirectory(path, words[1]);
                        }
                        catch(Exception)
                        {
                            Console.WriteLine("Greska - Netacan unos! Unesite \"help mkdir\" za pomoc\n");
                        }
                        break;

                    case "create":
                        break;

                    case "put":
                        break;

                    case "get":
                        break;

                    case "ls":
                        break;

                    case "cp":
                        break;

                    case "mv":
                        break;

                    case "rename":
                        break;

                    case "echo":
                        break;

                    case "cat":
                        break;

                    case "rm":
                        break;

                    case "stat":
                        break;

                    default:
                        Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help list\"\n");
                        break;
                }
            }
        }

        private void GetRootInformation(int a = 0)
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();

            Regex regex = new Regex(@"HOME~([0-9]+)~([0-9]+)~([0-9]+)~([0-9]+)~([0-9]+)");
            Match match = regex.Match(line);

            if (a == 1)
            {
                Console.WriteLine("Slobodan memorijski prostor: " + match.Groups[4] + "b\n");
            }
            else
            {
                Console.WriteLine("Broj direktorijuma: " + match.Groups[1]);
                Console.WriteLine("Broj datoteka: " + match.Groups[2]);
                Console.WriteLine("Iskoriscen memorijski prostor: " + match.Groups[3] + "b");
                Console.WriteLine("Slobodan memorijski prostor: " + match.Groups[4] + "b");
                Console.WriteLine("Ukupna velicina fajl sistema: " + match.Groups[5] + "b\n");
            }

            reader.Close();

            //Drugi nacin
            //Regex regex2 = new Regex(@"[0-9]+");
            //MatchCollection matches = regex2.Matches(line);
            //Console.WriteLine(matches[0]);
            //Console.WriteLine(matches[1]);
            //Console.WriteLine(matches[2]);
            //Console.WriteLine(matches[3]);

        }

        private void MakeDirectory(string name, string path = "\root")
        {
           throw new NotImplementedException();
        }

        private void Help(string method)
        {
            switch(method)
            {
                case "list":
                    Console.WriteLine("Lista svih ocija: mkdir create put get ls cp mv rename echo cat rm stat df");
                    Console.WriteLine("Za detaljno uputstvo unesite \"help naziv_opcije\"");
                    Console.WriteLine("Za izlaz iz programa unesite \"exit\"\n");
                    break;

                case "df":
                    Console.WriteLine("Prikazuje slobodan prostor na fajl sistemu, prilikom poziva potrebno je navesti naziv_komande. Za detaljniji prikaz trenutnog stanja fajl sistema potrebno je navesti \"df -h\"\n");
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
            writer.Write(numberOfDirectories);
            writer.Write('~');
            writer.Write(numberOfFiles);
            writer.Write('~');
            writer.Write(initialUsedSpace);
            writer.Write('~');
            writer.Write(MAX_SIZE_OF_FILE_SYSTEM - initialUsedSpace);
            writer.Write('~');
            writer.Write(MAX_SIZE_OF_FILE_SYSTEM);
            writer.Write('\n');
            writer.Close();
        }
    }
}

//string path = Directory.GetCurrentDirectory(); //putanja do bin foldera projekta