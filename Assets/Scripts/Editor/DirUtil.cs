using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DirUtil : MonoBehaviour
{
    #region Exposed fields

    #endregion Exposed fields

    #region Internal fields

    #endregion Internal fields

    #region Custom Events

    #endregion Custom Events

    #region Properties

    #endregion Properties

    #region Events methods

    #endregion Events methods

    #region Public Methods

    /// <summary>
    /// Returns the relative to the project folder
    /// </summary>
    public static string GetRelativeProjectPath(string path)
    {
        var index = path.IndexOf("Assets");
        return index >= 0 ? path.Substring(index) : null;
    }

    /// <summary>
    /// Verifies if the path exissts inside the "Assets" folder
    /// </summary>
    public static bool CheckForProjectPath(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// Create a path inside "Assets" folder, path uses forward slashes "/"
    /// </summary>
    public static void CreateFolderTree(string path)
    {
        // Split by folders
        var nodes = new List<string>(path.Split('/'));

        // Check for folders to create
        if (nodes.Count == 0) return;

        // Check for initial node
        if (nodes[0].ToUpper() != "ASSETS") nodes.Insert(0, "Assets");

        // Create path
        var constructedPath = nodes[0];
        for (int i = 1; i < nodes.Count; i++)
        {
            var currNode = nodes[i];
            if (!Directory.Exists(constructedPath + "/" + currNode))
                AssetDatabase.CreateFolder(constructedPath, currNode);

            constructedPath += "/" + currNode;
        }
    }

    /// <summary>
    /// A dir exists_
    /// </summary>
    public static bool Exists(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// Delete a folder
    /// </summary>
    public static void DeleteFolder(string path)
    {
        string[] files = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteFolder(dir);
        }

        Directory.Delete(path, false);
    }

    #endregion Methods

    #region Non Public Methods

    #endregion Methods
}