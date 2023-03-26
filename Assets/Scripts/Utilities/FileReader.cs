/*
 * References:
 * - https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/file-system/how-to-read-from-a-text-file
 * - https://support.unity.com/hc/en-us/articles/115000341143-How-do-I-read-and-write-data-from-a-text-file-
 * - https://docs.microsoft.com/en-us/dotnet/api/system.io.file.exists?view=net-6.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace BBTS
{
    // file reader for excel text file exports.
    // NOTE: if the file is in the Assets folder (or a subfolder of the Assets folder), you can just do it from there.
    public class FileReader
    {
        // file
        public string file = "";

        // file path
        public string filePath = "";

        // the lines from the file.
        public string[] lines;


        // sets the file.
        public void SetFile(string newFile)
        {
            file = newFile;
        }

        // sets the file and the file path.
        public void SetFile(string newFile, string newFilePath, bool useBackSlash = true)
        {
            SetFile(newFile);
            SetFilePath(newFilePath, useBackSlash);
        }

        // sets the file path 9make sure to use back slashes ('\'), not forward slashes.
        // If useBackSlash is set to false, forward slashes are used.
        public void SetFilePath(string newFilePath, bool useBackSlash = true)
        {
            // set new file path.
            filePath = newFilePath;

            // if the file path is not empty.
            if (filePath.Length != 0)
            {
                // if the last character is not a slash, add one.
                if (filePath[filePath.Length - 1] != '\\' && useBackSlash)
                {
                    filePath += "\\";
                }
                else if (filePath[filePath.Length - 1] != '/' && !useBackSlash)
                {
                    filePath += "/";
                }
            }
        }

        // Sets the file path and the file.
        public void SetFilePath(string newFilePath, string newFile)
        {
            SetFilePath(newFilePath);
            SetFile(newFile);
        }

        // Checks if the file exists.
        public bool FileExists()
        {
            // sets the file and file path to make sure they're formatted properly.
            SetFile(file, filePath);

            // returns true if the file exists.
            bool result = File.Exists(filePath + file);

            return result;
        }

        // Read from the file.
        public void ReadFile()
        {
            // checks if the file exists.
            if (!FileExists())
            {
                Debug.LogError("File does not exist.");
                return;
            }

            // TODO: see if this works for clearing the array. C# garbage collection should take care of this?
            lines = null;

            // gets all the lines from the file.
            string f = filePath + file;
            lines = File.ReadAllLines(@f);
        }

    }
}