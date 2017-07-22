using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class PrefabUtil : MonoBehaviour
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
    /// Create prefabs with current selection
    /// </summary
    public static GameObject[] CreatePrefabs(string path)
    {
        var selectedObjects = Selection.gameObjects;
        var selectedCount = selectedObjects.Length;

        // Check for folder
        var localPath = path;
        if (!DirUtil.CheckForProjectPath(localPath))
            DirUtil.CreateFolderTree(localPath);

        // Fill array of prefabs
        var prefabs = new GameObject[selectedCount];
        for (int i = 0; i < selectedCount; i++)
        {
            var go = selectedObjects[i];
            prefabs[i] = PrefabUtility.CreatePrefab(localPath + go.name + ".prefab", go);
        }

        return prefabs;
    }

    /// <summary>
    /// Create prefabs with current selection
    /// </summary
    public static GameObject CreatePrefabFromActiveGO(string path)
    {
        var go = Selection.activeGameObject;

        // Check for folder
        var localPath = path;
        if (!DirUtil.CheckForProjectPath(localPath))
            DirUtil.CreateFolderTree(localPath);

        // Fill array of prefabs
        var prefab = PrefabUtility.CreatePrefab(localPath + go.name + ".prefab", go);

        return prefab;
    }

    #endregion Methods

    #region Non Public Methods

    #endregion Methods
}