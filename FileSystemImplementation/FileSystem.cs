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
        public static readonly int initialUsedSpace = 50; //prilikom kreiranja fajl sistema velicina je 50 bajta zbog upisivanja inicijalnih vrijednosti o fajl sistemu
        public static int numberOfFiles = 0; //ukupan broj fajlova na fajl sistemu
        public static int numberOfDirectories = 0; //ukupan broj direktorijuma na fajl sistemu
        public static int usedSpace;
        public static int freeSpace;
        public static readonly int EXIT = 0;
        public static readonly string separator = "~~~DATA--SECTION~~~";

        public static List<string> contentOfRootFolder = new List<string>();

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
            else
            {
                LoadData();
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

                    case "ls":
                        GetContent();
                        break;

                    case "cd": //dodatna komanda
                        try
                        {
                            OpenDirectory(words[1]);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Greska - Netacan unos! Unesite \"help cd\" za pomoc\n");
                        }
                        break;

                    case "help": //dodatna komanda
                        try
                        {
                            Help(words[1]);
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Za pomoc pri radu unesite \"help list\"\n");
                        }
                        break;

                    case "df": //dodatna komanda
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
                            MakeDirectory(words[1]);
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

        private void GetContent(string path = "root/")
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string _ = reader.ReadLine(); //preskacem prvu liniju

            Regex regex = new Regex(@"[A-Za-z0-9.-]+");
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                MatchCollection matches = regex.Matches(line);
                if (matches[4].Value == "1") //Ako je dubina jednako 1, tj. ako se dati folder nalazi u root folderu onda se doda njegov naziv u listu sadrzaja root foldera
                {
                    contentOfRootFolder.Add(matches[2].Value);
                }
                line = reader.ReadLine();
            }


            reader.Close();
        }

        private void OpenDirectory(string name, string path = "root/")
        {
            throw new NotImplementedException();
        }

        private void LoadData()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();

            Regex regex = new Regex(@"HOME~([0-9]+)~([0-9]+)~([0-9]+)~([0-9]+)");
            Match match = regex.Match(firstLine);

            string _numberOfDirectories = match.Groups[1].Value;
            numberOfDirectories = Int32.Parse(_numberOfDirectories);

            string _numberOfFiles = match.Groups[2].Value;
            numberOfFiles = Int32.Parse(_numberOfFiles);

            string _usedSpace = match.Groups[3].Value;
            usedSpace = Int32.Parse(_usedSpace);

            string _freeSpace = match.Groups[4].Value;
            freeSpace = Int32.Parse(_freeSpace);

            Regex regex2 = new Regex(@"[A-Za-z0-9.-]+");
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                MatchCollection matches = regex2.Matches(line);
                if (matches[4].Value == "1") //Ako je dubina jednako 1, tj. ako se dati folder nalazi u root folderu onda se doda njegov naziv u listu sadrzaja root foldera
                {
                    contentOfRootFolder.Add(matches[2].Value);
                }
                line = reader.ReadLine();
            }
            reader.Close();
        }

        private void CreateFileSystem()
        {
            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Create));
            writer.Write("HOME~" + numberOfDirectories + "~" + numberOfFiles + "~");
            writer.WriteLine(initialUsedSpace + "~" + (MAX_SIZE_OF_FILE_SYSTEM - initialUsedSpace) + "~" + MAX_SIZE_OF_FILE_SYSTEM);
            writer.Write("~~~DATA--SECTION~~~");
            writer.Close();
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

                case "cd":
                    Console.WriteLine("Omogucava pozicioniranje u zadati poddirektorijum, prilikom poziva potrebno je navesti naziv_komande naziv_poddirektorijuma");
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

        private void MakeDirectory(string name, string _ = "root/")
        {
            name = CheckName(name);

            Directory directory = new Directory(name, getNewId(), DateTime.Today, DateTime.Today, DateTime.Today);
            directory.WriteToFile();
            numberOfDirectories++;
            Update();
        }

        private string CheckName(string name)
        {
            while(true)
            {
                bool flag = false; // ovaj flag sam dodala jer kad se unese naziv koji vec postoji program ucita novi naziv i prelazi u drugi else if uslov zbog neslaganja matcha i naziva
                Match match = (new Regex(@"[A-Za-z0-9.-]+")).Match(name); //naziv moze sadrzati samo slova, brojeve, . i -

                if (match.Value == name && !contentOfRootFolder.Contains(name))
                {
                    return name;
                }
                else if (match.Value == name && contentOfRootFolder.Contains(name) && !flag)
                {
                    Console.WriteLine("Greska - vec postoji datoteka ili direktorijum sa datim nazivom");
                    Console.WriteLine("Unesite novi naziv direktorijuma");
                    name = Console.ReadLine();
                    flag = true;
                }
                else if (match.Value != name && !flag)
                {
                    Console.WriteLine("Naziv moze sadrzati velika i mala slova, cifre, . i -");
                    Console.WriteLine("Unesite novi naziv direktorijuma");
                    name = Console.ReadLine();
                }
            }
        }

        private void Update()
        {
            StreamReader reader1 = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader1.ReadLine();
            string content = reader1.ReadToEnd();
            reader1.Close();


            //procita sve podatke o datotekama i direktorijumima u root-u i upise u listu - sadrzaj root direktorijuma
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            _ = reader.ReadLine();
            Regex regex2 = new Regex(@"[A-Za-z0-9\/:.-]+");
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                MatchCollection matches = regex2.Matches(line);
                if (matches[4].Value == "1") //Ako je dubina jednako 1, tj. ako se dati folder nalazi u root folderu onda se doda njegov naziv u listu sadrzaja root foldera
                {
                    if(!contentOfRootFolder.Contains(matches[2].Value)) //dodaje samo novi direktorijum, bez uslova bi dodavao sav sadrzaj svaki put kad se pozove update pa bi bilo dupliranja podataka
                        contentOfRootFolder.Add(matches[2].Value);
                    
                }
                line = reader.ReadLine();
            }
            reader.Close();


            usedSpace = firstLine.Length + content.Length + 4; // 4 dodajem zbog razlike u stvarnoj velicini fajla i duzinama ovih stringova, a razlika postoji zbog broja bita rezervisanih za newline
            freeSpace = MAX_SIZE_OF_FILE_SYSTEM - usedSpace;


            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.Write("HOME~" + numberOfDirectories + "~" + numberOfFiles + "~");
            writer.WriteLine(usedSpace + "~" + (MAX_SIZE_OF_FILE_SYSTEM - usedSpace) + "~" + MAX_SIZE_OF_FILE_SYSTEM);
            writer.WriteLine(content);
            writer.Close();
        }

        private int getNewId()
        {
            StreamReader reader2 = new StreamReader(new FileStream("ListOfIdentificators.txt", FileMode.OpenOrCreate));
            string content = reader2.ReadToEnd();
            reader2.Close();

            StreamReader reader = new StreamReader(new FileStream("ListOfIdentificators.txt", FileMode.OpenOrCreate));
            string _lastId = reader.ReadLine();
            reader.Close();

            int lastId;
            if(_lastId != null)
            {
                lastId = Int32.Parse(_lastId);
            }
            else
            {
                lastId = 0;
            }

            int newId = lastId + 1;

            StreamWriter writer = new StreamWriter(new FileStream("ListOfIdentificators.txt", FileMode.OpenOrCreate));
            writer.WriteLine(newId + "\r\n" + content);
            writer.Close();

            return newId;
        } 
    }
}

//string path = Directory.GetCurrentDirectory(); //putanja do bin foldera projekta