using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Reflection;

// 참고 링크
// https://answers.unity.com/questions/682285/editor-script-for-setting-the-sorting-layer-of-an.html
[CustomEditor( typeof( CCompoSortingLayer ) )]
public class CEditorInspector_CCompoSortingLayer : Editor
{
	private string[] _arrSortingLayerNames;
	private CCompoSortingLayer _pTarget;

	public void OnEnable()
	{
		this._pTarget = (CCompoSortingLayer)target;
	}

	public override void OnInspectorGUI()
	{
        //base.OnInspectorGUI();

		System.Type internalEditorUtilityType = typeof( InternalEditorUtility );
		PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty( "sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic );

		int iIndex = 0;
		this._arrSortingLayerNames = (string[])sortingLayersProperty.GetValue( null, new object[0] );
		for (int i = 0; i < _arrSortingLayerNames.Length; i++)
		{
			if (_arrSortingLayerNames[i].Equals( _pTarget.strSortingLayer ))
				iIndex = i;
		}

		iIndex = EditorGUILayout.Popup( "SortLayer", iIndex, _arrSortingLayerNames );
		_pTarget.strSortingLayer = _arrSortingLayerNames[iIndex];
        _pTarget.iSortOrder = EditorGUILayout.IntField("SortOrder", _pTarget.iSortOrder);
        EditorUtility.SetDirty(_pTarget);
    }
}
