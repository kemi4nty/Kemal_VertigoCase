using UnityEditor;
using UnityEngine;
using System.IO;

public class CustomImporterTool : EditorWindow
{
    private string targetFolder = "";
    private string externalAssetsFolder = "";
    private string findString = "";
    private string replaceString = "";

    [MenuItem("Tools/Custom Importer Tool")]
    public static void ShowWindow()
    {
        GetWindow<CustomImporterTool>("Custom Importer Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Custom Importer Tool", EditorStyles.boldLabel);

        // Select target folder
        GUILayout.Label("1. Select Target Folder:");
        if (GUILayout.Button("Select Target Folder"))
        {
            targetFolder = EditorUtility.OpenFolderPanel("Select Target Folder", "", "");
        }
        GUILayout.Label("Selected: " + targetFolder);

        // Select external assets folder
        GUILayout.Label("2. Select External Assets Folder:");
        if (GUILayout.Button("Select External Assets Folder"))
        {
            externalAssetsFolder = EditorUtility.OpenFolderPanel("Select External Assets Folder", "", "");
        }
        GUILayout.Label("Selected: " + externalAssetsFolder);

        // Find & Replace strings
        GUILayout.Label("3. Find & Replace Strings:");
        findString = EditorGUILayout.TextField("Find: ", findString);
        replaceString = EditorGUILayout.TextField("Replace: ", replaceString);

        // Import button
        if (GUILayout.Button("Import & Process"))
        {
            ProcessFolders();
        }
    }

    private void ProcessFolders()
    {
        if (string.IsNullOrEmpty(targetFolder) || string.IsNullOrEmpty(externalAssetsFolder))
        {
            Debug.LogError("Please select both Target Folder and External Assets Folder.");
            return;
        }

        string duplicatedFolder = targetFolder + "_Duplicated";
        DirectoryCopy(targetFolder, duplicatedFolder, true);

        string[] files = Directory.GetFiles(duplicatedFolder, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string newFileName = file.Replace(findString, replaceString);
            if (file != newFileName)
            {
                File.Move(file, newFileName);
            }
        }

        string[] externalFiles = Directory.GetFiles(externalAssetsFolder);
        foreach (var externalFile in externalFiles)
        {
            string fileName = Path.GetFileName(externalFile);
            foreach (var duplicatedFile in Directory.GetFiles(duplicatedFolder))
            {
                if (Path.GetFileName(duplicatedFile) == fileName)
                {
                    File.Copy(externalFile, duplicatedFile, true);
                }
            }
        }

        Debug.Log("Import and process completed successfully!");
    }

    private static void DirectoryCopy(string sourceDir, string destDir, bool copySubDirs)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDir);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException("Source directory does not exist: " + sourceDir);
        }

        if (!Directory.Exists(destDir))
        {
            Directory.CreateDirectory(destDir);
        }

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destDir, file.Name);
            file.CopyTo(tempPath, false);
        }

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destDir, subdir.Name);
                DirectoryCopy(subdir.FullName, tempPath, true);
            }
        }
    }
}
