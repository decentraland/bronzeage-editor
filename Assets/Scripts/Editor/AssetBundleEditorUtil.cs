using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AssetBundleEditorUtil : MonoBehaviour 
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
    /// Creates an asset bundle and return its manifest
    /// </summary>
    /// <param name="prefab">Prefab to create the asset from</param>
    /// <param name="name">Name of the asset bundle</param>
    /// <param name="path">Temporal path</param>
    /// <returns>The asset bundle manifest</returns>
    public static AssetBundleManifest CreateAssetBundle(GameObject prefab, string name, string path)
    {
        return CreateAssetBundle(prefab, name, path, EditorUserBuildSettings.activeBuildTarget);
    }

    /// <summary>
    /// Creates an asset bundle for an specific platform and return its manifest
    /// </summary>
    /// <param name="prefab">Prefab to create the asset from</param>
    /// <param name="name">Name of the asset bundle</param>
    /// <param name="path">Temporal path</param>
    /// <returns>The asset bundle manifest</returns>
    public static AssetBundleManifest CreateAssetBundle(GameObject prefab, string name, string path, BuildTarget platform)
    {
        var assetName = AssetDatabase.GetAssetPath(prefab);

        // Check for folder
        var localPath = path;
        if (!DirUtil.CheckForProjectPath(localPath))
            DirUtil.CreateFolderTree(localPath);

        // Create bundle
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
        buildMap[0] = new AssetBundleBuild();
        buildMap[0].assetBundleName = name;
        buildMap[0].assetNames = new string[] { assetName };
        var manifest = BuildPipeline.BuildAssetBundles(path, buildMap, BuildAssetBundleOptions.ChunkBasedCompression, platform);
        
        return manifest;
    }

    /// <summary>
    /// Return an asset bundle bytes
    /// </summary>
    public static byte[] GetBytesFromManifest(AssetBundleManifest manifest, string path)
    {
        var bundlePath = path + manifest.GetAllAssetBundles()[0];
        var bytes = File.ReadAllBytes(bundlePath);

        return bytes;
    }

    #endregion Methods

    #region Non Public Methods

    #endregion Methods
}