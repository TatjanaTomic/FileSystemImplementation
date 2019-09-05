using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{

    //TODO: TREBA DODATI AZURIRANJE SADRZAJA DATOTEKE U MFT-u
    class Program
    { 
        static void Main(string[] args)
        {
            try
            {
                new FileSystem().ExecuteFileSystem();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}

//TASKS:
//help - DONE
//df - DONE
//cd - DONE
//mkdir - DONE
//create - DONE
//put
//get
//ls - DONE
//cp
//mv - DONE
//- DONE
//echo
//cat
//rm
//stat