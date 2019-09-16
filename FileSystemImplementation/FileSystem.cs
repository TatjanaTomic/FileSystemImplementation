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
        public static readonly int SIZE_OF_BLOCK = 512; //velicina jednog bloka za cuvanje sadrzaja datoteke
        public static readonly int initialUsedSpace = 70; //prilikom kreiranja fajl sistema velicina je 50 bajta zbog upisivanja inicijalnih vrijednosti o fajl sistemu
        public static readonly int initialFreeSpace = MAX_SIZE_OF_FILE_SYSTEM - initialUsedSpace;
        public static int numberOfFiles = 0; //ukupan broj fajlova na fajl sistemu, pocetna vrijednost 0
        public static int numberOfDirectories = 0; //ukupan broj direktorijuma na fajl sistemu, pocetna vrijednost 0
        public static int usedSpace;
        public static int freeSpace;
        public static readonly int EXIT = 0;
        public static readonly string separator = "~~~DATA-SEGMENT~~~";

        /// <summary>
        /// Funkcija se prva poziva prilikom pokretanja i predstavlja izvršavanje programa dok je korisnik pozicioniran u root folderu fajl sistema.
        /// Prvo se provjeri da li postoji fajl sistem, ukoliko ne postoji poziva se funkcija CreateFileSystem(), a ukoliko postoji čitaju se podaci o trenutnom stanju FS.
        /// Funkcija čita liniju sa konzole i na osnovu korisnikovog unosa poziva odgovarajuću komandu fajl sistema.
        /// </summary>
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
                Console.Write('\n' + "HOME>");

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

                    case "ls":
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

                    case "cd":
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

                    case "help":
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

                    case "df":
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

                    case "mkdir": //mkdir pozivam samo sa nazivom foldera, ne unosim putanju jer se podrazumijevano kreiraju unutar root-a
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

                    case "create": //create moze da se pozove samo sa nazivom datoteke, tada se podrazumijeva da je datoteka na putanji na kojoj se trenutno nalazimo ili da se zada putanja na kojoj bi trebalo da se nalazi ta datoteka
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

                    case "rename": //poziva se sa putanje na kojoj se trenutno nalazimo
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
                        
                    case "mv": //sa trenutne putanje premjesta na unesenu putanju
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

                    case "echo":
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
        
        /// <summary>
        /// Funkcija se poziva prilikom pokretanja programa ukoliko fajl sistem vec ne postoji. 
        /// Kreira binarnu datoteku sa nazivom FileSystem i u nju upisuje inicijalne podatke o FS.
        /// </summary>      
        private void CreateFileSystem()
        {
            StreamWriter writer1 = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Create));
            writer1.Write("HOME~" + numberOfDirectories.ToString().PadLeft(8)
                            + "~" + numberOfFiles.ToString().PadLeft(8)
                            + "~" + initialUsedSpace.ToString().PadLeft(8)
                            + "~" + initialFreeSpace.ToString().PadLeft(8)
                            + "~" + MAX_SIZE_OF_FILE_SYSTEM + "~" + '\n');
            writer1.Write(separator + '\n');
            writer1.Close();
        }
        
        /// <summary>
        /// Funkcija se poziva prilikom pokretanja programa i čita prvu liniju FileSystem.bin datoteke.
        /// Iz pročitane linije čita podatke o trenutnom stanju FS i smješta podatke u odgovarajuće promjenljive - atribute objekta klase FileSystem
        /// Linija je formata "HOME~      25~      44~   96539~20874981~20971520~ pa pomoću regex-a mečira potrebne brojeve
        /// </summary>
        private void LoadData()
        {
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string firstLine = reader.ReadLine();
            reader.Close();

            Regex regex = new Regex(@"[0-9]+");
            MatchCollection matches = regex.Matches(firstLine);

            string _numberOfDirectories = matches[0].Value;
            numberOfDirectories = Convert.ToInt32(_numberOfDirectories);

            string _numberOfFiles = matches[1].Value;
            numberOfFiles = Convert.ToInt32(_numberOfFiles);

            string _usedSpace = matches[2].Value;
            usedSpace = Convert.ToInt32(_usedSpace);

            string _freeSpace = matches[4].Value;
            freeSpace = Convert.ToInt32(_freeSpace);
        }
        
        /// <summary>
        /// Funkcija se pozivi kada god se u fajl sistemu desi izmjena kako bi se ažurirali podaci prve linije
        /// Funkcija pročita sve bajtove FileSystem.bin datoteke u niz bajtova, upiše prvu liniju nanovo a ostatak sadržaja(niza) prepiše
        /// </summary>
        private void Update()
        {
            byte[] content = File.ReadAllBytes("FileSystem.bin");

            usedSpace = content.Length;
            freeSpace = MAX_SIZE_OF_FILE_SYSTEM - usedSpace;

            StreamWriter writer1 = new StreamWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            writer1.Write("HOME~" + numberOfDirectories.ToString().PadLeft(8)
                            + "~" + numberOfFiles.ToString().PadLeft(8)
                            + "~" + usedSpace.ToString().PadLeft(8)
                            + "~" + freeSpace.ToString().PadLeft(8)
                            + "~" + MAX_SIZE_OF_FILE_SYSTEM + "~" + '\n');
            writer1.Close();

            BinaryWriter writer2 = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Append));
            for (int i = 51; i < content.Length; i++) // Krećem od 51. pozicije jer je prvih 50 bajtova rezervisano za prvu liniju
                writer2.Write(content[i]);
            writer2.Close();
        }
        
        /// <summary>
        /// Funkcija se poziva prilikom unosa komande "mkdir"
        /// Funkcija kreira objekat tipa Directory i podatke upisuje u FileSystem.bin datoteku u vidu jednog zapisa u MFT tabeli
        /// </summary>
        /// <param name="name">Naziv foldera koji se kreira</param>
        /// <param name="path">Putanja na kojoj se direktorijum kreira(po uslovu zadatka to je uvijek root)</param>
        private void MakeDirectory(string name, string path = "root/")
        {
            name = CheckName(name, path, 'd');

            Directory directory = new Directory(name, GetNewId(), DateTime.Now, path);
            if(directory.WriteToMFT())
            {
                numberOfDirectories++;
                Update();
            }
            
        }
        
        /// <summary>
        /// Funkcija se poziva prilikom unosa komande "create"
        /// Funkcija kreira objekat tipa FileOnFS i podatke upisuje u FileSystem.bin datoteku u vidu jednog zapisa u MFT tabeli
        /// </summary>
        /// <param name="name">Naziv fajla koji se kreira</param>
        /// <param name="path">Putanja na kojoj se fajl kreira</param>
        private void CreateFile(string name, string path = "root/")
        {
            if(!path.EndsWith("/"))
            {
                path += '/';
            }

            if (!path.Equals("root/")) //Pošto korisnik može da unese putanju na kojoj želi da se kreira fajl, potrebno ju je provjeriti
            {
                //Ako korisnik želi da kreira fajl npr. na putanji root/folder1 potrebno je provjeriti da li postoji folder1
                string tmpPath, tmpName;
                (tmpPath, tmpName) = SplitPath(path.Substring(0, path.Length - 1)); //pošto putanja završava sa / trebam izbaciti taj znak
                if(tmpName == "" || tmpPath == "")
                {
                    Console.WriteLine("Greska - unesena je nepostojeca putanja.");
                    return;
                }
                if(!Exists(tmpName, tmpPath))
                {
                    Console.WriteLine("Greska - pokusavate dodati fajl {0} u folder {1} koji ne postoji na fajl sistemu.", name, tmpName);
                    return;
                }
            }

            name = CheckName(name, path, 'f');

            FileOnFS file = new FileOnFS(name, GetNewId(), DateTime.Now, path);
            if (file.WriteToMFT())
            {
                numberOfFiles++;
                Update();
            }
        }
        
        /// <summary>
        /// Funkcija vraća niz ID-jeva (u formatu stringa) fajlova koji se nalaze u zadatom folderu
        /// </summary>
        /// <param name="name">naziv direktorijuma čiji se sadržaj traži</param>
        /// <returns></returns>
        private string[] GetFilesOfDirectory(string nameOfDirectory)
        {
            LinkedList<string> files = new LinkedList<string>();

            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                if (line.Contains("~root/" + nameOfDirectory + "/"))
                {
                    files.AddLast(line.Split('~')[2]);
                }
                line = reader.ReadLine();
            }
            reader.Close();

            return files.ToArray();
        }
        
        /// <summary>
        /// Funkcija odgovara komandi "rm -r"
        /// Funkcija briše folder i njegov kompletan sadržaj
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
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

            Update();

        }

        /// <summary>
        /// Pomoćna funkcija, uklanja zapis iz MFT tabele
        /// Funkcija pročita cijeli fajl kao niz bajtova, pronađe odgovarajući podniz koji treba ukloniti a zatip prepiše binarnu datoteku sadrzajem koji se ne briše
        /// </summary>
        /// <param name="type">'d' ako je direktorijum, 'f' ako je fajl</param>
        /// <param name="name">naziv fajla/foldera koji se uklanja</param>
        /// <param name="path">putanja na kojoj se traženi fajl/folder nalazi</param>
        private void RemoveMftRecord(char type, string name, string path = "root/")
        {
            byte[] contentOfFS = File.ReadAllBytes("FileSystem.bin");

            byte[] _id = GetFileID(name, path);

            int start = 0, end = 0;
            for(int i=0; i<contentOfFS.Length; i++)
            {
                if (contentOfFS[i] == '~' && contentOfFS[i + 1] == _id[0] && contentOfFS[i + 2] == _id[1] && contentOfFS[i + 3] == _id[2] && contentOfFS[i + 4] == '~')
                {
                    if(type == 'd')
                    {
                        start = i - 4; //početak linije koju treba obrisati
                        break;
                    }
                    else if(type == 'f')
                    {
                        start = i - 5; //početak linije koju treba obrisati
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
                } //znaci trazim prvi znak za novi red - tu je kraj linije koju trebam brisati
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

        /// <summary>
        /// Pomoćna funkcija koja obriše trenutni sadržaj datoteke FileSystem.bin i upiše novi sadržaj
        /// </summary>
        /// <param name="content">novi sadržaj koji se upisuje</param>
        private void RewriteFS(byte[] content)
        {
            BinaryWriter writer = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            writer.Write(content);
            writer.Close();
        }

        /// <summary>
        /// Funkcija odgovara komandi "rm"
        /// Uklanja fajl sa fajl sistema
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
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

            Update();

        }

        /// <summary>
        /// Funkcija odgovara komandi cat
        /// Ispisuje sadržaj tekstualne datoteke
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void GetContentOfFile(string name, string path = "root/")
        {
            if (!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            if(!name.EndsWith(".txt"))
            {
                Console.WriteLine("Greska - komanda cat radi samo sa tekstualnim datotekama");
                return;
            }

            if (GetSizeOfFile(name, path) == 0)
            {
                Console.WriteLine("Datoteka {0} je prazna.", name);
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

        /// <summary>
        /// Pomoćna funkcija koja služi za uklanjanje "korisnog sadržaja" fajla
        /// FS se pročita u niz bajtova i pretraga se vrši na osnovu ID-ja fajla
        /// </summary>
        /// <param name="_id"></param>
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

            Update();
        }

        /// <summary>
        /// Pomoćna funkcija koja dodaje sadržaj datoteke u data segment
        /// Funkcija uvijek dodaje na kraj FileSystem.bin datoteke
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="content"></param>
        private bool WriteToDataSegment(byte[] _id, byte[] content)
        {
            byte[] oldContent = File.ReadAllBytes("FileSystem.bin");

            if(content.Length + _id.Length + 8 > freeSpace)
            {
                Console.WriteLine("Greška - nije moguće izvršiti radnju jer nema dovoljno memorije na fajl sistemu.");
                return false;
            }

            BinaryWriter writer = new BinaryWriter(new FileStream("FileSystem.bin", FileMode.Truncate));
            writer.Write(oldContent);
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
            Update();

            return true;
        }

        /// <summary>
        /// Pomoćna funkcija koja čita sadržaj datoteke na osnovu njenog ID-ja
        /// </summary>
        /// <param name="_id"></param>
        /// <returns>vraća sadržaj kao niz bajtova</returns>
        private byte[] ReadFromDataSegment(byte[] _id)
        {
            byte[] _contentOfFS = File.ReadAllBytes("FileSystem.bin");

            int start, end; //promjenljive koje ce mi u prethodnom nizu oznaciti pocetak i kraj data segmenta trazenog fajla
            (start, end) = GetStartAndEndPositions(_id);

            LinkedList<byte> contentOfFile = new LinkedList<byte>();
            for (int i = start; i <= end; i++)
            {
                contentOfFile.AddLast(_contentOfFS[i]);
            }

            return contentOfFile.ToArray();
        }

        /// <summary>
        /// Funkcija čita FS kao niz bajtova i u zadatom nizu traži početni i krajnji indeks između kojih se nalazi sadržaj fajla
        /// </summary>
        /// <param name="_id"></param>
        /// <returns>vraća uređeni par (int start, int end) </returns>
        private (int, int) GetStartAndEndPositions(byte[] _id)
        {
            byte[] _contentOfFS = File.ReadAllBytes("FileSystem.bin");
            int start = 0, end = 0; //promjenljive koje ce mi u prethodnom nizu oznaciti pocetak i kraj data segmenta trazenog fajla
            for (int i = 0; i < _contentOfFS.Length; i++)
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

        /// <summary>
        /// Funkcija odgovara komandi "echo"
        /// Funkcija upisuje sadržaj koji korisnik unese u tekstualnu datoteku
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void WriteToFile(string name, string path = "root/")
        {
            if(!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            if(!name.EndsWith(".txt"))
            {
                Console.WriteLine("Greska - komanda echo radi samo sa tekstulanim datotekama.");
                return;
            }

            Console.WriteLine("Unesite sadrzaj datoteke:");
            string sContentOfFile = Console.ReadLine();
            byte[] _contentOfFile = new byte[sContentOfFile.Length];
            for (int i = 0; i < sContentOfFile.Length; i++)
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
                if (!WriteToDataSegment(_id, _contentOfFile))
                    return;
            }
            else if (initialSize > 0)
            {
                if(contentOfFile.Count > freeSpace)
                {
                    Console.WriteLine("Greska - nije moguće dovršiti radnju jer nema dovoljno memorije na fajl sistemu.");
                    return;
                }

                byte[] _oldContentOfFile = ReadFromDataSegment(_id);
                List<byte> oldContentOfFile = _oldContentOfFile.ToList();

                DeleteFromDataSegment(_id); //Obrisem stari zapis u data segmentu pa kasnije upisem novi na kraj FS

                oldContentOfFile.AddRange(contentOfFile);  //na onaj stari sadrzaj dodajem novi sto je korisnik unio
                byte[] newContentOfFile = oldContentOfFile.ToArray();
                WriteToDataSegment(_id, newContentOfFile);
            }

            UpdateSizeOfFile(name, path, newSize);

            Update();
        }

        /// <summary>
        /// Pomoćna funkcija koja ažurira veličinu fajla(u MFT zapisu)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="newSize"></param>
        private void UpdateSizeOfFile(string name, string path, int newSize)
        {
            byte[] _id = GetFileID(name, path);
            int newNumberOfBlocks = (int)Math.Ceiling((double)(newSize/SIZE_OF_BLOCK));

            int i;
            char[] array = (newSize.ToString() + "~" + newNumberOfBlocks.ToString()).ToCharArray();
            byte[] _array = new byte[array.Length];
            for (i = 0; i < array.Length; i++)
                _array[i] = Convert.ToByte(array[i]);

            int index1 = 0, index2 = 0;
            byte[] contentOfFS = File.ReadAllBytes("FileSystem.bin");
            for (i = 0; i < contentOfFS.Length; i++)
            {
                if (contentOfFS[i] == '~' && contentOfFS[i + 1] == _id[0] && contentOfFS[i + 2] == _id[1] && contentOfFS[i + 3] == _id[2] && contentOfFS[i + 4] == '~')
                {
                    int br = 0;
                    for (int j = i + 1; j < contentOfFS.Length; j++) //od i-te pozicije trazim četvrto ~, nakon njega treba upisati novu velicinu fajla
                    {
                        if (contentOfFS[j] == '~')
                        {
                            br++;
                        }
                        if (br == 4) //kad nadjem cetvrto ~ onda trazim znak za novi red jer od tog novog reda ostale bajtove trebam prepisati
                        {
                            index1 = j + 1;
                            for (int k = index1; k < contentOfFS.Length; k++)
                            {
                                if (contentOfFS[k] == '\n')
                                {
                                    index2 = k;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            List<byte> newContentOfFS = new List<byte>();
            for (i = 0; i < index1; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }
            for (i = 0; i < _array.Length; i++)
            {
                newContentOfFS.Add(_array[i]);
            }
            for (i = index2; i < contentOfFS.Length; i++)
            {
                newContentOfFS.Add(contentOfFS[i]);
            }

            RewriteFS(newContentOfFS.ToArray());

            Update();
        }

        /// <summary>
        /// Pomoćna funkcija, vraća datum kreiranja za dati fajl
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Pomoćna funkcija, vraća ID traženog fajla
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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
                    for (int i = 0; i < id.Length; i++)
                        _id[i] = (byte)id[i];

                    return _id;
                }
                line = reader.ReadLine();
            }
            byte[] error = { (byte)'-', (byte)'1' };
            return error;
        }

        /// <summary>
        /// Pomoćna funkcija, vraća ID traženog fajla
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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
                    return Convert.ToInt32(size);
                }
                line = reader.ReadLine();
            }
            reader.Close();
            return -1;
        }

        /// <summary>
        /// Funkcija odgovara komandi "stat", ispisuje podatke o datoteci
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void GetFileInformation(string name, string path = "root/")
        {
            if (!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }
            if(GetType(name, path) == 'd')
            {
                Console.WriteLine("Greska - komanda stat radi sa datotekama");
                return;
            }

            byte[] _id = GetFileID(name, path);
            int size = GetSizeOfFile(name, path);
            int start;
            if (size != 0)
                (start, _) = GetStartAndEndPositions(_id);
            else
                start = -1;

            Console.WriteLine("Naziv datoteke: " + name);
            Console.Write("ID: ");
            for (int i = 0; i < _id.Length; i++)
                Console.Write((char)_id[i]);
            Console.WriteLine("\nPutanja: " + path + name);
            Console.WriteLine("Datum i vrijeme kreiranja: " + GetDateCreated(name, path));
            Console.WriteLine("Velicina: " + size + "b");
            Console.WriteLine("Broj blokova: {0}", Math.Ceiling((double)size/(double)SIZE_OF_BLOCK));
            Console.WriteLine("Pocetna lokacija blokova: {0}", start);
        }

        /// <summary>
        /// Funkcija odgovara komandi "mv", premješta fajl na novu putanju
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newPath"></param>
        /// <param name="path"></param>
        private void MoveFile(string name, string newPath, string path = "root/")
        {
            //Ako se zeli premjestiti datoteka koja je na putanji root/folder/datoteka, funkcija ce se pozivati sa putanje root/folder
            if (!newPath.EndsWith("/"))
            {
                newPath += "/";
            }
            if(!path.EndsWith("/"))
            {
                path += "/";
            }

            if (!Exists(name, path)) //Ako neko pokusa premjestiti datoteku koja ne postoji na trenutnoj putanji
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            if (!path.Equals("root/")) //Potrebno je provjeriti putanju na koju korisnik želi da premjesti fajl
            {
                //Ako korisnik želi da premjesti fajl npr. na putanju root/folder1 potrebno je provjeriti da li postoji folder1
                string tmpPath, tmpName;
                (tmpPath, tmpName) = SplitPath(path.Substring(0, path.Length - 1)); //pošto putanja završava sa / trebam izbaciti taj znak
                if (tmpName == "" || tmpPath == "")
                {
                    Console.WriteLine("Greska - unesena je nepostojeca putanja.");
                    return;
                }
                if (!Exists(tmpName, tmpPath))
                {
                    Console.WriteLine("Greska - pokusavate dodati fajl {0} u folder {1} koji ne postoji na fajl sistemu.", name, tmpName);
                    return;
                }
            }

            //Provjera da li na novoj putanji vec postoji datoteka sa istim nazivom
            if (Exists(name, newPath))
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

            Update();
        }

        /// <summary>
        /// Komanda "rename" - promjena naziva fajla/foldera
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newName"></param>
        /// <param name="path"></param>
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

            newName = CheckName(newName, path, GetType(name, path));

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

            Update();
        }

        /// <summary>
        /// Funkcija odgovara komandi ls, ispisuje sadržaj foldera u kom je korisnik trenutno pozicioniran
        /// </summary>
        /// <param name="path"></param>
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

        /// <summary>
        /// Funkcija odgovara komandi "cd" i predstavlja rad programa dok je korisnik pozicioniran u neki folder na fajl sistemu
        /// </summary>
        /// <param name="name"></param>
        private void OpenDirectory(string name)
        {
            string currentPath = "root/" + name;

            bool flag = true;

            if (!Exists(name, "root/"))
                flag = false;
            if (GetType(name, "root/") != 'd')
                flag = false;

            if (!flag)
            {
                Console.WriteLine("Ne postoji direktorijum sa datim nazivom");
                return;
            }
            else
            {
                while(true)
                {
                    Console.Write('\n' + currentPath.Replace("root", "HOME") + ">");

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

                        case "ls":
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

                        case "cd":
                            {
                                if(words.Count() == 2 && words[1] != "..")
                                {
                                    Console.WriteLine("Greska - za pozicioniranje u root folder unesite \"cd ..\"");
                                }
                                else if (words.Count() == 2 && words[1] == "..")
                                {
                                    return; //Izlazak iz trenutno otvorenog foldera, izlazi se iz ove funkcije i vraća se u funkciju ExecuteFileSystem jer je OpenDirectory iz nje pozvana
                                }
                                else
                                {
                                    Console.WriteLine("Greska - Netacan unos! Unesite \"help cd\" za pomoc");
                                }
                                break;
                            }

                        case "help":
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

                        case "df":
                            {
                                if (words.Count() == 1) //df mi je samo za slobodan memorijski prostor
                                {
                                    GetRootInformation(false);
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

                        case "create":
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
                             
                        case "rename":
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

                        case "mv":
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

        /// <summary>
        /// Pomoćna funkcija koja za unesenu putanju vraća naziv fajla/foldera i putanju na kom se taj fajl/folder nalazi
        /// </summary>
        /// <param name="pathToCheck"></param>
        /// <returns></returns>
        private (string path, string name) SplitPath(string pathToCheck)
        {
            string path = "", name = "";
            if(pathToCheck.StartsWith("root/")) //svaka validna putanja pocinje sa root
            {
                string[] parts = pathToCheck.Split('/');
                if(parts.Count() == 2) //ako je unesena putanja bila root/naziv, 
                {
                    path = parts[0];
                    name = parts[1];
                }
                else if(parts.Count() == 3)
                {
                    path = parts[0] + "/" + parts[1];
                    name = parts[2];
                }
            }
            return (path, name);
        }

        /// <summary>
        /// Funkcija odgovara komandi "get"
        /// Funkcija kreira fajl na fajl sistemu racunara na osnovu fajla koji se nalazi na ovom fajl sistemu
        /// </summary>
        private void GetFile()
        {
            Console.WriteLine("Unesite putanju ulaznog fajla(primjer: root/folder/datoteka):");
            string inputPath = Console.ReadLine();
            Console.WriteLine(@"Unesite putanju izlaznog fajla(primjer: C:\\Users\\Tatjana Tomic\\Desktop\\dat.txt):");
            string outputPath = Console.ReadLine();

            string iPath, iFile;
            (iPath, iFile) = SplitPath(inputPath);
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

            try
            {
                File.WriteAllBytes(outputPath, contentOfFile);
            }
            catch(Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom upisivanja u fajl {0}", outputPath);
                Console.WriteLine(e.Message);
            }
            
        }

        /// <summary>
        /// Funkcija odgovara komandi "put"
        /// Omogućava dodavanje fajla sa fajl sistema računara na ovaj fajl sistem
        /// </summary>
        private void PutFile()
        {
            Console.WriteLine(@"Unesite putanju ulaznog fajla(primjer: C:\\Users\\Tatjana Tomic\\Desktop\\dat.txt):");
            string inputPath = Console.ReadLine();
            Console.WriteLine("Unesite putanju izlaznog fajla(primjer: root/folder/datoteka):");
            string outputPath = Console.ReadLine();

            if (!File.Exists(inputPath))
            {
                Console.WriteLine("Greska - fajl {0} ne postoji.", inputPath);
                return;
            }

            string oPath, oFile;
            (oPath, oFile) = SplitPath(outputPath);
            if (oPath == "" && oFile == "")
            {
                Console.WriteLine("Greska - unesena je nevazeca putanja {0}", outputPath);
                return;
            }

            if (Exists(oFile, oPath))
            {
                Console.WriteLine("Greska - fajl {0} vec postoji na fajl sistemu", outputPath);
                return;
            }

            if(oPath != "root")
            {
                string tmpPath, tmpName;
                (tmpPath, tmpName) = SplitPath(oPath);
                if(!Exists(tmpName, tmpPath))
                {
                    Console.WriteLine("Greska - pokusavate dodatati fajl {0} u folder {1} koji ne postoji na fajl sistemu.", oFile, tmpName);
                    return;
                }
            }

            oFile = CheckName(oFile, oPath, 'f');

            List<byte> list = new List<byte>();
            try
            {
                byte[] content = File.ReadAllBytes(inputPath);
                list = content.ToList();
            }
            catch(Exception e)
            {
                Console.WriteLine("Doslo je do greske prilikom otvaranja fajla {0}", inputPath);
                Console.WriteLine(e.Message);
                return;
            }

            byte[] _contentOfInputFile = list.ToArray();

            if (_contentOfInputFile.Count() > MAX_SIZE_OF_FILE)
            {
                Console.WriteLine("Greska - najveca dozvoljena velicina fajla je 64kB({0}b)", MAX_SIZE_OF_FILE);
                return;
            }

            CreateFile(oFile, oPath);
            byte[] _id = GetFileID(oFile, oPath);

            if(!WriteToDataSegment(_id, _contentOfInputFile))
            {
                Console.WriteLine("Greska - nije moguce dovrsiti radnju jer nema dovoljno memorije na fajl sistemu");
                return;
            }

            UpdateSizeOfFile(oFile, oPath, _contentOfInputFile.Length);

            Update();
        }

        /// <summary>
        /// Funkcija predstavlja komandu "cp"
        /// Funkcija kreira kopiju datoteke
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        private void CopyFile(string name, string path = "root/")
        {
            if (!Exists(name, path))
            {
                Console.WriteLine("Greska - ne postoji datoteka sa nazivom {0} na trenutnoj putanji", name);
                return;
            }

            string newName = "";
            string[] partsOfName = name.Split('.'); //ako je npr. name = naziv.txt => partsOfName[0] = naziv, partsOfName[1] = txt
            for(int i=1; i<1000; i++) //valjda niko nece praviti vise od 1000 kopija jedne datoteke :'D
            {
                newName = partsOfName[0] + "-copy" + i.ToString() + "." + partsOfName[1]; //novi naziv ce biti u obliku naziv-copyI.txt 
                if (!Exists(newName, path))
                    break;
            }

            CreateFile(newName, path);

            if(GetSizeOfFile(name,path) != 0)
            {
                byte[] contentOfFile = ReadFromDataSegment(GetFileID(name, path)); //procitam sadrzaj originalnog fajla - name

                if(!WriteToDataSegment(GetFileID(newName, path), contentOfFile)) //upisem u kopiju - newName
                {
                    Console.WriteLine("Greska - nije moguce dovrsiti radnju jer nema dovoljno memorije na fajl sistemu.");
                    return;
                }

                UpdateSizeOfFile(newName, path, contentOfFile.Length);
            }
        }

        /// <summary>
        /// Pomoćna funkcija koja za dati fajl/folder na datoj putanji vraća tip
        /// 'd' ako je direktorijum, 'f' ako je fajl
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private char GetType(string name, string path)
        {
            if (!path.EndsWith("/"))
                path += "/";

            char type = 'x'; //u slucaju da ne pronadje trazini fajl, vratice x
            StreamReader reader = new StreamReader(new FileStream("FileSystem.bin", FileMode.Open));
            string line = reader.ReadLine();
            while (!line.Contains(separator))
            {
                if (line.Contains("~" + path + name + "~"))
                {
                    type = line[0];
                    break;
                }
                line = reader.ReadLine();
            }
            reader.Close();
            return type;
        }
        
        /// <summary>
        /// Funkcija koja odgovara komandama "df" i "df -h"
        /// Ako je proslijeđeni parametar true znači da je zadana komanda df -h
        /// Ako je proslijeđeni parametar false znači da je zadana komanda df i potrebno je samo ispisati slobodan memorijski prostor
        /// </summary>
        /// <param name="flag"></param>
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
        
        /// <summary>
        /// Funkcija se poziva prilikom dodavanja novog fajla/foldera ili promjene naziva i provjerava da li je naziv u odgovarajućem formatu
        /// Funkcija se izvršava dok korisnik ne unese validan naziv
        /// </summary>
        /// <param name="name">novi naziv fajla/foldera</param>
        /// <param name="path">putanja na kojoj se dodaje fajl/folder</param>
        /// <param name="type">type='f' ako je name naziv fajla, type='d' ako je name naziv direktorijuma</param>
        /// <returns></returns>
        private string CheckName(string name, string path, char type)
        {
            while(true)
            {
                Match match;
                if(type == 'd')
                    match = (new Regex(@"[A-Za-z0-9-]+")).Match(name);
                else
                    match = (new Regex(@"[A-Za-z0-9-]+\.[A-Za-z0-9]+")).Match(name);

                bool exists = Exists(name, path); 

                if (name.Length <= 20 && !exists && match.Value == name)
                {
                    return name;
                }
                else if(name.Length > 20)
                {
                    Console.WriteLine("Greska - naziv je predugacak.");
                    Console.WriteLine("Unesite novi naziv");
                    name = Console.ReadLine();
                }
                else if(match.Value != name)
                {
                    Console.WriteLine("Greska - naziv datoteke ili direktorijuma moze sadrzati velika i mala slova, brojeve i \"-\". Naziv datoteke unesite u formatu naziv.ekstenzija.");
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
        
        /// <summary>
        /// Funkcija provjerava da li na zadatoj putanji postoji fajl/folder sa datim nazivom
        /// Funkcija čita liniju po liniju datoteke FileSystem.bin dok ne dodje do separatora i provjerava da li postoji traženi fajl/folder
        /// </summary>
        /// <param name="name">naziv fajla/foldera</param>
        /// <param name="path">putanja na kojoj bi se trebao nalaziti</param>
        /// <returns>Funkcija vraća tru/false u zavisnosti od toga da li postoji fajl/folder</returns>
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
        }
        
        /// <summary>
        /// Funkcija dodjeljuje ID novom fajlu/folderu
        /// Funkcija čita posljednji upisan ID u ListOfIdentificators.txt fajlu(ako fajl postoji), vraća cijeli broj za 1 veći i upisuje njegovu vrijednost u fajl
        /// Ako fajl ne postoji, funkcija ga kreira i vraća broj 100 kao ID
        /// </summary>
        /// <returns></returns>
        private int GetNewId()
        {
            StreamReader reader = new StreamReader(new FileStream("ListOfIdentificators.txt", FileMode.OpenOrCreate));
            string _lastId = reader.ReadLine();
            string content = reader.ReadToEnd();
            reader.Close();

            int lastId;
            if(_lastId != null)
            {
                lastId = Convert.ToInt32(_lastId);
            }
            else
            {
                lastId = 100;
            }

            int newId = lastId + 1;

            StreamWriter writer = new StreamWriter(new FileStream("ListOfIdentificators.txt", FileMode.OpenOrCreate));
            writer.WriteLine(newId);
            writer.WriteLine(_lastId);
            writer.WriteLine(content);
            writer.Close();

            return newId;
        }
        
        /// <summary>
        /// Funkcija predstavlja uputstvo za korišćenje programa, ispisuje listu svih komandi i detaljan opis svake komande.
        /// </summary>
        /// <param name="method"></param>
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
    }
}