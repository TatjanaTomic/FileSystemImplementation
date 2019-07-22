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

//TASKS:
//help - DONE
//df - DONE
//cd
//mkdir - uradjeno ali samo za root
//create
//put
//get
//ls - uradjeno za root
//cp
//mv
//rename
//echo
//cat
//rm
//stat