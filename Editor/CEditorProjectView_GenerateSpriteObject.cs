using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class CEditorProjectView_GenerateSpriteObject : Editor
{
	[MenuItem( "Assets/StrixTool/GenerateSpriteObject", false, 0)]
	static public void DoGetObjectNameList_Enum()
	{
		if (Selection.objects == null)
			return;
		else
		{
			for(int i = 0; i < Selection.objects.Length; i++)
			{
				string strPath = AssetDatabase.GetAssetPath( Selection.objects[i] );
				Sprite pSprite = AssetDatabase.LoadAssetAtPath<Sprite>( strPath );
				if (pSprite == null) continue;

				
				//TextureImporter pImporter = TextureImporter.GetAtPath( strPath ) as TextureImporter;
				GameObject pObject = new GameObject( pSprite .name );
				SpriteRenderer pRenderer = pObject.AddComponent<SpriteRenderer>();
				pRenderer.sprite = pSprite;
			}
		}
	}
}
#endif