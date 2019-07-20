using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{

    //TODO: Dodati komandu df koja prikazuje trenutno zauzece memorije na disku


    class Program
    { 
        static void Main(string[] args)
        { 
            FileSystem fileSystem = new FileSystem();
            fileSystem.ExecuteFileSystem();
            Console.ReadKey();
        }

    }
}
