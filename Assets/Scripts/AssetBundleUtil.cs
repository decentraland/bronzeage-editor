using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AssetBundleUtil : MonoBehaviour 
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
    /// Load an asset bundle in async
    /// </summary>
    /// <param name="data">The byte array</param>
    public static IEnumerator GetBundleFromBytesAsync(byte[] data, System.Action<AssetBundle> output)
    {
        var request = AssetBundle.LoadFromMemoryAsync(data);
        yield return request;

        output(request.assetBundle);
    }

    /// <summary>
    /// Load an asset bundle
    /// </summary>
    /// <param name="data">The byte array</param>
    public static AssetBundle GetBundleFromBytes(byte[] data)
    {
        return AssetBundle.LoadFromMemory(data);
    }

    /// <summary>
    /// Load the main asset from the bundle
    /// </summary>
    /// <typeparam name="T">Asset type</typeparam>
    /// <param name="bundle">Bundle</param>
    public static T LoadAssetBundle<T>(AssetBundle bundle, HideFlags flags = HideFlags.None) where T : Object
    {
        var obj = bundle.LoadAllAssets<T>()[0];
        obj.hideFlags = flags;
        return obj;
    }

    /// <summary>
    /// Load the main asset from the bundle in async
    /// </summary>
    /// <typeparam name="T">Asset type</typeparam>
    /// <param name="bundle">Bundle</param>
    public static IEnumerator LoadAssetBundleAsync<T>(AssetBundle bundle, System.Action<Object> output, HideFlags flags = HideFlags.None) where T : Object
    {
        var request = bundle.LoadAllAssetsAsync<T>();
        yield return request;

        var obj = request.allAssets[0];
        obj.hideFlags = flags;
        output(obj);
    }

    #endregion Methods

    #region Non Public Methods

    #endregion Methods
}