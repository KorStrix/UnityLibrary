using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class CEditorProjectView_SetSortOrder : Editor
{
	static private LinkedList<Transform> _listObject = new LinkedList<Transform>();

	[MenuItem( "GameObject/StrixTool/Set SortOrder/ - 1", false, 0 )]
	static public void DoSetSortOrder_Minus_1() { ProcSetSortOrder( -1 ); }

	[MenuItem( "GameObject/StrixTool/Set SortOrder/ - 10", false, 1 )]
	static public void DoSetSortOrder_Minus_10() { ProcSetSortOrder( -10 ); }
	
	[MenuItem( "GameObject/StrixTool/Set SortOrder/ - 100", false, 2 )]
	static public void DoSetSortOrder_Minus_100() { ProcSetSortOrder( -100 ); }

	[MenuItem( "GameObject/StrixTool/Set SortOrder/ + 1", false, 3 )]
	static public void DoSetSortOrder_Plus_1() { ProcSetSortOrder( 1 ); }

	[MenuItem( "GameObject/StrixTool/Set SortOrder/ + 10", false, 4 )]
	static public void DoSetSortOrder_Plus_10() { ProcSetSortOrder( 10 ); }

	[MenuItem( "GameObject/StrixTool/Set SortOrder/ + 100", false, 5 )]
	static public void DoSetSortOrder_Plus_100() { ProcSetSortOrder( 100 ); }

	static private void ProcSetSortOrder(int iSortOrderOffset)
	{
		if (Init_And_CheckIsReady() == false) return;
		while (_listObject.Count != 0)
		{
			Transform pTransTarget = _listObject.First.Value;
			_listObject.RemoveFirst();
			SetSortOrder( pTransTarget, iSortOrderOffset );
		}
	}


	static private bool Init_And_CheckIsReady()
	{
		_listObject.Clear();
		if (Selection.gameObjects == null)
			return false;

		for (int i = 0; i < Selection.gameObjects.Length; i++)
		{
			Transform[] arrTransform = Selection.gameObjects[i].GetComponentsInChildren<Transform>();
			for(int j = 0; j < arrTransform.Length; j++)
			{
				if (_listObject.Contains( arrTransform[j] ) == false)
					_listObject.AddLast( arrTransform[j] );
			}
		}
		return true;
	}

	static private void SetSortOrder(Transform pObject, int iSortOrderOffset)
	{
		Renderer pRenderer = pObject.GetComponent<Renderer>();
		if (pRenderer != null)
			pRenderer.sortingOrder += iSortOrderOffset;
	}
}
#endif