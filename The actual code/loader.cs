using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DoomahLevelLoader
{
    public static class Loader
    {
        private static string[] doomahFiles;
        private static int currentFileIndex = -1;

        public static void LoadDoomahFiles()
        {
            string location = Assembly.GetExecutingAssembly().Location;
            string directory = Path.GetDirectoryName(location);
            doomahFiles = Directory.GetFiles(directory, "*.doomah");

            if (doomahFiles.Length == 0)
            {
                Debug.LogError("No .doomah files found in the directory: " + directory);
                return;
            }

            Array.Sort(doomahFiles); // Sort the files alphabetically
            currentFileIndex = 0; // Set the current file index to the first file
        }

        public static string GetCurrentDoomahFile()
        {
            if (doomahFiles == null || doomahFiles.Length == 0 || currentFileIndex < 0 || currentFileIndex >= doomahFiles.Length)
            {
                Debug.LogError("No .doomah files loaded or invalid index.");
                return null;
            }

            return doomahFiles[currentFileIndex];
        }

        public static void MoveToNextFile()
        {
            if (doomahFiles == null || doomahFiles.Length == 0)
            {
                Debug.LogError("No .doomah files loaded.");
                return;
            }

            currentFileIndex = (currentFileIndex + 1) % doomahFiles.Length;
		    EnvyandSpiteterimal.Instance.UpdateLevelName();
        }

        public static void MoveToPreviousFile()
        {
            if (doomahFiles == null || doomahFiles.Length == 0)
            {
                Debug.LogError("No .doomah files loaded.");
                return;
            }

            currentFileIndex = (currentFileIndex - 1 + doomahFiles.Length) % doomahFiles.Length;
			EnvyandSpiteterimal.Instance.UpdateLevelName();

        }

        public static AssetBundle LoadBundle()
        {
            if (doomahFiles == null || doomahFiles.Length == 0 || currentFileIndex < 0 || currentFileIndex >= doomahFiles.Length)
            {
                Debug.LogError("No .doomah files loaded or invalid index.");
                return null;
            }

            // Load the currently selected .doomah file
            return AssetBundle.LoadFromFile(doomahFiles[currentFileIndex]);
        }

        public static AssetBundle LoadTerminal()
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
		
		public static string GetCurrentFileName()
		{
			if (doomahFiles == null || doomahFiles.Length == 0 || currentFileIndex < 0 || currentFileIndex >= doomahFiles.Length)
			{
				Debug.LogError("No .doomah files loaded or invalid index.");
				return null;
			}

			// Get only the file name from the full file path
			return Path.GetFileNameWithoutExtension(doomahFiles[currentFileIndex]);
		}

    }
}
