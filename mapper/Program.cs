using System;
using System.IO;
using System.Linq;

namespace mapper
{
    class Program
    {
        static void Main(string[] args)
        {
            string testInputDir = "/home/anthonyaue/data/brown/brown/processed/cr";
            string[] fileNames = {
                "cr01.proc",  "cr03.proc",  "cr05.proc" 
            };

            string[] fullInputPaths = fileNames.Select(x => Path.Combine(testInputDir, 
            x)).ToArray();

            foreach(var curr in fullInputPaths)
            {
                Console.WriteLine(curr);
            }

            FileMapper.MapFileSet(fullInputPaths, "concat.txt", "filemap");

        }
    }
}
