using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleTestEditor : CEditorWindowBase<AssetBundleTestEditor>
{
    static readonly string const_strBundlePathOutput = "Assets/StreamingAssets/AssetBundle";

//#if UNITY_STANDALONE
//    static readonly BuildTarget const_BuildTarget = BuildTarget.StandaloneWindows64;
//#elif UNITY_ANDROID
//    static readonly BuildTarget const_BuildTarget = BuildTarget.Android;
//#endif

    string _strBundleName;

    [MenuItem("Tools/Strix_Tools/AssetBundleTestEditor")]
    public static void ShowWindow()
    {
        GetWindow<AssetBundleTestEditor>("AssetBundleTestEditor");
    }

    protected override void OnGUIWindowEditor()
    {
        base.OnGUIWindowEditor();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("AssetName");
        _strBundleName = EditorGUILayout.TextField(_strBundleName);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("에셋번들 빌드!"))
            ProcBuild_AssetBundle(_strBundleName);
    }

    static void ProcBuild_AssetBundle(string strBundleName)
    {
        // ProcClearFolder(Directory.GetCurrentDirectory() + "/" + const_strBundlePathOutput);

        if(string.IsNullOrEmpty(strBundleName))
        {
            BuildPipeline.BuildAssetBundles(const_strBundlePathOutput, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }
        else
        {
            AssetBundleBuild pAssetBundle = new AssetBundleBuild();
            pAssetBundle.assetBundleName = strBundleName;
            pAssetBundle.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(strBundleName);

            BuildPipeline.BuildAssetBundles(const_strBundlePathOutput, new AssetBundleBuild[] { pAssetBundle }, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
    }

    static void ProcClearFolder(string strPath)
    {
        if (Directory.Exists(strPath) == false)
        {
            Directory.CreateDirectory(strPath);
            Debug.Log("Create Directory " + strPath);
        }
        else
        {
            ProcDeleteAllFile_InDirectory(strPath);
            var arrDirectories = Directory.GetDirectories(strPath);
            foreach (string strDirectoryPath in arrDirectories)
            {
                ProcDeleteAllFile_InDirectory(strDirectoryPath);
                Directory.Delete(strDirectoryPath);
            }
        }
    }

    static void ProcDeleteAllFile_InDirectory(string strPath)
    {
        var arrFiles = Directory.GetFiles(strPath);
        foreach (string strFileName in arrFiles)
            File.Delete(strFileName);
    }

}
