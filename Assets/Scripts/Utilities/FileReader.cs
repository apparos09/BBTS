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


        // Returns the file with its path.
        // If 'useBackSlash' is true, a backslash is put at the end of the file path if one does not exist.
        // If 'useBackSlash' is false, a forward slash is added to the end of the file path if one does not exist.
        public string GetFileWithPath(bool useBackSlash = true)
        {
            // The resulting filepath.
            string result = "";

            // Make sure they're set properly.
            SetFile(file);
            SetFilePath(filePath);

            // Combines the two.
            result = filePath + file;

            return result;
        }

        // Sets the file.
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
            bool result = File.Exists(GetFileWithPath());

            return result;
        }

        // Deletes the file.
        public bool DeleteFile()
        {
            // Checks if the file exists.
            if(FileExists())
            {
                // Deletes the file.
                File.Delete(GetFileWithPath());
                return true;
            }
            else
            {
                return false;
            }
        }

        // Checks if a file path exists.
        public bool FilePathExists()
        {
            bool result = Directory.Exists(filePath);

            return result;
        }

        // Makes the file directory. Returns false if it fails, or if the directory already exists.
        public bool MakeFileDirectory()
        {
            // Checks if the file path already exists.
            if(FilePathExists())
            {
                Debug.LogWarning("File directory already exists.");
                return false;
            }
            else
            {
                // Genrate the directory.
                DirectoryInfo direcInfo = Directory.CreateDirectory(filePath);

                // Checks if it was successful.
                bool result = direcInfo.Exists;
                return result;           
            } 
        }


        // Checks if the file is empty.
        public bool IsFileEmpty()
        {
            // Checks if the file exists.
            bool result = FileExists();

            // File exists, so open it and see if it has data.
            if (result)
            {
                // Opens a reading file stream.
                FileStream fs = File.OpenRead(GetFileWithPath());
                
                // Checks if the file stream is set.
                result =  fs.Length == 0;


                // Close the file stream.
                fs.Close();
            }

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