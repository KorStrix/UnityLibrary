#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-16 오후 5:35:59
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public struct SInputValue
{
    static public SInputValue Dummy => _Dummy;
    static SInputValue _Dummy = new SInputValue(false, false, 0f, Vector2.zero, false);

    public bool bIsAlwaysInput;
    public bool bIsInput;

    public bool bValue;
    public float fAxisValue_Minus1_1;
    public Vector2 vecValue;

    public SInputValue(bool bIsAlwaysInput, bool bIsInput, float fAxisValue_0_1, Vector2 vecValue, bool bValue)
    {
        this.bIsAlwaysInput = bIsAlwaysInput;
        this.bIsInput = bIsInput;
        this.bValue = bValue;
        this.fAxisValue_Minus1_1 = fAxisValue_0_1;
        this.vecValue = vecValue;
    }

    public override string ToString()
    {
        return "SInputValue - bValue : " + bValue + " /fAxisValue_0_1 : " + fAxisValue_Minus1_1 + " /vecValue : " + vecValue;
    }

    static public bool operator ==(SInputValue pValueA, SInputValue pValueB)
    {
        if (pValueA.bIsAlwaysInput != pValueB.bIsAlwaysInput)
            return false;

        if (pValueA.bIsInput != pValueB.bIsInput)
            return false;

        if (pValueA.bValue != pValueB.bValue)
            return false;

        if (pValueA.fAxisValue_Minus1_1 != pValueB.fAxisValue_Minus1_1)
            return false;

        return true;
    }

    static public bool operator !=(SInputValue pValueA, SInputValue pValueB)
    {
        return !(pValueA == pValueB);
    }

    static public SInputValue operator &(SInputValue pValueA, SInputValue pValueB)
    {
        SInputValue pValue = new SInputValue();
        pValue.bIsAlwaysInput = pValueA.bIsAlwaysInput || pValueB.bIsAlwaysInput;
        pValue.bIsInput = pValueA.bIsInput && pValueB.bIsInput;
        if(pValue.bIsInput)
        {
            pValue.bValue = pValueA.bValue && pValueB.bValue;
            pValue.fAxisValue_Minus1_1 = pValueA.fAxisValue_Minus1_1 + pValueB.fAxisValue_Minus1_1;
        }

        return pValue;
    }

    static public SInputValue operator |(SInputValue pValueA, SInputValue pValueB)
    {
        SInputValue pValue = new SInputValue();
        pValue.bIsAlwaysInput = pValueA.bIsAlwaysInput || pValueB.bIsAlwaysInput;
        pValue.bIsInput = pValueA.bIsInput || pValueB.bIsInput;
        if (pValue.bIsInput)
        {
            pValue.bValue = pValueA.bValue || pValueB.bValue;
            pValue.fAxisValue_Minus1_1 = pValueA.fAxisValue_Minus1_1 + pValueB.fAxisValue_Minus1_1;
        }

        return pValue;
    }
}

[System.Serializable]
public abstract class CommandBase : IDictionaryItem<string>
{
    public CommandExecuter_Input pManagerCommand { get; private set; }
    public bool bIsInit { get; protected set; }

    public virtual int iPriority { get; }
    public virtual string strCommandCategory { get; }

    public void DoInitCommand(CommandExecuter_Input pManagerCommand)
    {
        this.pManagerCommand = pManagerCommand;

        bool bIsInit_Local;
        OnInitCommand(out bIsInit_Local);
        bIsInit = bIsInit_Local;
    }

    public bool DoExcute(ref SInputValue sValue)
    {
        bool bIsExcuted = true;
        DoExcute(ref sValue, ref bIsExcuted);

        return bIsExcuted;
    }

    virtual public string IDictionaryItem_GetKey()
    {
        return GetDisplayCommandName();
    }

    virtual public string GetDisplayCommandName()
    {
        return GetType().GetFriendlyName();
    }

    abstract public void DoExcute(ref SInputValue sInputValue, ref bool bIsExcuted_DefaultIsTrue);

    virtual public void DoExcute_Undo(ref SInputValue sInputValue) { }
    virtual public void OnInitCommand(out bool bIsInit) { bIsInit = true; }
}

/// <summary>
/// 
/// </summary>
public class CommandExecuter_Input : CSingletonMonoBase<CommandExecuter_Input>, ICommandExecuter
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public class CommandWrapper
    {
        static StringBuilder g_pStrBuilder = new StringBuilder();
        static public CommandWrapperComparer Comparer { get; private set; } = new CommandWrapperComparer();

        public class CommandWrapperComparer : IComparer<CommandWrapper>
        {
            public int Compare(CommandWrapper x, CommandWrapper y)
            {
                return x.iPriority.CompareTo(y.iPriority) * -1;
            }
        }

        public CommandBase pCommand { get; private set; }
        public int iPriority => pCommand.iPriority;
        public string strCommandCategory => pCommand.strCommandCategory;

        public bool bEnable { get; private set; }
        public bool bIsIgnored { get; private set; }
        public bool bIsInputed { get; private set; }
        public bool bIsExcuted_ThisFrame { get; private set; }
        public SInputValue sValueLast { get; private set; }

        HashSet<CommandWrapper> listCommand_IgnoreOnExcute = new HashSet<CommandWrapper>();
    }

    /* public - Field declaration            */

    public ObservableCollection<ICommandExecuteArg> p_Event_OnExecuteCommand { get; private set; } = new ObservableCollection<ICommandExecuteArg>();

    [DisplayName("Input Event Setting")]
    public InputEventSetting p_pInputEventV2;

    public float p_fLastExecuteTime { get; set; }
    public bool p_bEnableExecuter { get; set; } = true;

    /* protected & private - Field declaration         */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoInit()
    {
        p_pInputEventV2.DoInit(this);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        DoInit();
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        CManagerCommand.instance.DoAdd_CommandExecuter(this);
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
    {
        base.OnDisableObject(bIsQuitApplciation);

        CManagerCommand.instance?.DoRemove_CommandExecuter(this);
    }

    public void ICommandExecuter_Update(ref List<CommandExcuted> listCommandExecute_Default_Is_Empty)
    {
        float fTime = Time.time;
        var listCommandInfo = p_pInputEventV2.listCommandInfo;
        foreach (var pCommandInfo in listCommandInfo)
        {
            CommandBase pCommand = pCommandInfo.pCommand;
            if (pCommand == null || pCommand.bIsInit == false)
                continue;

            SInputValue sValue = pCommandInfo.pInputInfoGroup.DoCalculate_Relate();
            if (sValue.bIsInput == false)
                continue;

            pCommand.DoExcute(ref sValue);

            CommandExcuted pCommandExecuteData = new CommandExcuted(this, pCommand, sValue, fTime);

            listCommandExecute_Default_Is_Empty.Add(pCommandExecuteData);
            p_Event_OnExecuteCommand.DoNotify(new ICommandExecuteArg(pCommandExecuteData));
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}