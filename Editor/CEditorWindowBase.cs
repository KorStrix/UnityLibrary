using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
#endif

/* ============================================ 
   Editor      : Strix
   Description :
   
    [MenuItem("Tools/Strix_Tools/Somthing")]
    public static void ShowWindow()
    {
        GetWindow<CInputManager>();
    }

   Version	   :
   ============================================ */

#if ODIN_INSPECTOR
public class CEditorWindow : OdinEditorWindow
#else
public class CEditorWindow : EditorWindow
#endif
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Variable declaration            */

    /* protected - Variable declaration         */

    /* private - Variable declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    void Start() { OnStart(); }
	void Update() { OnUpdate(); }
	void OnDisable() { OnDisableObject(); }

#if ODIN_INSPECTOR
    protected override void OnEnable() { OnEnableObject(); }
    protected override void OnGUI() { OnGUIWindowEditor(); }
#else
    void OnEnable() { OnEnableObject(); }
    void OnGUI() { OnGUIWindowEditor(); }
#endif

    virtual protected void OnStart() { }
	virtual protected void OnEnableObject() { }
	virtual protected void OnUpdate() { }
	virtual protected void OnDisableObject() { }
	virtual protected void OnGUIWindowEditor() { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    protected void BeginHorizontal() { GUILayout.BeginHorizontal(); }
    protected void EndHorizontal() { GUILayout.EndHorizontal(); }

    protected void LabelField(string strLabel) { EditorGUILayout.LabelField(strLabel); }
    protected void Space(float fSpace) { GUILayout.Space(fSpace); }
    protected bool Button(string strButtonName) { return GUILayout.Button(strButtonName); }

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
