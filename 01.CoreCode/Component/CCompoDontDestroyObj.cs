using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-23 오후 1:44:15
// Description : 
// Edit Log    : 
// ============================================ 

// Singleton의 경우 미리 찾아서 SetActive를 False시켜야 같은 게임오브젝트의 다른 컴포넌트의 Awake의 실행을 방지한다.
[DefaultExecutionOrder(-1000)]
public class CCompoDontDestroyObj : CObjectBase
{
	static private Dictionary<string, CCompoDontDestroyObj> g_mapSingleton = new Dictionary<string, CCompoDontDestroyObj>();

	[Rename_InspectorAttribute( "중복있으면삭제?" )]
	public bool _bIsSingleton = false;

	protected override void OnAwake()
    {
		base.OnAwake();

		if (_bIsSingleton)
		{
			if (g_mapSingleton.ContainsKey( name ))
			{
				if (g_mapSingleton[name] == this)
					return;

				Debug.LogWarning( "[CCompoDontDestroyObj] Destroy!! " + gameObject.name );
				gameObject.SetActive( false );
				Destroy( gameObject );
				return;
			}
			else
				g_mapSingleton.Add( name, this );
		}

		ProcSetDonDestoryOnLoad();
	}

	private void ProcSetDonDestoryOnLoad()
	{
		Transform pTransformRoot = transform;
		while (pTransformRoot.parent != null)
		{
			pTransformRoot = transform.parent;
		}

		DontDestroyOnLoad( pTransformRoot.gameObject );
	}

}
