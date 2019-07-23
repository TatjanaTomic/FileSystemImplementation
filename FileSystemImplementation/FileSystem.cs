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
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help ls\" za pomoc");
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
                                Console.WriteLine("Greska! Trenutno ste pozicionirani u root direktorijumu fajl sistema");
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help cd\" za pomoc");
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
                                Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"");
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
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help df\" za pomoc");
                            }
                            break;
                        }

                    case "mkdir": //NEDOVRSENO 
                        {
                            if(words.Count() == 2)
                            {
                                MakeDirectory(words[1]); //Znaci ako odavde pozivam mkdir to znaci da se unutar root-a kreira direktorijum sa nazivom words[1] i on ce biti na dubini 1
                            }
                            else if(words.Count() == 3 && words[2] == "root/")
                            {
                                MakeDirectory(words[1]);
                            }
                            else if(words.Count() == 3 && words[3] != "root/")
                            {
                                Console.WriteLine("Greska - nije moguce kreirati direktorijum na zadatoj putanji");
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help mkdir\" za pomoc");
                            }
                            break;
                        }

                    case "create":
                        break;

                    case "rename":
                        {
                            if(words.Count() == 3)
                            {
                                Rename(words[1], words[2]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help rename\" za pomoc");
                            }
                        break;
                        }

                    case "put":
                        break;

                    case "get":
                        break;

                    case "cp":
                        break;

                    case "mv":
                        {
                            if(words.Count() == 3)
                            {
                                MoveFile(words[1], words[2]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help mv\" za pomoc");
                            }
                            break;
                        }

                    case "echo":
                        break;

                    case "cat":
                        break;

                    case "rm":
                        break;

                    case "stat":
                        break;

                    default:
                        Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"");
                        break;
                }
            }
        }

        //TREBA TESTIRATI I DEBAGOVATI
        private void MoveFile(string name, string newPath, string path = "root/") //Ako se zeli premjestiti datoteka koja je na putanji root/folder/datoteka, funkcija ce se pozvati sa putanje root/folder
        {
            if (!Exists(name, path)) //Ako neko pokusa premjestiti datoteku koja ne postoji na trenutnoj putanji
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            if(newPath != "root/") //Ako se premjesta na neku putanju poput root/naziv_foldera
            {
                Regex regex = new Regex(@"(root/)([A-Za-z0-9.-]+)"); //Provjera da li je validan zapis putanje, tj. da li je putanja unesena u obliku root/naziv_foldera
                Match match = regex.Match(newPath);

                if (match.Value != newPath || !Exists(match.Groups[1].Value, match.Groups[0].Value)) //Ili je putanja unesena u nepravilnom obliku ili ne postoji taj folder unutar root-a
                {
                    Console.WriteLine("Greska - unesena je nepostojeca putanja");
                    return;
                }
            }

            //Provjera da li na novoj putanji vec postoji datoteka sa istim nazivom
            if(Exists(name, newPath))
            {
                Console.WriteLine("Greska - na zadatoj putanji vec postoji datoteka sa nazivom {0}", name);
                return;
            }


            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            Queue<string> content1 = new Queue<string>();
            string line;

            do
            {
                line = reader.ReadLine();
                if (line.Contains("~" + path + name + "~"))
                {
                    line = line.Replace(path, newPath);
                    break;
                }
                else
                {
                    content1.Enqueue(line);
                }

            }
            while (!line.Contains(separator));

            string content2 = reader.ReadToEnd();
            reader.Close();

            string content = "";
            while (content1.Count != 0)
            {
                content += content1.Dequeue() + "\r\n";
            }
            content += line + "\r\n";
            content += content2;

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            writer.Write(content);
            writer.Close();

            Update();
        }

        private void Rename(string name, string newName, string path ="root/")
        {
            if(!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji direktorijum/datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            newName = CheckName(newName, path);

            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            Queue<string> content1 = new Queue<string>();
            string line;

            do
            {
                line = reader.ReadLine();
                if(line.Contains("~" + path + name + "~"))
                {
                    line = line.Replace(name, newName);
                    break;
                }
                else
                {
                    content1.Enqueue(line);
                }

            }
            while (!line.Contains(separator));

            string content2 = reader.ReadToEnd();
            reader.Close();

            string content = "";
            while (content1.Count != 0)
            {
                content += content1.Dequeue() + "\r\n";
            }
            content += line + "\r\n";
            content += content2;

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            writer.Write(content);
            writer.Close();

            Update();
        }

        private void GetContentOfDirectory(string path = "root/")
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string _ = reader.ReadLine(); //preskacem prvu liniju

            Console.WriteLine("Tip - naziv - putanja - datum kreiranja");
                
            Regex regex = new Regex(@"[A-Za-z0-9/ :.-]+");
            string line = reader.ReadLine();
            while(!line.Contains(separator))
            {
                MatchCollection matches = regex.Matches(line);
                if(matches[0].Value == "dir" && matches[3].Value.Contains(path))
                {
                    Console.WriteLine(matches[0].Value + " - " + matches[2].Value + " - " + matches[3].Value + " - " + matches[4].Value);
                }
                line = reader.ReadLine();
            }
            
            reader.Close();
        }

        private void OpenDirectory(string name, string path = "root/") //path je ovdje putanja sa koje se poziva funkcija cd
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            _ = reader.ReadLine();
            Regex regex2 = new Regex(@"[A-Za-z0-9/ :.-]+");
            bool flag = false;

            string line = reader.ReadLine();
            while(!line.Contains(separator)) //Prodje kroz MFT(zapisi o datotekama i direktorijumima) i pretrazi da li postoji dati folder
            {
                MatchCollection matches = regex2.Matches(line);
                if (matches[0].Value == "dir" && matches[2].Value == name && matches[3].Value == (path + name))
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
                    Console.Write(path.Replace("root", "HOME") + name + ">");

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
                                    GetContentOfDirectory(path + name + "/"); //trebaju mi fajlovi/direktorijumi koji su na dubini 2 i cija je putanja pocinje sa "root/otvoreni_direktorijum/"
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help ls\" za pomoc");
                                }
                                break;
                            }

                        case "cd":
                            {
                                if (words.Count() == 2 && words[1] != "..")
                                {
                                    OpenDirectory(words[1], path + name + "/"); //Ako iz open directory pozivam opet OpenDirectory() znači da sa putanje path+name otvaram folder sa nazivom words[1] koji se nalazi na dubini 2
                                }
                                else if (words.Count() == 2 && words[1] == "..")
                                {
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help cd\" za pomoc");
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
                                    Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"");
                                }
                                break;
                            }

                        case "df": //VALJDA JE DOBRO
                            {
                                if (words.Count() == 1) //df mi je samo za slobodan memorijski prostor
                                {
                                    GetRootInformation(1); //jedinicu saljem kao neki flag da bi mi ispisalo samo slobodan mem. prostor
                                }
                                else if (words.Count() == 2 && words[1] == "-h") // df -h je za ispis svih podataka o fajl sistemu
                                {
                                    GetRootInformation();
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help df\" za pomoc");
                                }
                                break;
                            }

                        case "mkdir":
                            {
                                if (words.Count() == 2) //Uneseno je mkdir naziv_direktorijuma unutar nekog otvorenog foldera
                                {
                                    Console.WriteLine("Greska - nije moguce kreirati direktorijum na ovoj putanji. Pozicionirajte se u root folder ili unesite putanju \"root/\"");
                                }
                                else if(words.Count() == 3 && words[2] == "root/")
                                {
                                    MakeDirectory(words[1]);
                                }
                                else if(words.Count() == 3 && words[2] != "root/")
                                {
                                    Console.WriteLine("Greska - nije moguce kreirati direktorijum na zadatoj putanji");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help mkdir\" za pomoc");
                                }
                                break;
                            }

                        case "create":
                            break;

                        case "rename":
                            {
                                if (words.Count() == 3)
                                {
                                    Rename(words[1], words[2]);
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help rename\" za pomoc");
                                }
                                break;
                            }

                        case "put":
                            break;

                        case "get":
                            break;

                        case "cp":
                            break;

                        case "mv":
                            {
                                if (words.Count() == 3)
                                {
                                    MoveFile(words[1], words[2]);
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help mv\" za pomoc");
                                }
                                break;
                            }

                        case "echo":
                            break;

                        case "cat":
                            break;

                        case "rm":
                            break;

                        case "stat":
                            break;

                        default:
                            Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"");
                            break;
                    }
                }
            }
        }

        //NE DIRAJ NISTA - RADI
        private void LoadData()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            reader.Close();

            Regex regex = new Regex(@"[0-9]+");
            MatchCollection matches = regex.Matches(firstLine);

            string _numberOfDirectories = matches[0].Value;
            numberOfDirectories = Int32.Parse(_numberOfDirectories);

            string _numberOfFiles = matches[1].Value;
            numberOfFiles = Int32.Parse(_numberOfFiles);

            string _usedSpace = matches[2].Value;
            usedSpace = Int32.Parse(_usedSpace);

            string _freeSpace = matches[4].Value;
            freeSpace = Int32.Parse(_freeSpace);

        }

        //RADI I OVO
        private void CreateFileSystem()
        {
            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Create));
            writer.Write("HOME~" + numberOfDirectories + "~" + numberOfFiles + "~");
            writer.WriteLine(initialUsedSpace + "~" + (MAX_SIZE_OF_FILE_SYSTEM - initialUsedSpace) + "~" + MAX_SIZE_OF_FILE_SYSTEM);
            writer.Write(separator);
            writer.Close();
        }
        
        //RADI
        private void Help(string method)
        {
            switch(method)
            {
                case "help":
                    Console.WriteLine("Lista svih ocija: mkdir create put get ls cp mv rename echo cat rm stat df");
                    Console.WriteLine("Za detaljno uputstvo unesite \"help naziv_opcije\"");
                    Console.WriteLine("Za izlaz iz programa unesite \"exit\"");
                    break;

                case "cd":
                    Console.WriteLine("Omogucava pozicioniranje u zadati direktorijum, prilikom poziva potrebno je navesti naziv_komande naziv_direktorijuma");
                    Console.WriteLine("Za vracanje u roditeljski direktorijum unesite \"cd ..\"");
                    break;

                case "df":
                    Console.WriteLine("Prikazuje slobodan prostor na fajl sistemu, prilikom poziva potrebno je navesti naziv_komande. Za detaljniji prikaz trenutnog stanja fajl sistema potrebno je navesti \"df -h\"");
                    break;

                case "mkdir":
                    Console.WriteLine("Kreira novi direktorijum na trenutnoj putanji, prilikom poziva potrebno je navesti naziv_komande naziv_direktorijuma putanja. Putanja je opcioni parametar");
                    break;

                case "create":
                    Console.WriteLine("Kreira novu datoteku na zadatoj putanji, prilikom poziva potrebno je navesti naziv_komande naziv_datoteke putanja.  Putanja je opcioni parametar, ukoliko se ne navede podrazumijeva se trenutna putanja");
                    break;

                case "put":
                    Console.WriteLine("Omogucava dodavanje datoteke sa fajl sistema racunara na novi fajl sistem, prilikom poziva potrebno je navesti naziv_komande izvorna_putanja krajnja_putanja");
                    break;

                case "get":
                    Console.WriteLine("Omogucava dodavanje datoteke sa novog fajl sistema na fajl sistema racunara, prilikom poziva potrebno je navesti naziv_komande izvorna_putanja krajnja_putanja");
                    break;

                case "ls":
                    Console.WriteLine("Ispisuje sadrzaj direktorijuma na trenutnoj putanji");
                    break;

                case "cp":
                    Console.WriteLine("Kreira kopiju datoteke na trenutnoj putanji");
                    break;

                case "mv":
                    Console.WriteLine("Omogucava premjestanje datoteke sa trenutne putanje na zadatu lokaciju, prilikom poziva potrebno je navesti naziv_komande naziv_fajla krajnja_putanja");
                    break;

                case "rename":
                    Console.WriteLine("Omogucava promjenu naziva datoteke ili diretorijuma na trenutnoj putanji. U slucaju promjene naziva datoteke potrebno je navesti naziv_komande trenutni_naziv_datoteke novi_naziv_datoteke. U slucaju promjene naziva direktorijuma potrebno je navesti naziv_komande novi_naziv_direktorijuma");
                    break;

                case "echo":
                    Console.WriteLine("Omogucava upis tekstualnog sadrzaja u datoteku, prilikom poziva poptreno je navesti naziv_komande naziv_datoteke tekstualni_sadrzaj");
                    break;

                case "cat":
                    Console.WriteLine("Omogucava prikaz sadrzaja tekstualne datoteke");
                    break;

                case "rm":
                    Console.WriteLine("Omogucava brisanje datoteke ili direktorijuma sa trenutne putanje. U slucaju brisanja kompletnog brisanja sadrzaja direktorijuma potrebno je navesti \"rm - r\". Prilikom poziva potrebno je navesti naziv_komande naziv_dat/dir");
                    break;

                case "stat":
                    Console.WriteLine("Omogucava ispis podataka o datoteci, prilikom poziva potrebno je navesti naziv_komande naziv_datoteke");
                    break;

                default:
                    Console.WriteLine("Greska - nepostojeca opcija! Za listu svih opcija unesite \"help\"");
                    break;


            }
            
        }

        //TREBA TESTIRATI
        private void GetRootInformation(int a = 0)
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();

            Regex regex = new Regex(@"[0-9]+");
            MatchCollection matches = regex.Matches(line);

            if (a == 1)
            {
                Console.WriteLine("Slobodan memorijski prostor: " + matches[3] + "b");
            }
            else
            {
                Console.WriteLine("Broj direktorijuma: " + matches[0]);
                Console.WriteLine("Broj datoteka: " + matches[1]);
                Console.WriteLine("Iskoriscen memorijski prostor: " + matches[2] + "b");
                Console.WriteLine("Slobodan memorijski prostor: " + matches[3] + "b");
                Console.WriteLine("Ukupna velicina fajl sistema: " + matches[4] + "b");
            }

            reader.Close();
        }

        private void MakeDirectory(string name, string path = "root/")
        {
            name = CheckName(name, path);

            Directory directory = new Directory(name, GetNewId(), DateTime.Now, path);
            directory.WriteToFile();
            numberOfDirectories++;
            Update();
        }

        private string CheckName(string name, string path)
        {
            while(true)
            {
                Match match = (new Regex(@"[A-Za-z0-9.-]+")).Match(name);
                bool exists = Exists(name, path); 

                if (name.Length <= 20 && !exists && match.Value == name)
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
                else if(exists)
                {
                    Console.WriteLine("Greska - vec postoji datoteka ili direktorijum sa datim nazivom");
                    Console.WriteLine("Unesite novi naziv direktorijuma");
                    name = Console.ReadLine();
                }
            }
        }

        private bool Exists(string name, string path)
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
            StreamReader reader1 = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader1.ReadLine();
            string content = reader1.ReadToEnd();
            reader1.Close();

            usedSpace = firstLine.Length + content.Length + 4; // 4 dodajem zbog razlike u stvarnoj velicini fajla i duzinama ovih stringova, a razlika postoji zbog broja bita rezervisanih za newline
            freeSpace = MAX_SIZE_OF_FILE_SYSTEM - usedSpace;

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.Write("HOME~" + numberOfDirectories + "~" + numberOfFiles + "~");
            writer.WriteLine(usedSpace + "~" + (MAX_SIZE_OF_FILE_SYSTEM - usedSpace) + "~" + MAX_SIZE_OF_FILE_SYSTEM);
            writer.Write(content);
            writer.Close();
        }

        //NE ČAČKAJ NIŠTA - RADI
        private int GetNewId()
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
            writer.WriteLine(newId);
            writer.WriteLine(content);
            writer.Close();

            return newId;
        } 
    }
}

//string path = Directory.GetCurrentDirectory(); //putanja do bin foldera projekta