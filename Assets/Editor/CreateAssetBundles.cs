using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;
using System.IO;
using UnityEngine.Networking;
using System;
using UnityEditor;


public class CreateAssetBundles: MonoBehaviour
{

    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        Debug.Log("Building bundled asset");
        try
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            if (!Directory.Exists(Application.streamingAssetsPath))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None,
              EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();

        }
        catch (Exception e)
        {
            Debug.Log(" failed to create bundled assets" + e);
            throw;
        }

    }
}
