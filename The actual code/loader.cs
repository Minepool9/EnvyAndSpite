using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DoomahLevelLoader
{
    public static class Loader
    {
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
    }
}
