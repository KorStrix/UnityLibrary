using System.Text;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;

public class CEditorProjectView_GetObjectNameList : Editor
{
	[MenuItem("Assets/StrixTool/Get Object Name List Like Enum", false, 0)]
	[MenuItem( "GameObject/StrixTool/Get Object Name List Like Enum", false, 15 )]
	static public void DoGetObjectNameList_Enum()
	{
		if (Selection.objects == null)
			return;
		else
		{
			List<UnityEngine.Object> listObject = new List<UnityEngine.Object>();
			listObject.AddRange(Selection.objects);
			ProcPrintListObject( listObject );
		}
	}

	[MenuItem( "Assets/StrixTool/Get Object Name List Like Enum - In Folder", false, 0 )]
	static public void DoGetObjectNameList_Enum_InFolder()
	{
		if (Selection.objects == null || Selection.objects.Length != 1)
		{
			Debug.LogWarning( "한개의 폴더만 선택 후 다시 눌러주세요" );
			return;
		}

		LinkedList<DirectoryInfo> _listDirectoryInfo = new LinkedList<DirectoryInfo>();
		string strFolderPath = AssetDatabase.GetAssetPath( Selection.objects[0] );
		List<UnityEngine.Object> listObject = new List<UnityEngine.Object>();

		DirectoryInfo pDirectory = new DirectoryInfo( strFolderPath );
		ProcAddUnityObject_InDirectory( pDirectory, listObject );

		_listDirectoryInfo.AddRange_Last( pDirectory.GetDirectories() );
		while (_listDirectoryInfo.Count != 0)
		{
			DirectoryInfo pDirectoryChild = _listDirectoryInfo.First.Value;
			_listDirectoryInfo.RemoveFirst();
			_listDirectoryInfo.AddRange_Last( pDirectoryChild.GetDirectories() );

			ProcAddUnityObject_InDirectory( pDirectoryChild, listObject );
		}

		ProcPrintListObject( listObject );
	}

	[MenuItem( "Assets/StrixTool/Get Object Name List Like by Enter - In Folder", false, 0 )]
	static public void DoGetObjectNameList_Enter_InFolder()
	{
		if (Selection.objects == null || Selection.objects.Length != 1)
		{
			Debug.LogWarning( "한개의 폴더만 선택 후 다시 눌러주세요" );
			return;
		}

		LinkedList<DirectoryInfo> _listDirectoryInfo = new LinkedList<DirectoryInfo>();
		string strFolderPath = AssetDatabase.GetAssetPath( Selection.objects[0] );
		List<UnityEngine.Object> listObject = new List<UnityEngine.Object>();

		DirectoryInfo pDirectory = new DirectoryInfo( strFolderPath );
		ProcAddUnityObject_InDirectory( pDirectory, listObject );

		_listDirectoryInfo.AddRange_Last( pDirectory.GetDirectories() );
		while (_listDirectoryInfo.Count != 0)
		{
			DirectoryInfo pDirectoryChild = _listDirectoryInfo.First.Value;
			_listDirectoryInfo.RemoveFirst();
			_listDirectoryInfo.AddRange_Last( pDirectoryChild.GetDirectories() );

			ProcAddUnityObject_InDirectory( pDirectoryChild, listObject );
		}

		ProcPrintListObject( listObject, false );
    }

	[MenuItem( "Assets/StrixTool/Get Object Name List by Enter", false, 0 )]
	[MenuItem( "GameObject/StrixTool/Get Object Name List by Enter", false, 15 )]
	static public void DoGetObjectNameList_Enter()
	{
		if (Selection.objects == null)
			return;
		else
		{
			List<UnityEngine.Object> listObject = new List<UnityEngine.Object>();
			listObject.AddRange( Selection.objects );
			ProcPrintListObject( listObject, false );
		}
	}

	static private void ProcPrintListObject(List<UnityEngine.Object> listObject, bool bIsEnumStyle = true)
	{
		listObject.Sort( CompareObject_ByName );
		StringBuilder pStrBuilder = new StringBuilder();
		for (int i = 0; i < listObject.Count; i++)
		{
			pStrBuilder.Append( listObject[i].name );
			if(bIsEnumStyle)
				pStrBuilder.Append( "," );
			else
				pStrBuilder.Append( "\n" );
		}

		UnityEngine.Debug.Log( pStrBuilder.ToString() );
	}

	static public int CompareObject_ByName(UnityEngine.Object pObj1, UnityEngine.Object pObj2)
	{
		string strObjName1 = pObj1.name;
		string strObjName2 = pObj2.name;

		return(strObjName1.CompareTo(strObjName2));
	}

	static private void ProcAddUnityObject_InDirectory( DirectoryInfo pDirectory, List<UnityEngine.Object> listObject )
	{
		FileInfo[] arrAllFileInFolder = pDirectory.GetFiles();
		for (int j = 0; j < arrAllFileInFolder.Length; j++)
		{
			string strRelativeFilePath = ConvertRelativePath( arrAllFileInFolder[j].FullName );
			UnityEngine.Object pObject = AssetDatabase.LoadMainAssetAtPath( strRelativeFilePath );
			if (pObject != null)
				listObject.Add( pObject );
		}
	}

	static protected string ConvertRelativePath( string strPath )
	{
		return "Assets" + strPath.Substring( Application.dataPath.Length );
	}
}
#endif