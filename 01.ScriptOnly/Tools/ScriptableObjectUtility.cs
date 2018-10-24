#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-09-13 오전 11:28:43
 *	개요 : http://wiki.unity3d.com/index.php/CreateScriptableObjectAsset
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 
/// </summary>
public static class ScriptableObjectUtility
{
    /// <summary>
    /// Perform a deep Copy of the object. 
    /// https://answers.unity.com/questions/260632/deep-copy-scriptableobject.html
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="pSource">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T Clone<T>(T pSource)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new System.ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (object.ReferenceEquals(pSource, null))
        {
            return default(T);
        }

        IFormatter pFormatter = new BinaryFormatter();
        Stream pStream = new MemoryStream();
        using (pStream)
        {
            pFormatter.Serialize(pStream, pSource);
            pStream.Seek(0, SeekOrigin.Begin);
            return (T)pFormatter.Deserialize(pStream);
        }
    }

    public static void Clone(ScriptableObject pSource, ref ScriptableObject pPasted)
    {
        string strJson = JsonUtility.ToJson(pSource);
        JsonUtility.FromJsonOverwrite(strJson, pPasted);
    }


#if UNITY_EDITOR

    public static string GetCurrentSelectObjectPath()
    {
        string path = UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject)), "");
        }

        return path;
    }

    public static T CreateAsset<T>(T pAsset, string strName, string strPath = "Assets") where T : ScriptableObject
    {
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(strPath + "/" + strName + ".asset");
        AssetDatabase.CreateAsset(pAsset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();

        return pAsset;
    }


    public static T CreateAsset<T>(string strName, string strPath = "Assets") where T : ScriptableObject
    {
        T pAsset = ScriptableObject.CreateInstance<T>();

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(strPath + "/" + strName + ".asset");
        AssetDatabase.CreateAsset(pAsset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();

        return pAsset;
    }
#endif

}