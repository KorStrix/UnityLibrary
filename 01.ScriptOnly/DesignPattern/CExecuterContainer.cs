#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-16 오전 10:41:47
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using Sirenix.OdinInspector.Editor;
#endif

public interface IExecuter : IHasName
{
    int p_iExecuterOrder { get; }

    void IExecuter_OnAwake(IExecuteContainer pContainer, CObjectBase pScriptOwner);
    void IExecuter_OnEnable(IExecuteContainer pContainer, CObjectBase pScriptOwner);
    void IExecuter_Check_IsInvalid_OnEditor(CObjectBase pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error);
}

public interface IExecuteContainer
{
    void DoNotify_OnAwake(CObjectBase pObjectOwner);
    void DoNotify_OnEnable();
}

/// <summary>
/// 
/// </summary>
public class CExecuterContainer<CLASS_EXECUTER> : IExecuteContainer
    where CLASS_EXECUTER : class, IExecuter
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class Comparer_ByOrder : CSingletonNotMonoBase<Comparer_ByOrder>, IComparer<ValueDropdownItem<CLASS_EXECUTER>>
    {
        public int Compare(ValueDropdownItem<CLASS_EXECUTER> x, ValueDropdownItem<CLASS_EXECUTER> y)
        {
            return x.Value.p_iExecuterOrder.CompareTo(y.Value.p_iExecuterOrder);
        }
    }

    /* public - Field declaration            */

    [Indent(-1)]
    [Space(10), TypeFilter(nameof(GetTypeFilter)), ValueDropdown(nameof(GetValueDownList)), ListDrawerSettings(ShowIndexLabels = false, ListElementLabelName = "IHasName_GetName")]
    public List<CLASS_EXECUTER> p_listExecute = new List<CLASS_EXECUTER>();

    /* protected & private - Field declaration         */

    protected string strList;
    protected CObjectBase _pObjectOwner { get; private set; }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoNotify_OnAwake(CObjectBase pObjectOwner)
    {
        _pObjectOwner = pObjectOwner;
        if(p_listExecute == null)
        {
            Debug.LogError(pObjectOwner.name + nameof(DoNotify_OnAwake) + " p_listExecute Is Null", pObjectOwner);
            return;
        }

        for (int i = 0; i < p_listExecute.Count; i++)
        {
            CLASS_EXECUTER pExecute = p_listExecute[i];
            if (pExecute == null)
            {
                Debug.LogError(pObjectOwner.name + " " + nameof(DoNotify_OnAwake) + " ExecuteItem Is Null", pObjectOwner);
                continue;
            }

            pExecute.IExecuter_OnAwake(this, _pObjectOwner);
        }

        OnAwake();
    }

    public void DoNotify_OnEnable()
    {
        for (int i = 0; i < p_listExecute.Count; i++)
            p_listExecute[i]?.IExecuter_OnEnable(this, _pObjectOwner);

        OnEnable();
    }

    public void DoCheck_IsInvalidExecute(CObjectBase pObjectOwner)
    {
        for(int i = 0; i < p_listExecute.Count; i++)
        {
            bool bIsInvalid = false;
            string strErrorMessage = "Error";
            p_listExecute[i]?.IExecuter_Check_IsInvalid_OnEditor(pObjectOwner, ref bIsInvalid, ref strErrorMessage);
            if(bIsInvalid)
            {
                Debug.LogError(nameof(DoCheck_IsInvalidExecute) + " bIsInvalid " + strErrorMessage, pObjectOwner);
            }
        }
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */

    virtual protected void OnAwake() { }
    virtual protected void OnEnable() { }

    virtual protected ValueDropdownList<CLASS_EXECUTER> GetValueDownList()
    {
        var list = OdinExtension.GetValueDropDownList_HasName<CLASS_EXECUTER>();
        list.Sort(Comparer_ByOrder.instance);
        return list;
    }

    virtual protected IEnumerable<System.Type> GetTypeFilter() { return OdinExtension.GetTypeFilter(typeof(CLASS_EXECUTER)); }

    // ========================================================================== //

    #region Private


    #endregion Private
}


#if UNITY_EDITOR

[DrawerPriority(1, 0, 0)]
public class CExecuteContainer_Drawer<CLASS_EXECUTER> : OdinValueDrawer<CExecuterContainer<CLASS_EXECUTER>>
    where CLASS_EXECUTER : class, IExecuter
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (this.ValueEntry.SmartValue == null)
            this.ValueEntry.SmartValue = new CExecuterContainer<CLASS_EXECUTER>();

        if (this.ValueEntry.SmartValue.p_listExecute == null)
            this.ValueEntry.SmartValue.p_listExecute = new List<CLASS_EXECUTER>();

        int iLoopIndex = 0;
        foreach (var pExecute in this.ValueEntry.SmartValue.p_listExecute)
        {
            if (pExecute == null)
                continue;

            bool bIsInvalid = false;
            string strErrorMessage = "Error";
            pExecute.IExecuter_Check_IsInvalid_OnEditor(null, ref bIsInvalid, ref strErrorMessage);
            if (bIsInvalid)
                SirenixEditorGUI.ErrorMessageBox("Index [" + iLoopIndex + "] Name : " + pExecute.IHasName_GetName() + "\n" + strErrorMessage);

            iLoopIndex++;
        }

        this.ValueEntry.Property.Children["p_listExecute"]?.Draw(label);
    }
}


[DrawerPriority(1, 0, 0)]
public class CExecuteContainer_Drawer_Inherit<CLASS_EXECUTE_CONTAINER, CLASS_EXECUTER>  : OdinValueDrawer<CLASS_EXECUTE_CONTAINER>
    where CLASS_EXECUTE_CONTAINER : CExecuterContainer<CLASS_EXECUTER>
    where CLASS_EXECUTER : class, IExecuter
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (this.ValueEntry.SmartValue == null)
            this.ValueEntry.SmartValue = Activator.CreateInstance<CLASS_EXECUTE_CONTAINER>();

        if (this.ValueEntry.SmartValue.p_listExecute == null)
            this.ValueEntry.SmartValue.p_listExecute = new List<CLASS_EXECUTER>();

        int iLoopIndex = 0;
        foreach (var pExecute in this.ValueEntry.SmartValue.p_listExecute)
        {
            if (pExecute == null)
                continue;

            bool bIsInvalid = false;
            string strErrorMessage = "Error";
            pExecute.IExecuter_Check_IsInvalid_OnEditor(null, ref bIsInvalid, ref strErrorMessage);
            if (bIsInvalid)
                SirenixEditorGUI.ErrorMessageBox("Index [" + iLoopIndex + "] Name : " + pExecute.IHasName_GetName() + "\n" + strErrorMessage);

            iLoopIndex++;
        }

        this.ValueEntry.Property.Children["p_listExecute"]?.Draw(label);
    }
}


#endif