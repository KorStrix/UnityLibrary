using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class CEditorProjectView_CSV_To_Json : Editor
{
	static private LinkedList<DirectoryInfo> _listDirectoryInfo = new LinkedList<DirectoryInfo>();
	static private StringBuilder _pStrBuilder = new StringBuilder();

	//[MenuItem( "Assets/StrixTool/CSV Format Ansi To UTF-8", false, 0 )]
	static public void DoConvertCSV_Ansi_To_UTF_8()
	{
		if (Selection.objects == null || Selection.objects.Length != 1)
		{
			Debug.LogWarning( "한개의 폴더만 선택 후 다시 눌러주세요" );
			return;
		}

		string strFolderPath = AssetDatabase.GetAssetPath( Selection.objects[0] );
		DirectoryInfo pDirectory = new DirectoryInfo( strFolderPath );
		FileInfo[] arrAllFileInFolder = pDirectory.GetFiles();

		Encoding pEncoding_UTF8 = Encoding.UTF8;
		for (int i = 0; i < arrAllFileInFolder.Length; i++)
		{
			FileInfo pFileInfo = arrAllFileInFolder[i];
			if ((pFileInfo.Extension.Contains( ".CSV" ) ||
				pFileInfo.Extension.Contains( ".csv" )) == false) continue;

			string strPath = pFileInfo.FullName;
			Encoding pEncodingCurrent = GetEncoding( strPath );

			if (pEncodingCurrent == pEncoding_UTF8) continue;

			// .net framework 3.5에서는 됐었던 버젼, 근데 .net 올리면서 안된다..
			// using (StreamReader pStreamReader = new StreamReader( strPath, Encoding.Default, true ))

			// 참조 링크 http://www.solarview.net/archives/94
			using (StreamReader pStreamReader = new StreamReader( strPath, System.Text.Encoding.GetEncoding( 949 ) ))
			{
				string strText = pStreamReader.ReadToEnd();
				pStreamReader.Close();

				using (StreamWriter pFileStream = new StreamWriter( strPath, false, pEncoding_UTF8 ))
				{
					pFileStream.Write( strText );
					pFileStream.Flush();
					pFileStream.Close();
				}
			}
		}
	}

	[MenuItem( "Assets/StrixTool/CSV To Json In Folder", false, 0 )]
	static public void DoConvertCSV_TO_Json()
	{
		if (Selection.objects == null || Selection.objects.Length != 1)
		{
			Debug.LogWarning( "한개의 폴더만 선택 후 다시 눌러주세요" );
			return;
		}

		DoConvertCSV_Ansi_To_UTF_8();
		AssetDatabase.Refresh( ImportAssetOptions.ForceSynchronousImport );

		_listDirectoryInfo.Clear();
		string strFolderPath = AssetDatabase.GetAssetPath( Selection.objects[0] );
		List<TextAsset> listText = new List<TextAsset>();

		DirectoryInfo pDirectory = new DirectoryInfo( strFolderPath );
		ProcAddText_InDirectory( pDirectory, listText );

		_listDirectoryInfo.AddRange_Last( pDirectory.GetDirectories() );
		while (_listDirectoryInfo.Count != 0)
		{
			DirectoryInfo pDirectoryChild = _listDirectoryInfo.First.Value;
			_listDirectoryInfo.RemoveFirst();
			_listDirectoryInfo.AddRange_Last( pDirectoryChild.GetDirectories() );

			ProcAddText_InDirectory( pDirectoryChild, listText );
		}

		string strFolderPath_CopyTo = Application.dataPath + "/Resources/" + SCManagerParserJson.const_strFolderName;
		if (System.IO.File.Exists( strFolderPath_CopyTo ) == false)
			System.IO.Directory.CreateDirectory( strFolderPath_CopyTo );

		for (int i = 0; i < listText.Count; i++)
		{
			TextAsset pCSVText = listText[i];
			if ((pCSVText.name.Contains( "CSV" ) == false ||
				pCSVText.name.Contains( "csv" ) == false) == false) continue;

			string strCSVText = System.Text.Encoding.UTF8.GetString( pCSVText.bytes );
			string strJson = ConvertCSV_To_Json( strCSVText );
			Debug.Log( pCSVText.name + " Json : " + strJson );

			string strFilePath = ExtractLocalFilePath( pCSVText.name, strFolderPath_CopyTo );
			using (StreamWriter sw = new StreamWriter( File.Open( strFilePath, FileMode.Create ), Encoding.UTF8 ))
			{
				sw.Write( strJson );
				sw.Close();
			}
		}

		AssetDatabase.Refresh( ImportAssetOptions.ForceUpdate );
	}




	static private void ProcAddText_InDirectory( DirectoryInfo pDirectory, List<TextAsset> listText )
	{
		FileInfo[] arrAllFileInFolder = pDirectory.GetFiles();
		for (int j = 0; j < arrAllFileInFolder.Length; j++)
		{
			string strRelativeFilePath = ConvertRelativePath( arrAllFileInFolder[j].FullName );
			TextAsset pText = AssetDatabase.LoadAssetAtPath<TextAsset>( strRelativeFilePath );
			if (pText == null) continue;

			listText.Add( pText );
		}
	}

	static private string ConvertCSV_To_Json( string strCSV )
	{
		string[] arrCSVRow = strCSV.Split( new string[] { "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries );
		string[] arrJsonHeader = arrCSVRow[0].Split( ',' );

		_pStrBuilder.Length = 0;
		_pStrBuilder.Append( "{ \"array\": [ " );

		// Convert Json 시작
		// 1번째줄은 Header 줄이므로 생략
		for (int i = 1; i < arrCSVRow.Length; i++)
		{
			string[] arrRowCurrent = arrCSVRow[i].Split( ',' );
			ProcWrite_Brace( _pStrBuilder, true );
			for (int j = 0; j < arrJsonHeader.Length; j++)
			{
				ProcWriteJsonField( _pStrBuilder, arrJsonHeader[j], arrRowCurrent[j] );
				if (j != arrJsonHeader.Length - 1)
					_pStrBuilder.Append( "," );
			}
			ProcWrite_Brace( _pStrBuilder, false );

			if (i != arrCSVRow.Length - 1)
				_pStrBuilder.Append( "," );
		}
		_pStrBuilder.Append( "] }" );

		return _pStrBuilder.ToString();
	}

	static private void ProcWriteJsonField( StringBuilder pStrBuilder, string strFieldName, string strField )
	{
		bool bIsNotInclude_DoubleQuote_FieldName = strFieldName.IndexOf( "\"" ) == -1;
		bool bIsNotInclude_DoubleQuote_strField = strField.IndexOf( "\"" ) == -1;

		if (bIsNotInclude_DoubleQuote_FieldName)
			pStrBuilder.Append( "\"" );
		pStrBuilder.Append( strFieldName );
		if (bIsNotInclude_DoubleQuote_FieldName)
			pStrBuilder.Append( "\"" );

		pStrBuilder.Append( " : " );

		if (bIsNotInclude_DoubleQuote_strField)
			pStrBuilder.Append( "\"" );
		pStrBuilder.Append( strField );
		if (bIsNotInclude_DoubleQuote_strField)
			pStrBuilder.Append( "\"" );
	}

	static private string ConvertRelativePath( string strPath )
	{
		return "Assets" + strPath.Substring( Application.dataPath.Length );
	}

	static private void ProcWrite_Brace( StringBuilder pStrBuilder, bool bIsOpen )
	{
		if (bIsOpen)
			pStrBuilder.Append( "{" );
		else
			pStrBuilder.Append( "}" );
	}

	static private string ExtractLocalFilePath( string strFileName, string strFolderPath )
	{
		_pStrBuilder.Length = 0;
		_pStrBuilder.Append( strFolderPath );
		_pStrBuilder.Append( "/" );

		_pStrBuilder.Append( strFileName.ToString() );
		_pStrBuilder.Append( ".json" );

		return _pStrBuilder.ToString();
	}

	// .net 기본 Get Encoding 관련은 다 실패율이 높다..
	// 따라서 외부 소스 퍼옴
	// https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
	static public Encoding GetEncoding( string filename )
	{
		// Read the BOM
		var bom = new byte[4];
		using (var file = new FileStream( filename, FileMode.Open, FileAccess.Read ))
		{
			file.Read( bom, 0, 4 );
			file.Close();
		}

		// Analyze the BOM
		if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
		if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
		if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
		if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
		if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
		return Encoding.ASCII;
	}
}
#endif