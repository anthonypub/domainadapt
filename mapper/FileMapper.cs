using System;
using System.Collections.Generic;
using System.IO;

namespace mapper
{
    public class FileMap
    {
        //The directory is basically just a map from id <-> file path
        public Dictionary<UInt32, string> FileId2Path = new Dictionary<UInt32, string>();

        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var kvp in FileId2Path)
                {
                    sw.WriteLine($"{kvp.Key}\t{kvp.Value}");
                }
            }
        }

        public void Load(string path)
        {
            if (FileId2Path.Count > 0)
            {
                throw new InvalidOperationException("Can't load nonempty map");
            }
            FileId2Path = new Dictionary<UInt32, string>();
            foreach (string currLine in File.ReadLines(path))
            {
                string[] fields = currLine.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (fields.Length != 2)
                {
                    throw new InvalidDataException($"Line was not in expected key\tvalue format: {currLine}");
                }
                UInt32 fileId = UInt32.Parse(fields[0]);
                FileId2Path.Add(fileId, fields[1]);
            }
        }
    }

    public class FileMapper
    {

        //Extract the corresponding file lines, in order, and write to file
        public static void GetLinesFromFileSet(Int64[] findMe, string outPath, string fileMapPath)
        {

            //Not sure what the best way of solving tihs problem is.
            //We could work in chunks, allocating say the first GB of sentences
            //in a giant array, figuring out all of the files we need to open
            //and all fo the sentences we need from them at once, then just
            //walking through each file and pulling each required sentence to its corresponding
            //slot.

            //Or we could have some kind of LRU cache and just walk through the
            //list, loading in files opporunistically as we go.

            //A large out-of-domain corpus will contain say 100M sentence pairs.
            //If a sentence is 10 words and a word is 5 letters, that gives us 
            //60 characters / sentence. Assuming UTF-8, for euro languages that's
            //60 * 100,000,000 bytes, or 6 GB.

            //Actually I guess that all fits nicely into memory and I shouldn't worry about it.

            //So for now I"ll just read in the whole corpus and dump as I go

            FileMap map = new FileMap();
            map.Load(fileMapPath);

            Dictionary<UInt32, string[]> sentCache =
                new Dictionary<UInt32, string[]>();

            using (StreamWriter sw = new StreamWriter(outPath))
            {
                foreach (Int64 currId in findMe)
                {
                    UInt32 fileID = (UInt32)(currId >> 32);
                    UInt32 sentID = (UInt32)(currId & 0xffffffff);

                    if (!sentCache.ContainsKey((UInt32)fileID))
                    {
                        string filePath = map.FileId2Path[fileID];
                        sentCache.Add(fileID, File.ReadAllLines(filePath));
                    }
                    sw.WriteLine(sentCache[fileID][sentID]);
                }
            }
        }
        //Given a set of input files, write out a concatenation of all of the lines
        //to a single file, each line preceded by an ID indicating which file / line
        //it comes from.
        //Also write out a dictionary mapping file ids to paths
        public static void MapFileSet(string[] inputFilesPaths, string concatFilePath, string fileMapPath)
        {
            using (StreamWriter sw = new StreamWriter(concatFilePath))
            {
                FileMap fm = new FileMap();
                for (UInt32 currFileIdx = 0; currFileIdx < inputFilesPaths.Length; ++currFileIdx)
                {
                    fm.FileId2Path.Add(currFileIdx, inputFilesPaths[currFileIdx]);
                    Int32 currLineNum = 0;
                    Console.WriteLine($"Working on {inputFilesPaths[currFileIdx]}");
                    Console.WriteLine(File.ReadAllLines(inputFilesPaths[currFileIdx]));
                    foreach (string currLine in File.ReadLines(inputFilesPaths[currFileIdx]))
                    {
                        if (currLine.IndexOf('\t') != -1)
                        {
                            throw new InvalidDataException("Tabs are not allowed");
                        }
                        Int64 fullId = currFileIdx;
                        fullId <<= 32;
                        fullId |= (Int64)currLineNum;
                        sw.WriteLine($"{fullId}\t{currLine}");
                        ++currLineNum;
                    }
                }
            }
        }
    }
}