using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

public class CEditorProjectView_GenerateParents : Editor
{
	static private LinkedList<Transform> _listObject = new LinkedList<Transform>();

    [MenuItem("GameObject/StrixTool/Generate Empty Object Parents #&b", false, 0 )]
	static public void DoGenerateParents_EmptyObject()
	{
		if (Init_And_CheckIsReady() == false) return;

		while (_listObject.Count != 0)
		{
			Transform pTransTarget = _listObject.First.Value;
			_listObject.RemoveFirst();
			ProcGenerateParents( pTransTarget );
		}
	}

    [MenuItem("GameObject/StrixTool/Generate Common Parents #&v", false, 0)]
    static public void DoGenerateParents_CommonParents()
    {
        if (Init_And_CheckIsReady() == false) return;

        Transform pTransformCommonParents = new GameObject("Group_" + _listObject.Count).transform;
        foreach (Transform pTransformChild in _listObject)
            pTransformChild.SetParent(pTransformCommonParents);

    }

    static private bool Init_And_CheckIsReady()
	{
		_listObject.Clear();
		if (Selection.gameObjects == null)
			return false;

		for (int i = 0; i < Selection.gameObjects.Length; i++)
		{
			if (_listObject.Contains( Selection.gameObjects[i].transform ) == false)
				_listObject.AddLast( Selection.gameObjects[i].transform );
		}
		return true;
	}

	static private void ProcGenerateParents( Transform pObject )
	{
		Vector3 vecOriginScale = pObject.localScale;
		GameObject pObjectNewParents = new GameObject( pObject.name );
		pObjectNewParents.transform.SetParent( pObject );
		pObjectNewParents.transform.DoResetTransform();

		pObjectNewParents.transform.SetParent( pObject.parent );
		pObject.SetParent( pObjectNewParents.transform );

		pObjectNewParents.transform.localScale = Vector3.one;
		pObject.localScale = vecOriginScale;
	}
}
#endif