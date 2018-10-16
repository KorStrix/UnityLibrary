using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix
   Description :
   
	//[MenuItem("Tools/Strix_Tools/Name")]
	//public static void ShowWindow()
	//{
	//	GetWindow(typeof(TClass));
	//}

   Version	   :
   ============================================ */

public class CEditorWindowBase<TClass> : EditorWindow
	where TClass : CEditorWindowBase<TClass>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	static public TClass instance { get { return _instance; } }

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	static private TClass _instance;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

	/* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	void Start() { OnStart(); }
	void OnEnable() { _instance = this as TClass; OnEnableObject(); }
	void Update() { OnUpdate(); }
	void OnDisable() { _instance = null; OnDisableObject(); }
	void OnGUI() { OnGUIWindowEditor(); }

	virtual protected void OnStart() { }
	virtual protected void OnEnableObject() { }
	virtual protected void OnUpdate() { }
	virtual protected void OnDisableObject() { }
	virtual protected void OnGUIWindowEditor() { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	// ========================================================================== //

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	protected string ConvertRelativePath( string strPath )
	{
		return "Assets" + strPath.Substring( Application.dataPath.Length );
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

}
