using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateAssetBundles : EditorWindow
{
    [MenuItem("SPITE/Create Level")]
    static void Init()
    {
        CreateAssetBundles window = (CreateAssetBundles)EditorWindow.GetWindow(typeof(CreateAssetBundles));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Create Asset Bundles", EditorStyles.boldLabel);

        if (GUILayout.Button("Create Bundles"))
        {
            CreateDoomahs(); // Changed method name
        }
    }

    static void CreateDoomahs() // Changed method name
    {
        string doomahDirectory = "ExportedDoomahs"; // Changed variable name
        string outputPath = Path.Combine(Application.dataPath, "..", doomahDirectory); // Changed variable name

        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        // Delete any existing doomah files with the same name and extension
        DeleteExistingDoomahs(outputPath); // Changed method name

        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

        // Add .doomah extension to asset doomah files
        AddDoomahExtension(outputPath); // Changed method name

        // Delete ExportedDoomahs.doomah and ExportedDoomahs.manifest.doomah files if they exist
        DeleteDoomahFile(outputPath, "ExportedDoomahs.doomah"); // Changed variable name
        DeleteDoomahFile(outputPath, "ExportedDoomahs.manifest.doomah"); // Changed variable name

        Debug.Log("Asset doomahs created at: " + outputPath); // Changed log message
    }

    static void DeleteExistingDoomahs(string directory) // Changed method name
    {
        string[] doomahFiles = Directory.GetFiles(directory, "*.doomah"); // Changed variable name

        foreach (string filePath in doomahFiles)
        {
            File.Delete(filePath);
        }
    }

    static void AddDoomahExtension(string directory) // Changed method name
    {
        string[] doomahFiles = Directory.GetFiles(directory, "*"); // Changed variable name

        foreach (string filePath in doomahFiles)
        {
            if (!filePath.EndsWith(".doomah"))
            {
                string newFilePath = filePath + ".doomah"; // Changed variable name
                File.Move(filePath, newFilePath);
            }
        }
    }

    static void DeleteDoomahFile(string directory, string fileName) // Changed method name and variable name
    {
        string filePath = Path.Combine(directory, fileName);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
