using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DoomahLevelLoader
{
    public static class Loader
    {
        private static string[] doomahFiles;
        private static int currentFileIndex = 0;

        public static void LoadDoomahFiles()
        {
            try
            {
                string location = Assembly.GetExecutingAssembly().Location;
                string directory = Path.GetDirectoryName(location);
                doomahFiles = Directory.GetFiles(directory, "*.doomah");

                if (doomahFiles.Length == 0)
                {
                    Debug.LogError("No .doomah files found in the directory: " + directory);
                }
                else
                {
                    Array.Sort(doomahFiles); // Sort the files alphabetically
                    currentFileIndex = 0; // Set the current file index to the first file
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error loading .doomah files: " + ex.Message);
            }
        }

        private static bool ValidateFileIndex()
        {
            if (doomahFiles == null || doomahFiles.Length == 0 || currentFileIndex < 0 || currentFileIndex >= doomahFiles.Length)
            {
                Debug.LogError("No .doomah files loaded or invalid index.");
                return false;
            }
            return true;
        }

        public static string GetCurrentDoomahFile()
        {
            if (!ValidateFileIndex())
                return null;

            return doomahFiles[currentFileIndex];
        }

        public static void MoveToNextFile()
        {
            if (!ValidateFileIndex())
                return;

            currentFileIndex = (currentFileIndex + 1) % doomahFiles.Length;
            Debug.Log("Current File Index: " + currentFileIndex);
            Debug.Log("Current File Loaded: " + GetCurrentDoomahFile());
            EnvyandSpiteterimal.Instance?.UpdateLevelName();
        }

        public static void MoveToPreviousFile()
        {
            if (!ValidateFileIndex())
                return;

            currentFileIndex = (currentFileIndex == 0) ? doomahFiles.Length - 1 : currentFileIndex - 1;
            Debug.Log("Current File Index: " + currentFileIndex);
            Debug.Log("Current File Loaded: " + GetCurrentDoomahFile());
            EnvyandSpiteterimal.Instance?.UpdateLevelName();
        }

        public static AssetBundle LoadBundle()
        {
            if (!ValidateFileIndex())
                return null;

            try
            {
                // Load the currently selected .doomah file
                return AssetBundle.LoadFromFile(doomahFiles[currentFileIndex]);
            }
            catch (Exception ex)
            {
                Debug.LogError("Error loading bundle: " + ex.Message);
                return null;
            }
        }

        public static AssetBundle LoadTerminal()
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = "meshwhatever.terminal.bundle";
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Debug.LogError("Resource 'terminal.bundle' not found in embedded resources.");
                        return null;
                    }

                    byte[] buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    return AssetBundle.LoadFromMemory(buffer);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error loading terminal: " + ex.Message);
                return null;
            }
        }

        public static string GetCurrentFileName()
        {
            if (!ValidateFileIndex())
                return null;

            // Get only the file name from the full file path
            return Path.GetFileNameWithoutExtension(doomahFiles[currentFileIndex]);
        }
    }
}
