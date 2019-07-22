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

        //public static List<string> contentOfRootFolder = new List<string>();

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

            while(true)
            {
                Console.Write("HOME>");

                string line = Console.ReadLine();
                string[] words = line.Split(' ');

            
                switch (words[0])
                {
                    case "exit":
                        Environment.Exit(EXIT);
                        break;

                    case "ls": //VALJDA JE DOBRO
                        {
                            if (words.Count() == 1)
                            {
                                GetContentOfDirectory();
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help ls\" za pomoc\n");
                            }
                            break;
                        }

                    case "cd": //SAD JE VALJDA DOBRO
                        {
                            if (words.Count() == 2 && words[1] != "..")
                            {
                                OpenDirectory(words[1]);
                            }
                            else if (words.Count() == 2 && words[1] == "..")
                            {
                                Console.WriteLine("Greska - Netacan unos! Trenutno ste pozicionirani u root direktorijumu fajl sistema\n");
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help cd\" za pomoc\n");
                            }
                            break;
                        }

                    case "help": //SAD JE VALJDA DOBRO
                        {
                            if (words.Count() == 1)
                            {
                                Help("help");
                            }
                            else if (words.Count() == 2)
                            {
                                Help(words[1]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"\n");
                            }
                            break;
                        }

                    case "df": //SAD JE VALJDA DOBRO
                        {
                            if (words.Count() == 1)
                            {
                                GetRootInformation(1);
                            }
                            else if (words.Count() == 2 && words[1] == "-h")
                            {
                                GetRootInformation();
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help df\" za pomoc\n");
                            }
                            break;
                        }

                    case "mkdir": //NEDOVRSENO
                        {
                            if(words.Count() == 2)
                            {
                                MakeDirectory(words[1]);
                            }
                            else if(words.Count() == 3)
                            {
                                //TODO: TREBA DODATI POZIV ZA KREIRANJE NA ZADATOJ PUTANJI
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help mkdir\" za pomoc\n");
                            }
                            break;
                        }

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
                        Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"\n");
                        break;
                }
            }
        }

        private void GetContentOfDirectory(string depth = "1", string path = "root/")
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string _ = reader.ReadLine(); //preskacem prvu liniju

            Console.WriteLine("Tip - naziv - putanja - datum kreiranja - datum posljednje modifikacije - datum posljednjeg otvaranja");
                
            Regex regex = new Regex(@"[A-Za-z0-9/ :.-]+");
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                MatchCollection matches = regex.Matches(line);
                if (depth == "1" && matches[4].Value == depth)  //iscitava one cija je dubina 1 jer se oni nalaze unutar root-a - to znaci da je komanda ls pozvana iz root-a
                {
                    Console.WriteLine(matches[0].Value + " - " + matches[2].Value + " - " + matches[3].Value + " - " + matches[5].Value + " - " + matches[6].Value + " - " + matches[7].Value);
                }
                else if( depth == "2" && matches[3].Value.Contains(path))
                {
                    Console.WriteLine(matches[0].Value + " - " + matches[2].Value + " - " + matches[3].Value + " - " + matches[5].Value + " - " + matches[6].Value + " - " + matches[7].Value);
                }
                line = reader.ReadLine();
            }

            reader.Close();
        }

        private void OpenDirectory(string name, string path = "root/", string depth = "1") //path je ovdje putanja sa koje se poziva funkcija cd
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            _ = reader.ReadLine();
            Regex regex2 = new Regex(@"[A-Za-z0-9/ :.-]+");
            bool flag = false;

            string line = reader.ReadLine();
            while(!line.Contains(separator)) //Prode kroz MFT(zapisi o datotekama i direktorijumima) i pretrazi da li postoji dati folder
            {
                MatchCollection matches = regex2.Matches(line);
                if (matches[0].Value == "dir" && matches[2].Value == name && matches[3].Value == (path + name) && matches[4].Value == "1")
                {
                    flag = true;
                    break;
                }
                line = reader.ReadLine();
            }
            reader.Close();

            if (!flag)
            {
                Console.WriteLine("Ne postoji direktorijum sa datim nazivom");
                return;
            }
            else
            {
                //TODO: ISPRAVITI WHILE PELJU
                while(true)
                {
                    Console.Write("HOME/" + name + ">");

                    string inputLine = Console.ReadLine();
                    string[] words = inputLine.Split(' ');

                    switch (words[0])
                    {
                        case "exit":
                            Environment.Exit(EXIT);
                            break;

                        case "ls":
                            {
                                if (words.Count() == 1)
                                {
                                    GetContentOfDirectory("2", path + name + "/"); //trebaju mi fajlovi/direktorijumi koji su na dubini 2 i cija je putanja pocinje sa "root/otvoreni_direktorijum/"
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help ls\" za pomoc\n");
                                }
                                break;
                            }

                        case "cd":
                            {
                                if (words.Count() == 2 && words[1] != "..")
                                {
                                    OpenDirectory(words[1]); //????????????????????????????????????????????????
                                }
                                else if (words.Count() == 2 && words[1] == "..")
                                {
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help cd\" za pomoc\n");
                                }
                                break;
                            }

                        case "help": // trebalo bi da su uslovi konacno dobri
                            {
                                if (words.Count() == 1)
                                {
                                    Help("help");
                                }
                                else if(words.Count() == 2)
                                {
                                    Help(words[1]);
                                }
                                else
                                {
                                    Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"\n");
                                }
                                break;
                            }

                        case "df": //VALJDA JE DOBRO
                            {
                                if (words.Count() == 1)
                                {
                                    GetRootInformation(1);
                                }
                                else if (words.Count() == 2 && words[1] == "-h")
                                {
                                    GetRootInformation();
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help df\" za pomoc\n");
                                }
                                break;
                            }

                        case "mkdir": //DOBRO, ALI NEDOVRSENO
                            {
                                if (words.Count() == 2)
                                {
                                    MakeDirectory(words[1], path + name + "/", "2");
                                }
                                else if (words.Count() == 3)
                                {
                                    //TODO: TREBA DODATI POZIV ZA KREIRANJE NA ZADATOJ PUTANJI
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help mkdir\" za pomoc\n");
                                }
                                break;
                            }

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
                            Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"\n");
                            break;
                    }
                }
            }
        }

        private void LoadData()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            reader.Close();

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

            //Regex regex2 = new Regex(@"[A-Za-z0-9.-]+");
            //string line = reader.ReadLine();
            //while (!line.Contains(separator))
            //{
            //    MatchCollection matches = regex2.Matches(line);
            //    if (matches[4].Value == "1") //Ako je dubina jednako 1, tj. ako se dati folder nalazi u root folderu onda se doda njegov naziv u listu sadrzaja root foldera
            //    {
            //        contentOfRootFolder.Add(matches[2].Value);
            //    }
            //    line = reader.ReadLine();
            //}
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
                case "help":
                    Console.WriteLine("Lista svih ocija: mkdir create put get ls cp mv rename echo cat rm stat df");
                    Console.WriteLine("Za detaljno uputstvo unesite \"help naziv_opcije\"");
                    Console.WriteLine("Za izlaz iz programa unesite \"exit\"\n");
                    break;

                case "cd":
                    Console.WriteLine("Omogucava pozicioniranje u zadati poddirektorijum, prilikom poziva potrebno je navesti naziv_komande naziv_poddirektorijuma");
                    Console.WriteLine("Za vracanje u roditeljski direktorijum unesite \"cd ..\"");
                    break;

                case "df":
                    Console.WriteLine("Prikazuje slobodan prostor na fajl sistemu, prilikom poziva potrebno je navesti naziv_komande. Za detaljniji prikaz trenutnog stanja fajl sistema potrebno je navesti \"df -h\"\n");
                    break;

                case "mkdir":
                    Console.WriteLine("Kreira novi direktorijum na zadatoj putanji, prilikom poziva potrebno je navesti naziv_komande naziv_direktorijuma putanja. Putanja je opcioni parametar, ukoliko se ne navede podrazumijeva se trenutna putanja\n");
                    break;

                case "create":
                    Console.WriteLine("Kreira novu datoteku na zadatoj putanji, prilikom poziva potrebno je navesti naziv_komande naziv_datoteke putanja.  Putanja je opcioni parametar, ukoliko se ne navede podrazumijeva se trenutna putanja\n");
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
                    Console.WriteLine("Greska - nepostojeca opcija! Za listu svih opcija unesite \"help\"\n");
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

        //Trebalo bi da sada mkdir savrseno radi
        private void MakeDirectory(string name, string path = "root/", string depth = "1") //path je putanja na kojoj treba da se nalazi novi folder
        {
            name = CheckName(name, path);

            Directory directory = new Directory(name, getNewId(), DateTime.Today, DateTime.Today, DateTime.Today, path, depth);
            directory.WriteToFile();
            numberOfDirectories++;
            Update();
        }

        private string CheckName(string name, string path)
        {
            while(true)
            {
                Match match = (new Regex(@"[A-Za-z0-9.-]+")).Match(name);
                bool isDuplicate = IsDuplicate(name, path); 

                if (name.Length <= 20 && !isDuplicate && match.Value == name)
                {
                    return name;
                }
                else if(name.Length > 20)
                {
                    Console.WriteLine("Greska - naziv je predugacak. Unesite novi naziv");
                    name = Console.ReadLine();
                }
                else if(match.Value != name)
                {
                    Console.WriteLine("Naziv moze sadrzati velika i mala slova, cifre, . i -");
                    Console.WriteLine("Unesite novi naziv direktorijuma");
                    name = Console.ReadLine();
                }
                else if(isDuplicate)
                {
                    Console.WriteLine("Greska - vec postoji datoteka ili direktorijum sa datim nazivom");
                    Console.WriteLine("Unesite novi naziv direktorijuma");
                    name = Console.ReadLine();
                }
            }
        }

        private bool IsDuplicate(string name, string path)
        {
            bool flag = false;
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                if(line.Contains("~" + path + name + "~"))
                {
                    flag = true;
                    break;
                }
                line = reader.ReadLine();
            }
            reader.Close();
            return flag;
        } //provjerava da li postoji fajl/folder sa istim nazivom na zadatoj putanji

        private void Update()
        {
            //procita sve podatke o datotekama i direktorijumima u root-u i upise u listu - sadrzaj root direktorijuma
            //StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            //_ = reader.ReadLine();
            //Regex regex2 = new Regex(@"[A-Za-z0-9\/:.-]+");
            //string line = reader.ReadLine();
            //while (!line.Contains(separator))
            //{
            //    MatchCollection matches = regex2.Matches(line);
            //    if (matches[4].Value == "1") //Ako je dubina jednako 1, tj. ako se dati folder nalazi u root folderu onda se doda njegov naziv u listu sadrzaja root foldera
            //    {
            //        if(!contentOfRootFolder.Contains(matches[2].Value)) //dodaje samo novi direktorijum, bez uslova bi dodavao sav sadrzaj svaki put kad se pozove update pa bi bilo dupliranja podataka
            //            contentOfRootFolder.Add(matches[2].Value);
            //        
            //    }
            //    line = reader.ReadLine();
            //}
            //reader.Close();

            StreamReader reader1 = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader1.ReadLine();
            string content = reader1.ReadToEnd();
            reader1.Close();

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