using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-02-02 오후 6:03:16
   Description : 
   Edit Log    : 
   ============================================ */

public class SCStrix_Tools
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	[MenuItem( "GameObject/StrixTool/Apply Multiple Prefabs", false, 15 )]
	static void ApplyPrefabs()
	{
		var selections = Selection.gameObjects;
		foreach (var go in selections)
		{
			var prefabType = PrefabUtility.GetPrefabType( go );
			if (prefabType == PrefabType.PrefabInstance ||
				prefabType == PrefabType.DisconnectedPrefabInstance)
			{
				var goRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot( go );
				var prefabParent = PrefabUtility.GetCorrespondingObjectFromSource( goRoot );
				var option = ReplacePrefabOptions.ConnectToPrefab;

				PrefabUtility.ReplacePrefab( goRoot, prefabParent, option );
				EditorSceneManager.MarkSceneDirty( goRoot.scene );
			}
		}
	}

	/* public - [Event] Function             
	   프랜드 객체가 호출                       */

	// ========================================================================== //

	/* private - [Proc] Function             
	   중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}
