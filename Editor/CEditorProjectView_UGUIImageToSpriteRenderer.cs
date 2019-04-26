using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

public class CEditorProjectView_UGUIImageToSpriteRenderer : Editor
{
	static private LinkedList<Transform> _listObject = new LinkedList<Transform>();

	[MenuItem( "GameObject/StrixTool/UGUI-Image To SpriteRenderer", false, 0 )]
	static public void DoConvert_UGUI_Image_To_SpriteRenderer()
	{
		if (Init_And_CheckIsReady() == false) return;

		while (_listObject.Count != 0)
		{
			Transform pTransTarget = _listObject.First.Value;
			_listObject.RemoveFirst();
			ConvertToSpriteRenderer( pTransTarget );
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

	static private void ConvertToSpriteRenderer( Transform pObject )
	{
		DestroyImmediate( pObject.GetComponent<CanvasRenderer>() );
		Image pImage = pObject.GetComponent<Image>();
		if (pImage == null) return;

		GameObject pObjectTarget = new GameObject( pObject.name + "_SpriteRenderer");
		pObjectTarget.transform.SetParent( pObject.parent );
		pObjectTarget.transform.SetPositionAndRotation( pObject.position, pObject.rotation );

		if(pObject.childCount != 0)
		{
			for (int i = 0; i < pObject.childCount; i++)
				pObject.GetChild( i ).SetParent( pObjectTarget.transform );
		}

		SpriteRenderer pSpriteRenderer = pObjectTarget.AddComponent<SpriteRenderer>();

		Sprite pSprite = pImage.sprite;
		pSpriteRenderer.sprite = pSprite;

		Vector3 vecLocalScaleOrigin = pObject.localScale;
		pObjectTarget.transform.localScale = new Vector3( 100 * vecLocalScaleOrigin.x, 100 * vecLocalScaleOrigin.y, 1 );
		for(int i = 0; i < pObjectTarget.transform.childCount; i++)
		{
			Vector3 vecOriginPos = pObjectTarget.transform.localPosition;
			vecOriginPos *= 0.01f;
			pObjectTarget.transform.localPosition = vecOriginPos;
		}
	}
}
#endif