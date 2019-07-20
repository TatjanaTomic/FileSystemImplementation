using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemImplementation
{
    class Directory
    {
        public string directoryName;
        public readonly int directoryId;
        public string directoryPath;
        public int directoryDepth;
        public DateTime dirDateCreated;
        public DateTime dirLastTimeModified;
        public DateTime dirLastTimeOpened;

        public Directory(string name, int id,  DateTime dateC, DateTime dateM, DateTime dateO, string path = "\root", int depth = 1)
        {
            directoryName = name;
            directoryId = id;
            directoryPath = path;
            directoryDepth = depth;
            dirDateCreated = dateC;
            dirLastTimeModified = dateM;
            dirLastTimeOpened = dateO;
        }

        public void WriteToFile()
        {
            throw new NotImplementedException();
        }
    }
}
