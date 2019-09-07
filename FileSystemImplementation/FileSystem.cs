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
        public static readonly int SIZE_OF_BLOCK = 512; //minimalna velicina jednog bloka za cuvanje sadrzaja datoteke
        public static readonly int initialUsedSpace = 50; //prilikom kreiranja fajl sistema velicina je 50 bajta zbog upisivanja inicijalnih vrijednosti o fajl sistemu
        public static int numberOfFiles = 0; //ukupan broj fajlova na fajl sistemu
        public static int numberOfDirectories = 0; //ukupan broj direktorijuma na fajl sistemu
        public static int usedSpace;
        public static int freeSpace;
        public static readonly int EXIT = 0;
        public static readonly string separator = "~~~DATA-SEGMENT~~~";

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

            Console.WriteLine("Pozdrav! Za pomoc pri radu unesite \"help\" :)");

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

                    case "clr":
                        Console.Clear();
                        break;

                    case "ls": //RADI - NE DIRAJ ! - ls radi tako da ispise sadrzaj direktorijuma u kom sam trenutno pozicionirana
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

                    case "cd": //RADI - NE DIRAJ ! - cd je komanda za otvaranje nekog direktorijuma
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

                    case "help": //RADI - NE DIRAJ !
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

                    case "df": //RADI - NE DIRAJ ! - df ispisuje trenutno stanje memorije
                        {
                            if (words.Count() == 1)
                            {
                                GetRootInformation(false);
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

                    case "mkdir": //RADI - NE DIRAJ ! - mkdir pozivam samo sa nazivom foldera, ne unosim putanju jer se podrazumijevano kreiraju unutar root-a
                        {
                            if(words.Count() == 2)
                            {
                                MakeDirectory(words[1]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help mkdir\" za pomoc");
                            }
                            break;
                        }

                    case "create": //RADIIII - create moze da se pozove samo sa nazivom datoteke, tada se podrazumijeva da je datoteka na putanji na kojoj se trenutno nalazimo ili da se zada putanja na kojoj bi trebalo da se nalazi ta datoteka
                        {
                            if (words.Count() == 2)
                            {
                                CreateFile(words[1]);
                            }
                            else if (words.Count() == 3)
                            {
                                CreateFile(words[1], words[2]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help create\" za pomoc");
                            }
                            break;
                        }

                    case "rename": //RADIIIII - poziva se sa putanje na kojoj se trenutno nalazimo
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
                        
                    case "mv": //TO JE TO - NE DIRAJ NISTA - sa trenutne putanje premjesta na unesenu putanju
                        {
                            if(words.Count() == 3)
                            {
                                MoveFile(words[1], words[2]); //words[1] je naziv fajla, a words[2] krajnja putanja
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help mv\" za pomoc");
                            }
                            break;
                        }

                    case "echo": //Dodaj echo u help!!! - echo se poziva sa trenutne putanje
                        {
                            if (words.Count() == 2)
                            {
                                WriteToFile(words[1]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help echo\" za pomoc");
                            }
                            break;
                        }

                    case "cat":
                        {
                            if (words.Count() == 2)
                            {
                                GetContentOfFile(words[1]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help cat\" za pomoc");
                            }
                            break;
                        }

                    case "put":
                        {
                            if (words.Count() == 1)
                            {
                                PutFile();
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help put\" za pomoc");
                            }
                            break;
                        }

                    case "get":
                        {
                            if (words.Count() == 1)
                            {
                                GetFile();
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help get\" za pomoc");
                            }
                            break;
                        }

                    case "cp":
                        {
                            if(words.Count() == 2)
                            {
                                CopyFile(words[1]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help copy\" za pomoc");
                            }
                            break;
                        }

                    case "rm":
                        {
                            if (words.Count() == 2)
                            {
                                 RemoveFile(words[1]);
                            }
                            else if(words.Count() == 3 && words[1] == "-r")
                            {
                                 RemoveDirectory(words[2]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help rm\" za pomoc");
                            }
                            break;
                        }

                    case "stat":
                        {
                            if (words.Count() == 2)
                            {
                                GetFileInformation(words[1]);
                            }
                            else
                            {
                                Console.WriteLine("Greska - Netacan unos! Unesite \"help stat\" za pomoc");
                            }
                            break;
                        }

                    default:
                        Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"");
                        break;
                }
            }
        }

        private string[] GetFilesOfDirectory(string name)
        {
            LinkedList<string> files = new LinkedList<string>();

            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                if (line.Contains("~root/" + name + "/"))
                {
                    files.AddLast(line.Split('~')[2]);
                }
                line = reader.ReadLine();
            }
            reader.Close();

            return files.ToArray();
        }
        private void RemoveDirectory(string name, string path = "root/")
        {
            if(name.StartsWith("root/"))
            {
                name = name.Substring(5);
            }

            if (!Exists(name, "root/"))
            {
                Console.WriteLine("Greska - ne postoji direktorijum sa unesenim nazivom ili putanjom.");
                return;
            }

            string[] files = GetFilesOfDirectory(name);
            foreach (var file in files)
                RemoveFile(file, path + name + "/");

            RemoveMftRecord('d', name);

            numberOfDirectories--;

            Update(); Update();

        }
        private void RemoveMftRecord(char type, string name, string path = "root/")
        {
            byte[] contentOfFS = File.ReadAllBytes("FileSystem.bin");

            byte[] _id = GetFileID(name, path);

            int start = 0, end = 0;
            for(int i=0; i<contentOfFS.Length; ++i)
            {
                if (contentOfFS[i] == '~' && contentOfFS[i + 1] == _id[0] && contentOfFS[i + 2] == _id[1] && contentOfFS[i + 3] == _id[2] && contentOfFS[i + 4] == '~')
                {
                    if(type == 'd')
                    {
                        start = i - 4;
                        break;
                    }
                    else if(type == 'f')
                    {
                        start = i - 5;
                        break;
                    }
                }
            }
            for(int i=start+1; i<contentOfFS.Length; ++i)
            {
                if(contentOfFS[i] == (byte)'\n')
                {
                    end = i;
                    break;
                } //znaci trazim prvi znak za novi red
            }

            List<byte> newContentOfFS = new List<byte>();
            for(int i=0; i<start;++i)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }
            for(int i=end; i<contentOfFS.Length;++i)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }

            RewriteFS(newContentOfFS.ToArray());
        }

        private void RewriteFS(byte[] content)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            writer.Write(content);
            writer.Close();
        }

        private void RemoveFile(string name, string path = "root/")
        {
            if (!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putnji.", name);
                return;
            }

            byte[] _id = GetFileID(name, path);

            if (GetSizeOfFile(name, path) != 0)
            {
                DeleteFromDataSegment(_id);
            }

            RemoveMftRecord('f', name, path);

            numberOfFiles--;

            Update(); Update();

        }
        private void GetContentOfFile(string name, string path = "root/")
        {
            if (!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            //Treba dodati provjeru da li je datoteka tekstualna 

            if (GetSizeOfFile(name, path) == 0)
            {
                Console.WriteLine(" ");
            }
            else
            {
                byte[] _id = GetFileID(name, path);

                byte[] contentOfFile = ReadFromDataSegment(_id);

                foreach (var b in contentOfFile)
                    Console.Write((char)b);
                Console.Write('\n');
            }
        }
        private void DeleteFromDataSegment(byte[] _id)
        {
            byte[] _contentOfFS = File.ReadAllBytes("FileSystem.bin");

            int start, end;
            (start, end) = GetStartAndEndPositions(_id);

            LinkedList<byte> newContentOfFS = new LinkedList<byte>();
            for (int i = 0; i < start - 6; ++i)
                newContentOfFS.AddLast(_contentOfFS[i]);
            for (int i = end + 6; i < _contentOfFS.Length; ++i)
                newContentOfFS.AddLast(_contentOfFS[i]);

            byte[] _newContentOfFS = newContentOfFS.ToArray();

            RewriteFS(_newContentOfFS);

            Update(); Update();
        }
        private void WriteToDataSegment(byte[] _id, byte[] content)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Append));
            writer.Write((byte)'~');
            writer.Write(_id);
            writer.Write((byte)'D');
            writer.Write((byte)'~');
            writer.Write(content);
            writer.Write((byte)'~');
            writer.Write((byte)'E');
            writer.Write((byte)'O');
            writer.Write((byte)'F');
            writer.Write((byte)'~');
            writer.Close();

            Update(); Update();
        }
        private byte[] ReadFromDataSegment(byte[] _id)
        {
            byte[] _contentOfFS = File.ReadAllBytes("FileSystem.bin");

            int start, end; //promjenljive koje ce mi u prethodnom nizu oznaciti pocetak i kraj data segmenta trazenog fajla
            (start, end) = GetStartAndEndPositions(_id);

            LinkedList<byte> contentOfFile = new LinkedList<byte>();
            for (int i = start; i <= end; ++i)
            {
                contentOfFile.AddLast(_contentOfFS[i]);
            }

            return contentOfFile.ToArray();
        }
        private (int, int) GetStartAndEndPositions(byte[] _id) //funkcija vracapocetak i kraj sadrzaja datoteke (zapis u data segmentu)
        {
            byte[] _contentOfFS = File.ReadAllBytes("FileSystem.bin");
            int start = 0, end = 0; //promjenljive koje ce mi u prethodnom nizu oznaciti pocetak i kraj data segmenta trazenog fajla
            for (int i = 0; i < _contentOfFS.Length; ++i)
            {
                if (_contentOfFS[i] == '~' && _contentOfFS[i + 1] == _id[0] && _contentOfFS[i + 2] == _id[1] && _contentOfFS[i + 3] == _id[2] && _contentOfFS[i + 4] == 'D' && _contentOfFS[i + 5] == '~')
                {
                    start = i + 6;
                    for (int j = start; j < _contentOfFS.Length; ++j)
                    {
                        if (_contentOfFS[j + 1] == '~' && _contentOfFS[j + 2] == 'E' && _contentOfFS[j + 3] == 'O' && _contentOfFS[j + 4] == 'F' && _contentOfFS[j + 5] == '~')
                        {
                            end = j;
                            break;
                        }
                    }
                    break;
                }
            }
            return (start, end);
        }

        //RADIIIII
        private void WriteToFile(string name, string path = "root/")
        {
            if(!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            Console.WriteLine("Unesite sadrzaj datoteke:");
            string sContentOfFile = Console.ReadLine();
            byte[] _contentOfFile = new byte[sContentOfFile.Length];
            for (int i = 0; i < sContentOfFile.Length; ++i)
                _contentOfFile[i] = (byte)sContentOfFile[i];
            List<byte> contentOfFile = _contentOfFile.ToList();

            byte[] _id = GetFileID(name, path);

            int initialSize = GetSizeOfFile(name, path);
            int newSize = initialSize + sContentOfFile.Length;
            if (newSize > MAX_SIZE_OF_FILE)
            {
                Console.WriteLine("Greska - maksimalna dozvoljena velicina fajla je 64kB");
                return;
            }

            if (initialSize == 0)
            {
                WriteToDataSegment(_id, _contentOfFile);
            }
            else if (initialSize > 0)
            {
                byte[] _oldContentOfFile = ReadFromDataSegment(_id);
                List<byte> oldContentOfFile = _oldContentOfFile.ToList();

                DeleteFromDataSegment(_id); //Obrisem stari zapis u data segmentu pa kasnije upisem novi na kraj FS

                oldContentOfFile.AddRange(contentOfFile);  //na onaj stari sadrzaj dodajem novi sto je korisnik unio
                byte[] newContentOfFile = oldContentOfFile.ToArray();
                WriteToDataSegment(_id, newContentOfFile);
            }

            UpdateSizeOfFile(name, path, newSize);

            Update(); Update();
        }
        
        private void UpdateSizeOfFile(string name, string path, int newSize)
        {
            int oldSize = GetSizeOfFile(name, path);
            byte[] _id = GetFileID(name, path);

            if(!path.EndsWith("/"))
            {
                path += "/";
            }

            int i = 0;
            string newSizeS = newSize.ToString();
            byte[] _newSize = new byte[newSizeS.Length];
            foreach (var x in newSizeS)
                _newSize[i++] = (byte)x;

            int start = 0, end = 0;
            byte[] contentOfFS = File.ReadAllBytes("FileSystem.bin");
            for (i = 0; i < contentOfFS.Length; i++)
            {
                if (contentOfFS[i] == '~' && contentOfFS[i + 1] == _id[0] && contentOfFS[i + 2] == _id[1] && contentOfFS[i + 3] == _id[2] && contentOfFS[i + 4] == '~')
                {
                    int br = 0;
                    //Od i-te pozicije trazim 4. ~
                    for (int j = i + 1; j < contentOfFS.Length; j++)
                    {
                        if (contentOfFS[j] == '~')
                        {
                            br++;
                        }
                        if (br == 4)
                        {
                            start = j + 1;
                            end = start + (oldSize.ToString()).Length;
                            break;
                        }
                    }
                    break;
                }
            }

            List<byte> newContentOfFS = new List<byte>();
            for (i = 0; i < start; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }
            for (i = 0; i < _newSize.Length; i++)
            {
                newContentOfFS.Add(_newSize[i]);
            }
            for (i = end; i < contentOfFS.Length; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }

            RewriteFS(newContentOfFS.ToArray());

            Update(); Update();
        }

        private string GetDateCreated(string name, string path)
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                if (line.Contains("~" + path + name + "~"))
                {
                    string id = (line.Split('~'))[4];
                    reader.Close();
                    return id;
                }
                line = reader.ReadLine();
            }
            return "-1";
        }

        private byte[] GetFileID(string name, string path = "root/")
        {
            if (!path.EndsWith("/"))
                path += "/";

            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                if (line.Contains("~" + path + name + "~"))
                {
                    string id = (line.Split('~'))[1];
                    reader.Close();

                    byte[] _id = new byte[id.Length];
                    for (int i = 0; i < id.Length; ++i)
                        _id[i] = (byte)id[i];

                    return _id;
                }
                line = reader.ReadLine();
            }
            byte[] error = { (byte)'-', (byte)'1' };
            return error;
        }

        private int GetSizeOfFile(string name, string path)
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                if (line.Contains("~" + path + name + "~"))
                {
                    string size = (line.Split('~'))[5]; 
                    reader.Close();
                    return Int32.Parse(size);
                }
                line = reader.ReadLine();
            }
            reader.Close();
            return -1;
        }

        //RADI I OVA
        private void CreateFile(string name, string path = "root/")
        {
            if(!path.EndsWith("/"))
            {
                path += '/';
            }

            if(!path.Equals("root/"))
            {
                Regex regex = new Regex(@"(root/)([A-Za-z0-9.-]+)/"); //Provjera da li je validan zapis putanje, tj. da li je putanja unesena u obliku root/naziv_foldera
                Match match = regex.Match(path);

                if (match.Value != path || !Exists(match.Groups[2].Value, match.Groups[1].Value)) //Ili je putanja unesena u nepravilnom obliku ili ne postoji taj folder unutar root-a
                {
                    Console.WriteLine("Greska - unesena je nepostojeca putanja");
                    return;
                }
            }

            name = CheckName(name, path);

            FileOnFS file = new FileOnFS(name, GetNewId(), DateTime.Now, path);
            file.WriteToFile();
            
            numberOfFiles++;
            Update(); Update();
        }

        private void GetFileInformation(string name, string path = "root/")
        {
            if (!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            byte[] _id = GetFileID(name, path);
            int start;
            (start, _) = GetStartAndEndPositions(_id);
     
            Console.WriteLine("Naziv datoteke: " + name);
            Console.Write("ID: ");
            for (int i = 0; i < _id.Length; ++i)
                Console.Write((char)_id[i]);
            Console.WriteLine("\nPutanja: " + path + name);
            Console.WriteLine("Datum i vrijeme kreiranja: " + GetDateCreated(name, path));
            Console.WriteLine("Velicina: " + GetSizeOfFile(name, path));
            Console.WriteLine("Broj blokova: {0}", Math.Ceiling((double)GetSizeOfFile(name, path)/(double)SIZE_OF_BLOCK));
            Console.WriteLine("Pocetna lokacija blokova: {0}", start);
        }

        //RADIIII
        private void MoveFile(string name, string newPath, string path = "root/") //Ako se zeli premjestiti datoteka koja je na putanji root/folder/datoteka, funkcija ce se pozvati sa putanje root/folder
        {
            if (!Exists(name, path)) //Ako neko pokusa premjestiti datoteku koja ne postoji na trenutnoj putanji
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            if(!newPath.EndsWith("/"))
            {
                newPath += "/";
            }
            if(!path.EndsWith("/"))
            {
                path += "/";
            }

            if(newPath != "root/") //Ako se premjesta na neku putanju poput root/naziv_foldera
            {
                Regex regex = new Regex(@"(root/)([A-Za-z0-9.-]+)/"); //Provjera da li je validan zapis putanje, tj. da li je putanja unesena u obliku root/naziv_foldera
                Match match = regex.Match(newPath);

                if (match.Value != newPath || !Exists(match.Groups[2].Value, match.Groups[1].Value)) //Ili je putanja unesena u nepravilnom obliku ili ne postoji taj folder unutar root-a
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

            byte[] _id = GetFileID(name, path);

            int i = 0;
            byte[] _newPath = new byte[newPath.Length];
            foreach (var x in newPath)
                _newPath[i++] = (byte)x;

            int start = 0, end = 0;
            byte[] contentOfFS = File.ReadAllBytes("FileSystem.bin");
            for (i = 0; i < contentOfFS.Length; i++)
            {
                if (contentOfFS[i] == '~' && contentOfFS[i + 1] == _id[0] && contentOfFS[i + 2] == _id[1] && contentOfFS[i + 3] == _id[2] && contentOfFS[i + 4] == '~')
                {
                    start = i + name.Length + 6;
                    end = start + path.Length;
                    break;
                }
            }

            List<byte> newContentOfFS = new List<byte>();
            for (i = 0; i < start; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }
            for (i = 0; i < _newPath.Length; i++)
            {
                newContentOfFS.Add(_newPath[i]);
            }
            for (i = end; i < contentOfFS.Length; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }

            RewriteFS(newContentOfFS.ToArray());

            Update(); Update();
        }

        //RADIII
        private void Rename(string name, string newName, string path ="root/")
        {
            if(!path.EndsWith("/"))
            {
                path += "/";
            }

            if(!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji direktorijum/datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            newName = CheckName(newName, path);

            byte[] _id = GetFileID(name, path);

            int i = 0;
            byte[] _newName = new byte[newName.Length];
            foreach (var x in newName)
                _newName[i++] = (byte)x;

            int start1 = 0, end1 = 0, start2 = 0, end2 = 0;
            byte[] contentOfFS = File.ReadAllBytes("FileSystem.bin");
            for (i = 0; i < contentOfFS.Length; i++)
            {
                if (contentOfFS[i] == '~' && contentOfFS[i + 1] == _id[0] && contentOfFS[i + 2] == _id[1] && contentOfFS[i + 3] == _id[2] && contentOfFS[i + 4] == '~')
                {
                    start1 = i + 5;
                    end1 = start1 + name.Length;
                    start2 = end1 + path.Length + 1;
                    end2 = start2 + name.Length;
                    break;
                }
            }

            List<byte> newContentOfFS = new List<byte>();
            for (i = 0; i < start1; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }
            for (i = 0; i < _newName.Length; i++)
            {
                newContentOfFS.Add(_newName[i]);
            }
            for (i = end1; i < start2; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }
            for (i = 0; i < _newName.Length; i++)
            {
                newContentOfFS.Add(_newName[i]);
            }
            for (i = end2; i < contentOfFS.Length; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }

            RewriteFS(newContentOfFS.ToArray());

            Update(); Update();
        }


        //RADI - NE DIRAJ !
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
                if(path.Equals("root/") && matches[3].Value == (new Regex(@"root/([A-Za-z0-9.-]+)")).Match(matches[3].Value).Value) //ispisem onaj sadrzaj kod koga je putanja = root/naziv, bez ovog uslova bi ispisalo sve na file sistemu jer sve putanje pocinju sa root/
                {
                    Console.WriteLine(matches[0].Value + " - " + matches[2].Value + " - " + matches[3].Value + " - " + matches[4].Value);
                }
                else if (!path.Equals("root/") && matches[3].Value.Contains(path))
                {
                    Console.WriteLine(matches[0].Value + " - " + matches[2].Value + " - " + matches[3].Value + " - " + matches[4].Value);
                }
                line = reader.ReadLine();
            }
            
            reader.Close();
        }

        private void OpenDirectory(string name)
        {
            string currentPath = "root/" + name;

            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            _ = reader.ReadLine();
            Regex regex2 = new Regex(@"[A-Za-z0-9/ :.-]+");
            bool flag = false;

            string line = reader.ReadLine();
            while(!line.Contains(separator)) //Prodje kroz MFT(zapisi o datotekama i direktorijumima) i pretrazi da li postoji dati folder
            {
                MatchCollection matches = regex2.Matches(line);
                if (matches[0].Value == "dir" && matches[2].Value == name && matches[3].Value == (currentPath))
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
                    Console.Write(currentPath.Replace("root", "HOME") + ">");

                    string inputLine = Console.ReadLine();
                    string[] words = inputLine.Split(' ');

                    switch (words[0])
                    {
                        case "exit":
                            Environment.Exit(EXIT);
                            break;

                        case "clr":
                            Console.Clear();
                            break;

                        case "ls": //RADI - NE DIRAJ NISTA - 5. funkcija
                            {
                                if (words.Count() == 1)
                                {
                                    GetContentOfDirectory(currentPath + "/");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help ls\" za pomoc");
                                }
                                break;
                            }

                        case "cd": //RADI - moja funkcija
                            {
                                if(words.Count() == 2 && words[1] != "..")
                                {
                                    Console.WriteLine("Greska - za pozicioniranje u root folder unesite \"cd ..\"");
                                }
                                else if (words.Count() == 2 && words[1] == "..")
                                {
                                    return; //Izlazak iz trenutno otvorenog foldera
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help cd\" za pomoc");
                                }
                                break;
                            }

                        case "help": //DOBRO JE - moja funkcija
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

                        case "df": //DOBRO JE - NE DIRAJ NISTA - moja funkcija
                            {
                                if (words.Count() == 1) //df mi je samo za slobodan memorijski prostor
                                {
                                    GetRootInformation(false); //jedinicu saljem kao neki flag da bi mi ispisalo samo slobodan mem. prostor
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

                        case "mkdir": //RADII - 1. funkcija
                            {
                                if (words.Count() == 2)
                                {
                                    MakeDirectory(words[1]);
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help mkdir\" za pomoc");
                                }
                                break;
                            }

                        case "create": //RADIIIIII - 2. funkcija
                            {
                                if (words.Count() == 2)
                                {
                                    CreateFile(words[1], currentPath); //Ako odavde pozivam create to znaci da se unutar trenutno otvorenog foldera (root/ime_foldera/) kreira fajl sa nazivom words[1]
                                }
                                else if(words.Count() == 3)
                                {
                                    CreateFile(words[1], words[2]); //A možemo putanju i sami unijeti (words[2])
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help create\" za pomoc");
                                }
                                break;
                            }
                             
                        case "rename": //RADIIII - 8. funkcija
                            {
                                if (words.Count() == 3)
                                {
                                    Rename(words[1], words[2], currentPath + "/");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help rename\" za pomoc");
                                }
                                break;
                            }

                        case "mv": //7. funkcija
                            {
                                if (words.Count() == 3)
                                {
                                    MoveFile(words[1], words[2] , currentPath + "/"); //words[1] je naziv fajla, a words[2] krajnja putanja
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help mv\" za pomoc");
                                }
                                break;
                            }

                        case "echo":
                            {
                                if (words.Count() == 2)
                                {
                                    WriteToFile(words[1], currentPath + "/");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help echo\" za pomoc");
                                }
                                break;
                            }

                        case "put":
                            {
                                if (words.Count() == 1)
                                {
                                    PutFile();
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help put\" za pomoc");
                                }
                                break;
                            }

                        case "get":
                            {
                                if (words.Count() == 1)
                                {
                                    GetFile();
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help get\" za pomoc");
                                }
                                break;
                            }

                        case "cp":
                            {
                                if (words.Count() == 2)
                                {
                                    CopyFile(words[1], currentPath + "/");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help copy\" za pomoc");
                                }
                                break;
                            }

                        case "cat":
                            {
                                if (words.Count() == 2)
                                {
                                    GetContentOfFile(words[1], currentPath + "/");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help cat\" za pomoc");
                                }
                                break;
                            }

                        case "rm":
                            {
                                if (words.Count() == 2)
                                {
                                    RemoveFile(words[1], currentPath + "/");
                                }
                                else if (words.Count() == 3 && words[1] == "-r")
                                {
                                    Console.WriteLine("Brisanje foldera moguce je samo iz root-a");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help rm\" za pomoc");
                                }
                                break;
                            }

                        case "stat":
                            {
                                if(words.Count() == 2)
                                {
                                    GetFileInformation(words[1], currentPath + "/");
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help stat\" za pomoc");
                                }
                                break;
                            }

                        default:
                            Console.WriteLine("Greska - nepoznata opcija! Za pomoc pri radu unesite \"help\"");
                            break;
                    }
                }
            }
        }

        private (string path, string name) CheckPath(string path)
        {
            Regex regex1 = new Regex(@"(root/)([A-Za-z0-9.-]+)"); //Provjera da li je validan zapis putanje, tj. da li je putanja unesena u obliku root/naziv_foldera
            Regex regex2 = new Regex(@"(root/)([A-Za-z0-9.-]+)/([A-Za-z0-9.-]+)");
            Match match1 = regex1.Match(path);
            Match match2 = regex2.Match(path);

            if (match1.Value == path)
            {
                return (match1.Groups[1].Value, match1.Groups[2].Value);
            }
            else if (match2.Value == path)
            {
                return (match1.Groups[1].Value + match1.Groups[2].Value, match1.Groups[3].Value);
            }
            return ("", "");
        }

        private void GetFile() //sa ovog fajl sistema na racunar
        {
            Console.WriteLine("Unesite putanju ulaznog fajla(primjer: root/folder/datoteka):");
            string inputPath = Console.ReadLine();
            Console.WriteLine(@"Unesite putanju izlaznog fajla(primjer: C:\\Users\\Tatjana Tomic\\Desktop\\dat.txt):");
            string outputPath = Console.ReadLine();

            string iPath, iFile;
            (iPath, iFile) = CheckPath(inputPath);
            if (iPath == "" && iFile == "")
            {
                Console.WriteLine("Greska - unesena je nevazeca putanja {0}", inputPath);
                return;
            }

            if (!Exists(iFile, iPath))
            {
                Console.WriteLine("Greska - fajl {0} nije pronadjen na fajl sistemu", inputPath);
                return;
            }

            byte[] contentOfFile = ReadFromDataSegment(GetFileID(iFile, iPath));
            Console.WriteLine(contentOfFile.Length);
            try
            {
                File.WriteAllBytes(outputPath, contentOfFile);
                /*FileStream file = new FileStream(outputPath, FileMode.Create);
                file.Close();
                file = new FileStream(outputPath, FileMode.Open);
                BinaryWriter bw = new BinaryWriter(file);
                bw.Write(contentOfFile);
                bw.Close();
                file.Close();*/
            /*
                BinaryWriter writer = new BinaryWriter(new FileStream(outputPath, FileMode.Create));
                writer.Write(contentOfFile);
                writer.Close();*/            
            }
            catch(Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom upisivanja u fajl {0}", outputPath);
                Console.WriteLine(e.Message);
            }
            
        }
        private void PutFile() //sa racunara na ovaj fajl sistem
        {
            Console.WriteLine(@"Unesite putanju ulaznog fajla(primjer: C:\\Users\\Tatjana Tomic\\Desktop\\dat.txt):");
            string inputPath = Console.ReadLine();
            Console.WriteLine("Unesite putanju izlaznog fajla(primjer: root/folder/datoteka):");
            string outputPath = Console.ReadLine();

            if(!File.Exists(inputPath))
            {
                Console.WriteLine("Greska - fajl {0} ne postoji.", inputPath);
                return;
            }

            string oPath, oFile;
            (oPath, oFile) = CheckPath(outputPath);
            if(oPath == "" && oFile == "")
            {
                Console.WriteLine("Greska - unesena je nevazeca putanja {0}", outputPath);
                return;
            }

            if(Exists(oFile, oPath))
            {
                Console.Write("Greska - fajl {0} vec postoji na fajl sistemu", outputPath);
                return;
            }

            try
            {
                File.OpenRead(inputPath);
            }
            catch(Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom otvaranja fajla {0}", inputPath);
                Console.WriteLine(e.Message);
                return;
            }
            byte[] _contentOfInputFile = File.ReadAllBytes(inputPath);
            
            if(_contentOfInputFile.Count() > MAX_SIZE_OF_FILE)
            {
                Console.WriteLine("Greska - najveca dozvoljena velicina fajla je {0}b", MAX_SIZE_OF_FILE);
                return;
            }

            CreateFile(oFile, oPath);
            byte[] _id = GetFileID(oFile, oPath);

            WriteToDataSegment(_id, _contentOfInputFile);

            UpdateSizeOfFile(oFile, oPath, _contentOfInputFile.Length);

            Update(); Update();
        }

        private void CopyFile(string name, string path = "root/")
        {
            if (!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            string newName = "";
            for(int i=1; i<1000; ++i) //valjda niko nece praviti vise od 1000 kopija jedne datoteke :'D
            {
                if (!Exists(name + "-copy" + i.ToString(), path))
                {
                    newName = name + "-copy" + i.ToString();
                    break;
                }
            }

            CreateFile(newName, path);

            if(GetSizeOfFile(name,path) != 0)
            {
                byte[] contentOfFile = ReadFromDataSegment(GetFileID(name, path)); //procitam sadrzaj originalnog fajla - name

                WriteToDataSegment(GetFileID(newName, path), contentOfFile); //upisem u kopiju - newName

                UpdateSizeOfFile(newName, path, contentOfFile.Length);
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
            writer.Write(initialUsedSpace + "~" + (MAX_SIZE_OF_FILE_SYSTEM - initialUsedSpace) + "~" + MAX_SIZE_OF_FILE_SYSTEM + '~' + '\n');
            writer.Write(separator + '\n');
            writer.Close();
        }
        
        //RADI
        private void Help(string method)
        {
            switch(method)
            {
                case "help":
                    Console.WriteLine("Lista svih opcija: mkdir create put get ls cp mv rename echo cat rm stat df cd");
                    Console.WriteLine("Za detaljno uputstvo unesite \"help naziv_opcije\"");
                    Console.WriteLine("Za izlaz iz programa unesite \"exit\"");
                    Console.WriteLine("Za brisanje teksta konzole unesite \"clr\"");
                    break;

                case "cd":
                    Console.WriteLine("Omogucava pozicioniranje u zadati direktorijum, prilikom poziva potrebno je navesti naziv_komande naziv_direktorijuma");
                    Console.WriteLine("Za vracanje u roditeljski direktorijum unesite \"cd ..\"");
                    break;

                case "df":
                    Console.WriteLine("Prikazuje slobodan prostor na fajl sistemu, prilikom poziva potrebno je navesti naziv_komande. Za detaljniji prikaz trenutnog stanja fajl sistema potrebno je navesti \"df -h\"");
                    break;

                case "mkdir":
                    Console.WriteLine("Kreira novi direktorijum, prilikom poziva potrebno je navesti naziv_komande naziv_direktorijuma. Novi direktorijum se uvijek dodaje unutar root direktorijuma");
                    break;

                case "create":
                    Console.WriteLine("Kreira novu datoteku na zadatoj putanji. Prilikom poziva potrebno je navesti naziv_komande naziv_datoteke (putanja). Putanja je opcioni parametar, ukoliko se ne navede podrazumijeva se trenutna putanja.");
                    break;

                case "put":
                    Console.WriteLine("Omogucava dodavanje datoteke sa fajl sistema racunara na novi fajl sistem, prilikom poziva potrebno je navesti naziv_komande");
                    break;

                case "get":
                    Console.WriteLine("Omogucava dodavanje datoteke sa novog fajl sistema na fajl sistema racunara, prilikom poziva potrebno je navesti naziv_komande");
                    break;

                case "ls":
                    Console.WriteLine("Ispisuje sadrzaj direktorijuma u kom ste trenutno pozicionirani");
                    break;

                case "cp":
                    Console.WriteLine("Kreira kopiju datoteke na trenutnoj putanji. Prilikom poziva potrebno je navesti naziv_komande naziv_datoteke");
                    break;

                case "mv":
                    Console.WriteLine("Omogucava premjestanje datoteke sa trenutne putanje na zadatu lokaciju, prilikom poziva potrebno je navesti naziv_komande naziv_fajla krajnja_putanja");
                    break;

                case "rename":
                    Console.WriteLine("Omogucava promjenu naziva datoteke ili diretorijuma. Potrebno je navesti naziv_komande trenutni_naziv novi_naziv. Promjena naziva direktorijuma moguca je ukoliko ste pozicionirani u root folderu, a promjena naziva datoteke ukoliko ste pozicionirani u folder u kom se datoteka nalazi");
                    break;

                case "echo":
                    Console.WriteLine("Omogucava upis tekstualnog sadrzaja u datoteku, prilikom poziva poptreno je navesti naziv_komande naziv_datoteke");
                    break;

                case "cat":
                    Console.WriteLine("Omogucava prikaz sadrzaja tekstualne datoteke. Prilikom poziva potrebno je navesti naziv_komande naziv_datoteke");
                    break;

                case "rm":
                    Console.WriteLine("Omogucava brisanje datoteke ili direktorijuma. Za brisanje datoteke potrebno je navesti naziv_komande naziv_datoteke. Za brisanje direktorijuma i njegovog kompletnog sadrzaja potrebno je unijeti naziv_komande -r naziv_ili_putanja_direktorijuma");
                    break;

                case "stat":
                    Console.WriteLine("Omogucava ispis podataka o datoteci, prilikom poziva potrebno je navesti naziv_komande naziv_datoteke");
                    break;

                default:
                    Console.WriteLine("Greska - nepostojeca opcija! Za listu svih opcija unesite \"help\"");
                    break;


            }
            
        }

        //NE DIRAJ!!!
        private void GetRootInformation(bool flag = true)
        {
            if (flag)
            {
                Console.WriteLine("Broj direktorijuma: " + numberOfDirectories);
                Console.WriteLine("Broj datoteka: " + numberOfFiles);
                Console.WriteLine("Iskoriscen memorijski prostor: " + usedSpace + "b");
                Console.WriteLine("Slobodan memorijski prostor: " + freeSpace + "b");
                Console.WriteLine("Ukupna velicina fajl sistema: " + MAX_SIZE_OF_FILE_SYSTEM + "b");
            }
            else
            {
                Console.WriteLine("Slobodan memorijski prostor: " + freeSpace + "b");
            }
        }

        //Ne DIRAJ
        private void MakeDirectory(string name, string path = "root/")
        {
            name = CheckName(name, path);

            Directory directory = new Directory(name, GetNewId(), DateTime.Now, path);
            directory.WriteToFile();
            numberOfDirectories++;
            Update(); Update();
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
                    Console.WriteLine("Unesite novi naziv");
                    name = Console.ReadLine();
                }
                else if(exists)
                {
                    Console.WriteLine("Greska - vec postoji datoteka ili direktorijum sa datim nazivom");
                    Console.WriteLine("Unesite novi naziv");
                    name = Console.ReadLine();
                }
            }
        }

        private bool Exists(string name, string path)
        {
            bool flag = false;

            if (!path.EndsWith("/"))
                path += "/";

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

            usedSpace = firstLine.Length + content.Length + 1; // 1 dodajem zbog razlike u stvarnoj velicini fajla i duzinama ovih stringova
            freeSpace = MAX_SIZE_OF_FILE_SYSTEM - usedSpace;

            StreamWriter writer = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Open));
            writer.Write("HOME~" + numberOfDirectories + "~" + numberOfFiles + "~");
            writer.Write(usedSpace + "~" + (MAX_SIZE_OF_FILE_SYSTEM - usedSpace) + "~" + MAX_SIZE_OF_FILE_SYSTEM + "~" + '\n');
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
                lastId = 100;
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