#region Header
/* ============================================ 
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DefaultExecutionOrder( -1000 )]
public class CCompoDestroyObject_WhenMultipleObject : CObjectBase
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration            */

	/* protected - Field declaration         */

	/* private - Field declaration           */

	static private Dictionary<string, CCompoDestroyObject_WhenMultipleObject> g_mapSingleton = new Dictionary<string, CCompoDestroyObject_WhenMultipleObject>();

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/


	// ========================================================================== //

	#region Protected
	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		if (g_mapSingleton.ContainsKey( name ))
		{
			if (g_mapSingleton[name] == this)
				return;

			Debug.LogWarning( "[CCompoDestroyObject_WhenMultipleObject] Destroy!! " + gameObject.name );
			gameObject.SetActive( false );
			Destroy( gameObject );
			return;
		}
		else
			g_mapSingleton.Add( name, this );
	}

	private void OnDestroy()
	{
		if (g_mapSingleton.ContainsKey( name ))
		{
			if (g_mapSingleton[name] == this)
				g_mapSingleton.Remove( name );
		}
	}

	#endregion Protected

	// ========================================================================== //

	#region Private
	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}
