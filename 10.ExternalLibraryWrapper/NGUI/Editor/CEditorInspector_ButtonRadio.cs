using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if NGUI

[CanEditMultipleObjects]
[CustomEditor( typeof( CUIButtonRadio ) )]
public class CEditorInspector_ButtonRadio : Editor
{
	private List<CUIButtonRadio> _listButtonSibling = new List<CUIButtonRadio>();

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		CUIButtonRadio pTarget = target as CUIButtonRadio;
		
		GUILayout.Label( string.Format( "Radio Group : {0}", pTarget._iRadioGroup ) );
		GUILayout.Label( string.Format( "Radio Index : {0}", pTarget._iRadioIndex ));
	}

	private void OnEnable()
	{
		if (Application.isPlaying) return;

		_listButtonSibling.Clear();
		CUIButtonRadio pTarget = target as CUIButtonRadio;

		GameObject pObject_Parents = pTarget.transform.parent.gameObject;
		pObject_Parents.GetComponentsInChildren<CUIButtonRadio>( _listButtonSibling );
		_listButtonSibling.Sort( Comparer_BySiblingIndex );

		for (int i = 0; i < _listButtonSibling.Count; i++)
		{
			_listButtonSibling[i]._iRadioGroup = pObject_Parents.GetInstanceID();
			_listButtonSibling[i]._iRadioIndex = i;
		}
	}

	private int Comparer_BySiblingIndex( CUIButtonRadio pObjectX, CUIButtonRadio pObjectY )
	{
		int iSiblingIndexX = pObjectX.transform.GetSiblingIndex();
		int iSiblingIndexY = pObjectY.transform.GetSiblingIndex();

		if (pObjectX.transform.parent != pObjectY.transform.parent)
		{
			iSiblingIndexX = pObjectX.transform.parent.GetSiblingIndex();
			iSiblingIndexY = pObjectY.transform.parent.GetSiblingIndex();
		}

		if (iSiblingIndexX < iSiblingIndexY)
			return -1;
		else if (iSiblingIndexX > iSiblingIndexY)
			return 1;
		else
			return 0;
	}
}
#endif