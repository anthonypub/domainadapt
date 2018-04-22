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

            string concatFile =  "concat.txt";
            string filemapFile = "filemap";
            string compFile = "comp.txt";

            //Dump to concat
            FileMapper.MapFileSet(fullInputPaths, concatFile, filemapFile);
            
            string[] concatLines = File.ReadAllLines(concatFile);

            Int64[] ids = concatLines.Select(x => 
                Int64.Parse(x.Split(new char[]{'\t'}, StringSplitOptions.RemoveEmptyEntries)[0])
            ).ToArray();

            FileMapper.GetLinesFromFileSet(ids, compFile, filemapFile);

        }
    }
}
