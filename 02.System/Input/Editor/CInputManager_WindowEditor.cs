#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-15 오전 11:27:03
 *	개요 : http://plyoung.appspot.com/blog/manipulating-input-manager-in-script.html
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using static CManagerCommand;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector;

/// <summary>
/// 
/// </summary>
public class CInputManager_WindowEditor : OdinMenuEditorWindow
{
    /* const & readonly declaration             */


    /* enum & struct declaration                */


    /* public - Field declaration            */


    /* protected & private - Field declaration         */



    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    [MenuItem("Tools/Strix_Tools/InputManager")]
    public static void ShowWindow()
    {
        GetWindow<CInputManager_WindowEditor>();
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnEnable()
    {
        base.OnEnable();

        ProjectInputSetting.p_Event_OnChangeSetting.Subscribe += OnChangeSetting;
    }

    private void OnChangeSetting()
    {
        ForceMenuTreeRebuild();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        if (ProjectInputSetting.Instance.p_pInputSetting == null)
        {
            ProjectInputSetting.DoSet_InputSetting(CreateInstance<InputSetting>());
            ProjectInputSetting.DoUpdate_InputSetting_From_ProjectSetting();
        }

        if (ProjectInputSetting.Instance.p_pInputElementSetting == null)
            ProjectInputSetting.DoSet_InputElementSetting(ScriptableObjectUtility_Editor.CreateAsset<InputElementSetting>());

        if (ProjectInputSetting.Instance.p_pInputEventSetting == null)
            ProjectInputSetting.DoSet_InputEventSetting(ScriptableObjectUtility_Editor.CreateAsset<InputEventSetting>());

        OdinMenuTree pTree = new OdinMenuTree(supportsMultiSelect: true)
        {
            { "Home", ProjectInputSetting.Instance },
            { "Input Setting (= Project Setting - Input)", ProjectInputSetting.Instance.p_pInputSetting },
            { "Input Element Setting", ProjectInputSetting.Instance.p_pInputElementSetting },
            { "Input Event Setting", ProjectInputSetting.Instance.p_pInputEventSetting },
        };

        return pTree;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}

#endif