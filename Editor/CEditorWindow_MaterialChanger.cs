using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description : 
   Version	   :
   ============================================ */

public class CEditorWindow_MaterialChanger : CEditorWindowBase<CEditorWindow_MaterialChanger>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	//private List<GameObject> _listObjectTemp = new List<GameObject>();
	private UnityEngine.Object[] _arrCurrentSelectObject;

	private Material _pMaterialChange;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	[MenuItem("Tools/Strix_Tools/MaterialChanger")]
	public static void ShowWindow()
	{
		GetWindow<CEditorWindow_MaterialChanger>("MaterialChanger");
    }

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */
	
	protected override void OnGUIWindowEditor()
	{
		base.OnGUIWindowEditor();

		EditorGUILayout.HelpBox(string.Format("Select Object Count : {0}", _arrCurrentSelectObject == null ? 0 : _arrCurrentSelectObject.Length), MessageType.Info);
		_pMaterialChange = EditorGUILayout.ObjectField("Material ", _pMaterialChange, typeof(Material), true, GUILayout.Width(400f)) as Material;

		GUILayout.Space(20f);
		if (GUILayout.Button("Change Mateiral"))
		{
			for(int i = 0; i < _arrCurrentSelectObject.Length; i++)
			{
				GameObject pObject = _arrCurrentSelectObject[i] as GameObject;
				if (pObject == null) continue;

				Renderer[] arrRenderer = pObject.GetComponentsInChildren<Renderer>();
				if (arrRenderer == null || arrRenderer.Length == 0) continue;

				for(int j = 0; j < arrRenderer.Length; j++)
					arrRenderer[j].material = _pMaterialChange;
			}
		}

		//if (GUILayout.Button("Change Mateiral"))
		{

		}
	}

	private void OnSelectionChange()
	{
		_arrCurrentSelectObject = Selection.objects;
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
