using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
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
