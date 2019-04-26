using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CEditorProjectView_MaterialGenerator : Editor
{
	[MenuItem("Assets/StrixTool/Generate Material", false, 0)]
	static public void DoGenerateMaterial()
	{
		if (Selection.objects == null)
			return;

		string strPathFolder = AssetDatabase.GetAssetPath(Selection.objects[0]);
		DirectoryInfo pDirectoryFolder = new DirectoryInfo(strPathFolder);
		pDirectoryFolder = pDirectoryFolder.Parent;
		pDirectoryFolder = new DirectoryInfo(pDirectoryFolder.ToString() + "/Material");
		if(pDirectoryFolder.Exists == false)
			pDirectoryFolder.Create();

		strPathFolder = pDirectoryFolder.ToString() + "/";

		for (int i = 0; i < Selection.objects.Length; i++)
		{
			string strPath = AssetDatabase.GetAssetPath(Selection.objects[i]);
			Sprite pSprite = AssetDatabase.LoadAssetAtPath(strPath, typeof(Sprite)) as Sprite;

			if (pSprite == null) continue;

			Shader shader = Shader.Find("Standard");
			Material Material = new Material(shader);
			Material.SetTexture("_MainTex", pSprite.texture);

			string strFilePath = ConvertRelativePath(strPathFolder + pSprite.name + ".mat");
			AssetDatabase.CreateAsset(Material, strFilePath);
			AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
		}
	}

	static private string ConvertRelativePath(string strPath)
	{
		return "Assets" + strPath.Substring(Application.dataPath.Length);
	}
}
#endif