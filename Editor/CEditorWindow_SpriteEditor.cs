using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-04 오후 4:33:17
   Description : 
   Edit Log    : 
   ============================================ */

public class CEditorWindow_SpriteEditor : CEditorWindow
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* private - Variable declaration           */

	private LinkedList<DirectoryInfo> _listDirectoryInfo = new LinkedList<DirectoryInfo>();

	private SpriteMeshType _eSpriteMeshType;
	private string _strSpriteTag;
	private int _iSpriteExtrude;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	[MenuItem( "Tools/Strix_Tools/Multiple Sprite Editor" )]
	public static void ShowWindow()
	{
		GetWindow< CEditorWindow_SpriteEditor>("SpriteEditor");
    }

	protected override void OnGUIWindowEditor()
	{
		base.OnGUIWindowEditor();

		EditorGUILayout.HelpBox( "Project View에서 폴더 한개만 선택해주세요.\n", MessageType.Info );
        Space( 20f );

		BeginHorizontal();
		_eSpriteMeshType = (SpriteMeshType)EditorGUILayout.EnumPopup( "SpriteMeshType", _eSpriteMeshType );
		EndHorizontal();

		BeginHorizontal();
        LabelField( "Sprite Tag" );
		_strSpriteTag = EditorGUILayout.TextField( _strSpriteTag );
		EndHorizontal();

		BeginHorizontal();
        LabelField( "iSprite Extrude" );
		_iSpriteExtrude = EditorGUILayout.IntField( _iSpriteExtrude );
		EndHorizontal();

        Space( 20f );
		if (Button( "폴더 내 모든 Texture 수정하기" ))
		{
			DoEditTextureAll();
		}
	}

	public void DoEditTextureAll()
	{
		if (Selection.objects == null || Selection.objects.Length != 1)
		{
			Debug.LogWarning( "한개의 폴더만 선택 후 다시 눌러주세요" );
			return;
		}

		_listDirectoryInfo.Clear();
		string strFolderPath = AssetDatabase.GetAssetPath( Selection.objects[0] );
		List<TextureImporter> listTexture = new List<TextureImporter>();

		DirectoryInfo pDirectory = new DirectoryInfo( strFolderPath );
		ProcAddTextureImporter_InDirectory( pDirectory, listTexture );

		_listDirectoryInfo.AddRange_Last( pDirectory.GetDirectories() );
		while (_listDirectoryInfo.Count != 0)
		{
			DirectoryInfo pDirectoryChild = _listDirectoryInfo.First.Value;
			_listDirectoryInfo.RemoveFirst();
			_listDirectoryInfo.AddRange_Last( pDirectoryChild.GetDirectories() );

			ProcAddTextureImporter_InDirectory( pDirectoryChild, listTexture );
		}

		for (int i = 0; i < listTexture.Count; i++)
		{
			bool bIsChange = false;

			TextureImporter pImporter = listTexture[i];
			if (pImporter.textureType != TextureImporterType.Sprite)
			{
				pImporter.textureType = TextureImporterType.Sprite;
				bIsChange = true;
			}

			if (pImporter.spritePackingTag.Equals( _strSpriteTag ) == false)
			{
				pImporter.spritePackingTag = _strSpriteTag;
				bIsChange = true;
			}

			if (pImporter.spriteImportMode != SpriteImportMode.Single)
			{
				pImporter.spriteImportMode = SpriteImportMode.Single;
				bIsChange = true;
			}


			bool bIsChangeTextureImporter = false;
			TextureImporterSettings pTextureSettings = new TextureImporterSettings();
			pImporter.ReadTextureSettings( pTextureSettings );

			if(pTextureSettings.spriteMeshType.Equals( _eSpriteMeshType ) == false)
			{
				pTextureSettings.spriteMeshType = _eSpriteMeshType;
				bIsChangeTextureImporter = true;
			}

			if(pTextureSettings.spriteExtrude.Equals( (uint)_iSpriteExtrude ) == false)
			{
				pTextureSettings.spriteExtrude = (uint)_iSpriteExtrude;
				bIsChangeTextureImporter = true;
			}

			if(bIsChangeTextureImporter)
				pImporter.SetTextureSettings( pTextureSettings );

			if (bIsChange || bIsChangeTextureImporter)
			{
				Debug.Log( "Change :" + pImporter.assetPath, pImporter );
				pImporter.SaveAndReimport();
			}
		}

		AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
	}

	private void ProcAddTextureImporter_InDirectory( DirectoryInfo pDirectory, List<TextureImporter> listTexture )
	{
		FileInfo[] arrAllFileInFolder = pDirectory.GetFiles();
		for (int j = 0; j < arrAllFileInFolder.Length; j++)
		{
			string strRelativeFilePath = ConvertRelativePath( arrAllFileInFolder[j].FullName );
			TextureImporter pImporter = TextureImporter.GetAtPath( strRelativeFilePath ) as TextureImporter;
			if(pImporter != null)
				listTexture.Add( pImporter );
		}
	}
}