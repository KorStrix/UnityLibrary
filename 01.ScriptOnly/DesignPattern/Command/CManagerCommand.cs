#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-31 오후 4:16:11
 *	기능 : 
 *	
 *	1. InputElement에서 Buttodown, up, getAxis등을 체크하고,
 *	Ex) 키 1번 버튼 다운, 키 2번 버튼 업, 방향키 좌우 축 등
 *	
 *	2. InputEvent는 InputElement의 묶음이며, 좀더 구체적으로 캐치한다.
 *	Ex) 이벤트 1 = 키 시프트 다운 + 키 버튼 1, 이벤트 2 = 방향키 좌 버튼 3초 홀드 등
 *	
 *	3. Command는 Event가 호출되면 실행하는 함수이다.
 *	Ex) 이벤트 1를 호출하면 플레이어가 스킬 시전, 이벤트 2를 호출하면 플레이어가 좌로 이동 등
 *	
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public struct SInputValue
{
    static public SInputValue Dummy => _Dummy;
    static SInputValue _Dummy = new SInputValue(0f, Vector2.zero, false);

    public bool bValue;
    public float fAxisValue_0_1;
    public Vector2 vecValue;

    public SInputValue(float fAxisValue_0_1)
    {
        this.bValue = true;
        this.fAxisValue_0_1 = fAxisValue_0_1;
        this.vecValue = Vector2.zero;
    }

    public SInputValue(Vector2 vecValue)
    {
        this.bValue = true;
        this.fAxisValue_0_1 = 0f;
        this.vecValue = vecValue;
    }

    public SInputValue(float fAxisValue_0_1, Vector2 vecValue)
    {
        this.bValue = true;
        this.fAxisValue_0_1 = fAxisValue_0_1;
        this.vecValue = vecValue;
    }

    public SInputValue(float fAxisValue_0_1, Vector2 vecValue, bool bValue)
    {
        this.bValue = bValue;
        this.fAxisValue_0_1 = fAxisValue_0_1;
        this.vecValue = vecValue;
    }

    public override string ToString()
    {
        return "SInputValue - bValue : " + bValue + " /fAxisValue_0_1 : " + fAxisValue_0_1 + " /vecValue : " + vecValue;
    }
}

[System.Serializable]
public abstract class CCommandBase : IDictionaryItem<string>
{
    public CManagerCommand pManagerCommand { get; private set; }
    public bool bIsInit { get; protected set; }

    public virtual int iPriority { get; }

    public void DoInitCommand(CManagerCommand pManagerCommand)
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
        return this.GetType().Name;
    }

    abstract public void DoExcute(ref SInputValue fAxisValue_0_1, ref bool bIsExcuted_DefaultIsTrue);

    virtual public void DoExcute_Undo(ref SInputValue fAxisValue_0_1) { }
    virtual public void OnInitCommand(out bool bIsInit) { bIsInit = true; }
}

public partial class CManagerCommand : CSingletonMonoBase<CManagerCommand>, IUpdateAble
{
    /* const & readonly declaration             */

    readonly private Dictionary<EInnerClassType, string> _mapObjectName_ForDebug = new Dictionary<EInnerClassType, string>()
    {
        { EInnerClassType.InputElement, "Command DebugObject - InputElement" },
        { EInnerClassType.InputEvent, "Command DebugObject - InputEvent" },
        { EInnerClassType.Command, "Command DebugObject - Command" },
    };


    private const string const_DebugObject_InputElement = "";
    private const string const_DebugObject_InputEvent = "Command DebugObject - InputEvent";


    /* enum & struct declaration                */

    public enum EInput
    {
        KeyboardInput,
        UIInput,
    }

    private enum EInnerClassType
    {
        InputElement,
        InputEvent,
        Command
    }

    // ====================================================================================

    public class CommandWrapper : IDictionaryItem<string>
    {
        static StringBuilder g_pStrBuilder = new StringBuilder();

        public class CommandWrapperComparer : IComparer<CommandWrapper>
        {
            public int Compare(CommandWrapper x, CommandWrapper y)
            {
                return x.iPriority.CompareTo(y.iPriority) * -1;
            }
        }

        static public CommandWrapperComparer Comparer { get; private set; } = new CommandWrapperComparer();

        public CCommandBase pCommand { get; private set; }
        HashSet<CommandWrapper> listCommand_IgnoreOnExcute = new HashSet<CommandWrapper>();

        public int iPriority => pCommand.iPriority;
        public bool bIsIgnored { get; private set; }
        public bool bIsInputed { get; private set; }
        public bool bIsExcuted_ThisFrame { get; private set; }
        public SInputValue sValueLast { get; private set; }

        public CommandWrapper(CCommandBase pCommandBase)
        {
            pCommand = pCommandBase;
        }

        public void DoAdd_IgnoreCommand(CommandWrapper pCommand_IgnoreOnExcute)
        {
            listCommand_IgnoreOnExcute.Add(pCommand_IgnoreOnExcute);
        }

        public void DoExcute(ref SInputValue sValue)
        {
            bIsExcuted_ThisFrame = pCommand.DoExcute(ref sValue);

            foreach(var pCommand_IgnoreOnExcute in listCommand_IgnoreOnExcute)
                pCommand_IgnoreOnExcute.bIsIgnored = bIsExcuted_ThisFrame;
        }

        public void DoExcute_Undo(ref SInputValue sValue)
        {
            pCommand.DoExcute_Undo(ref sValue);
        }

        public bool Check_IsPossibleExcute()
        {
            if (bIsIgnored || pCommand.bIsInit == false)
                return false;

            return bIsInputed;
        }

        public void DoReset()
        {
            bIsIgnored = false;
            bIsExcuted_ThisFrame = false;
        }

        public void Event_OnInputEvent(InputEventBase pInputEvent, bool bIsInput, SInputValue sValueLast)
        {
            this.bIsInputed = bIsInput;
            this.sValueLast = sValueLast;

            //for (int i = 0; i < arrCommand_IgnoreOnExcute.Length; i++)
            //    arrCommand_IgnoreOnExcute[i].bIsIgnored = bIsInputed;
        }

        public string IDictionaryItem_GetKey()
        {
            return pCommand.IDictionaryItem_GetKey();
        }

        public override string ToString()
        {
            g_pStrBuilder.Length = 0;
            g_pStrBuilder.Append(IDictionaryItem_GetKey());

            if (bIsInputed)
                g_pStrBuilder.Append("/Inputed");
            else
                g_pStrBuilder.Append("/Not Inputed");

            if (pCommand.bIsInit)
            {
                if (bIsIgnored)
                    g_pStrBuilder.Append("/Ignored");
            }
            else
                g_pStrBuilder.Append("/[Not Init]");

            if(sValueLast.fAxisValue_0_1.Equals(0f) == false)
                g_pStrBuilder.Append("/AxisValue : ").Append(sValueLast.fAxisValue_0_1.ToString("F2"));

            if(sValueLast.vecValue.Equals(Vector2.zero) == false)
                g_pStrBuilder.Append("/vecValue : ").Append(sValueLast.vecValue.ToString("F2"));

            return g_pStrBuilder.ToString();
        }
    }

    public struct ComandExcuted
    {
        public CommandWrapper pCommandExcuted;

        public SInputValue sValue;
        public float fExcuteTime;

        public ComandExcuted(CommandWrapper pCommandExcuted, SInputValue sValue, float fExcuteTime)
        {
            this.pCommandExcuted = pCommandExcuted;
            this.sValue = sValue;
            this.fExcuteTime = fExcuteTime;
        }

        public void DoExcute()
        {
            pCommandExcuted.DoExcute(ref sValue);
        }

        public void DoExcute_Undo()
        {
            pCommandExcuted.DoExcute_Undo(ref sValue);
        }
    }


    /* public - Field declaration            */

    public InputElementSetting p_pInputElementConfig;

    /* protected & private - Field declaration         */

    Dictionary<string, InputElement> _mapInputElement = new Dictionary<string, InputElement>();
    Dictionary<string, InputEventBase> _mapInputEvent = new Dictionary<string, InputEventBase>();
    Dictionary<string, CommandWrapper> _mapCommandWrapper = new Dictionary<string, CommandWrapper>();

    CFixedSizeList<ComandExcuted> _listExcutedCommand = new CFixedSizeList<ComandExcuted>(1024);
    Stack<ComandExcuted> _stackPrevNextCommand = new Stack<ComandExcuted>();
    List<CommandWrapper> _listCommandWrapper = new List<CommandWrapper>();

    // For Editor Debug
    Dictionary<InputElement, GameObject> _mapInputElementObject_ForDebug = new Dictionary<InputElement, GameObject>();
    Dictionary<InputEventBase, GameObject> _mapInputEventObject_ForDebug = new Dictionary<InputEventBase, GameObject>();

    Dictionary<EInnerClassType, Transform> _mapTransform_ForDebug = new Dictionary<EInnerClassType, Transform>()
    {
        { EInnerClassType.InputElement, null }, { EInnerClassType.InputEvent, null }, { EInnerClassType.Command, null }
    };

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public InputElement DoCreate_InputElement_Key<T_ToString>(T_ToString strInputElement_ID, string strKeyInputName, EInputType eInputType)
    {
        string strID = strInputElement_ID.ToString();
        if (_mapInputElement.ContainsKey(strID) == false)
            _mapInputElement.Add(new KeyInputElement() { strInputElement_ID = strID });

        KeyInputElement pKeyboardInput = _mapInputElement[strID] as KeyInputElement;
        pKeyboardInput.strInputElement_ID = strID;
        pKeyboardInput.eInputType = eInputType;
        pKeyboardInput.strInputName = strKeyInputName;

        return pKeyboardInput;
    }

    public InputElement DoCreate_InputElement_Key_AxisDetail<T_ToString>(T_ToString strInputElement_ID, string strKeyInputName, EGetAxisDetail eGetAxisDetail, float fComparisonValue)
    {
        string strID = strInputElement_ID.ToString();
        if (_mapInputElement.ContainsKey(strID) == false)
            _mapInputElement.Add(new KeyInputElement() { strInputElement_ID = strID });

        KeyInputElement pKeyboardInput = _mapInputElement[strID] as KeyInputElement;
        pKeyboardInput.strInputElement_ID = strID;
        pKeyboardInput.eInputType = EInputType.GetAxis;
        pKeyboardInput.strInputName = strKeyInputName;
        pKeyboardInput.eAxisDetail = eGetAxisDetail;
        pKeyboardInput.fComparisonValue = fComparisonValue;

        return pKeyboardInput;
    }

    public InputElement DoCreate_InputElement_MouseDrag<T_ToString>(T_ToString strInputElement_ID, string strInputName)
    {
        string strID = strInputElement_ID.ToString();
        if (_mapInputElement.ContainsKey(strID) == false)
            _mapInputElement.Add(new MouseDragInputElement(strID, strInputName));

        return _mapInputElement[strID];
    }


    public void DoInit_CommandAll()
    {
        foreach (var pCommandWrapper in _mapCommandWrapper.Values)
            pCommandWrapper.pCommand.DoInitCommand(this);
    }

    public void DoUndo_Command()
    {
        if (_listExcutedCommand.Count == 0)
            return;

        ComandExcuted pExcutedCommand = _listExcutedCommand[_listExcutedCommand.Count - 1];
        _listExcutedCommand.RemoveAt(_listExcutedCommand.Count - 1);

        pExcutedCommand.DoExcute_Undo();
        _stackPrevNextCommand.Push(pExcutedCommand);
    }

    public void DoRedo_Command()
    {
        if (_stackPrevNextCommand.Count == 0)
            return;

        ComandExcuted pUndoCommand = _stackPrevNextCommand.Pop();
        ExcuteCommand(pUndoCommand.pCommandExcuted, ref pUndoCommand.sValue);
    }

    public void DoReplay_Command()
    {
        for (int i = 0; i < _listExcutedCommand.Count; i++)
            _listExcutedCommand[i].DoExcute();
    }

    public IEnumerator DoReplay_Command_Coroutine(float fFewSecondsTimeAgo)
    {
        return CoReplay_Command(fFewSecondsTimeAgo);
    }

    public InputEventBase DoCreate_InputEvent_Normal<T_ToString>(T_ToString strInputEvent_ID, params InputElement[] arrInputElement)
    {
        InputEvent_Normal pInputEvent = null;
        string strKey = strInputEvent_ID.ToString();
        if (_mapInputEvent.ContainsKey(strKey))
        {
            pInputEvent = _mapInputEvent[strKey] as InputEvent_Normal;
            if (pInputEvent == null)
                return null;
        }
        else
        {
            pInputEvent = new InputEvent_Normal();
            pInputEvent.strInputEvent_ID = strKey;
            _mapInputEvent.Add(pInputEvent);
        }


        for (int i = 0; i < arrInputElement.Length; i++)
        {
            InputElement pInputElement = arrInputElement[i];
            pInputElement.p_Event_OnInputElement.Subscribe += pInputEvent.Event_OnInputElement;
            if (_mapInputElement.ContainsKey(pInputElement) == false)
                _mapInputElement.Add(pInputElement);
        }

        return pInputEvent;
    }

    public CommandWrapper DoCreate_CommandWrapper(CCommandBase pCommand, InputEventBase pInputEvent)
    {
        string strID = pCommand.IDictionaryItem_GetKey();
        if (_mapCommandWrapper.ContainsKey(strID) == false)
            _mapCommandWrapper.Add(strID, new CommandWrapper(pCommand));

        CommandWrapper pCommandWrapper = _mapCommandWrapper[strID];
        pInputEvent.p_Event_OnInputEvent.Subscribe += pCommandWrapper.Event_OnInputEvent;

        if (_mapInputEvent.ContainsKey(pInputEvent) == false)
            _mapInputEvent.Add(pInputEvent);

        return pCommandWrapper;
    }


    public CommandWrapper GetCommandWrapper(string strCommandID)
    {
        if (_mapCommandWrapper.ContainsKey(strCommandID) == false)
        {
            Debug.LogError(name + " GetCommand is Null - strCommandID : " + strCommandID, this);
            return null;
        }

        return _mapCommandWrapper[strCommandID];
    }

    public CommandWrapper GetCommandWrapper<T_CommnadType>() where T_CommnadType : CCommandBase
    {
        string strCommandID = typeof(T_CommnadType).Name;
        if (_mapCommandWrapper.ContainsKey(strCommandID) == false)
        {
            Debug.LogError(name + " GetCommand is Null - strCommandID : " + strCommandID, this);
            return null;
        }

        return _mapCommandWrapper[strCommandID];
    }

    public InputElement GetInputElement<T_ToString>(T_ToString strInputElement_ID)
    {
        InputElement pElement;
        _mapInputElement.TryGetValue(strInputElement_ID.ToString(), out pElement);

        return pElement;
    }

    public InputEventBase GetInputEvent<T_ToString>(T_ToString strInputEvent_ID)
    {
        InputEventBase pEvent;
        if (_mapInputEvent.TryGetValue(strInputEvent_ID.ToString(), out pEvent) == false)
            Debug.LogError("GetInputEvent == null strInputEvent_ID : " + strInputEvent_ID);

        return pEvent;
    }

    public void Event_RegistUIInpput_Button(CUIObjectBase pUIObject, UnityEngine.UI.Button pButtonTarget, UnityEngine.Events.UnityAction OnButtonClickFunc)
    {
        Command_UIButton pCommand_UIButton = new Command_UIButton(pButtonTarget, OnButtonClickFunc);
        DoCreate_CommandWrapper(pCommand_UIButton, Create_InputEvent_UIInput(pUIObject, pButtonTarget));
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        ProjectInputSetting.Instance.DoInit();

        if (_mapInputElement.Count == 0)
        {
            Debug.LogError("base.Create_InputElement 함수를 통해 Input Element를 설정해 주세요", this);
            return;
        }

        if (_mapInputEvent.Count == 0)
        {
            Debug.LogError("base.Create_InputEvent 함수를 통해 Input Event를 설정해 주세요", this);
            return;
        }

        if (_mapCommandWrapper.Count == 0)
        {
            Debug.LogError("base.Create_CommandWrapper 함수를 통해 Command를 설정해 주세요", this);
            return;
        }

        _mapCommandWrapper.Values.ToList(_listCommandWrapper);
        _listCommandWrapper.Sort(CommandWrapper.Comparer);
    }

    public override void OnUpdate()
    {
        foreach (var pInputElement in _mapInputElement.Values)
            pInputElement.DoCheck_IsReceved();

        foreach (var pInputEvent in _mapInputEvent.Values)
            pInputEvent.DoNoityInputEvent_IfRecieveInputElement();

        for (int i = 0; i < _listCommandWrapper.Count; i++)
            ExcuteCommand_IfInputEvent(_listCommandWrapper[i]);

        for (int i = 0; i < _listCommandWrapper.Count; i++)
            _listCommandWrapper[i].DoReset();

        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Print_Hierarchy_ForDebug(EInnerClassType.InputElement, _mapInputElement.Values, _mapInputElementObject_ForDebug, PrintHierarchy_InputElement);
            Print_Hierarchy_ForDebug(EInnerClassType.InputEvent, _mapInputEvent.Values, _mapInputEventObject_ForDebug, PrintHierarchy_InputEvent);
        }
    }

    private void Print_Hierarchy_ForDebug<T>(EInnerClassType eInnerClassType, IEnumerable<T> listDebugTarget, Dictionary<T, GameObject> mapDebugObject, System.Func<T, GameObject, bool> OnPrintHierarchy)
        where T : class, IDictionaryItem<string>
    {
        Transform pTransform_ForDebug = _mapTransform_ForDebug[eInnerClassType];
        string strDebugObjectName = _mapObjectName_ForDebug[eInnerClassType];

        if (pTransform_ForDebug == null)
        {
            pTransform_ForDebug = new GameObject(strDebugObjectName).transform;
            _mapTransform_ForDebug[eInnerClassType] = pTransform_ForDebug;
        }

        int iInputCount = 0;
        foreach (var pDebugComponent in listDebugTarget)
        {
            GameObject pObjectForDebug = null;
            if (mapDebugObject.ContainsKey(pDebugComponent) == false)
            {
                pObjectForDebug = new GameObject(pDebugComponent.IDictionaryItem_GetKey());
                mapDebugObject.Add(pDebugComponent, pObjectForDebug);

                pObjectForDebug.transform.SetParent(pTransform_ForDebug);
            }
            else
                pObjectForDebug = mapDebugObject[pDebugComponent];

            if (OnPrintHierarchy(pDebugComponent, pObjectForDebug))
                iInputCount++;
        }

        pTransform_ForDebug.name = strDebugObjectName + " / Count : " + iInputCount;
    }

    private bool PrintHierarchy_InputElement(InputElement pComponent, GameObject pObjectDebug)
    {
        pObjectDebug.name = pComponent.ToString();
        return pComponent.bIsInputed;
    }

    private bool PrintHierarchy_InputEvent(InputEventBase pComponent, GameObject pObjectDebug)
    {
        pObjectDebug.name = pComponent.ToString();
        return pComponent.bIsInputed;
    }

    private bool PrintHierarchy_Command(CommandWrapper pComponent, GameObject pObjectDebug)
    {
        pObjectDebug.name = pComponent.ToString();
        return pComponent.bIsInputed;
    }

    /* protected - [abstract & virtual]         */

    // ========================================================================== //

    #region Private

    public IEnumerator CoReplay_Command(float fFewSecondsTimeAgo)
    {
        int iFindStartIndex = -1;
        float fTimeCurrent = Time.time;
        for (int i = 0; i < _listExcutedCommand.Count; i++)
        {
            if(_listExcutedCommand[i].fExcuteTime + fFewSecondsTimeAgo > fTimeCurrent)
            {
                iFindStartIndex = i;
                break;
            }
        }

        if (iFindStartIndex == -1)
            yield break;

        float fStartTime = fTimeCurrent - fFewSecondsTimeAgo;
        if (fStartTime < 0f)
            fStartTime = 0f;

        float fFirstDelay = _listExcutedCommand[iFindStartIndex].fExcuteTime - fStartTime;
        yield return YieldManager.GetWaitForSecond(fFirstDelay);

        for (int i = iFindStartIndex; i < _listExcutedCommand.Count; i++)
        {
            _listExcutedCommand[i].DoExcute();
            if(i != _listExcutedCommand.Count - 1)
            {
                float fDelay = _listExcutedCommand[i + 1].fExcuteTime - _listExcutedCommand[i].fExcuteTime;
                yield return YieldManager.GetWaitForSecond(fDelay);
            }
        }
    }

    protected InputElement Create_InputElement_Virtual<T_ToString>(T_ToString strInput_ID)
    {
        string strID = strInput_ID.ToString();
        if (_mapInputElement.ContainsKey(strID))
            return _mapInputElement[strID];
        else
            return new VirtualInputElement(strInput_ID.ToString());
    }

    protected InputEventBase Create_InputEvent_UIInput(CUIObjectBase pUIObject_Panel, UnityEngine.UI.Button pButtonTarget)
    {
        return new InputEvent_UIEvent(pUIObject_Panel, pButtonTarget);
    }

    private void ExcuteCommand_IfInputEvent(CommandWrapper pCommand)
    {
        if (pCommand.Check_IsPossibleExcute() == false)
            return;

        SInputValue sInputValue = pCommand.sValueLast;
        ExcuteCommand(pCommand, ref sInputValue);
    }

    private void ExcuteCommand(CommandWrapper pCommand, ref SInputValue sValue)
    {
        pCommand.DoExcute(ref sValue);
        _listExcutedCommand.Add(new ComandExcuted(pCommand, sValue, Time.time));
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

public class CommandTest_Print_1 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "1";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_0_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 1);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_0_1);
    }
}
public class CommandTest_Print_2 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "2";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_0_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 1);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_0_1);
    }
}
public class CommandTest_Print_3 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "3";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_0_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 1);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_0_1);
    }
}
public class CommandTest_Print_1_And_2 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "12";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_0_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 2, 2);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_0_1);
    }
}
public class CommandTest_Print_2_And_3 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "23";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_0_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 2, 2);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_0_1);
    }
}

[Category("StrixLibrary")]
public class CManagerCommandTest : CManagerCommand
{
    public enum EInputElementName
    {
        Virtual_KeyboardInput_1,
        Virtual_KeyboardInput_2,
        Virtual_KeyboardInput_3,

        KeyboardInput_Max,
    }

    public enum EInputEventName
    {
        InputEvent_1,
        InputEvent_2,
        InputEvent_3,

        InputEvent_1_And_2,
        InputEvent_2_And_3,
    }

    static public string g_strBuffer;

    /// <summary>
    /// Input Element를 등록합니다.
    /// Input Element란 GetButtonDown, GetButton, GetAxis 등의 구체적인 Input Event입니다.
    /// </summary>
    /// <param name="listInput_RegistHere"></param>
    /// 

    protected override void OnAwake()
    {
        Step1_OnRegist_InputElement();
        Step2_OnRegist_InputEvent();
        Step3_OnRegist_Command();

        base.OnAwake();
    }

    protected void Step1_OnRegist_InputElement()
    {
        // 테스트를 위한 가상 키보드 입력
        Create_InputElement_Virtual(EInputElementName.Virtual_KeyboardInput_1);
        Create_InputElement_Virtual(EInputElementName.Virtual_KeyboardInput_2);
        Create_InputElement_Virtual(EInputElementName.Virtual_KeyboardInput_3);

        // 원래 프로그램에 작성해야 될 인풋 코드
        // listInput_RegistHere.Add(Create_InputElement_Keyboard(EInputElementName.Virtual_KeyboardInput_1, "1", EInputType.ButtonDown));
    }

    /// <summary>
    /// Input Event는 Input Element를 감싼 객체입니다.
    /// Input Event는 여러개의 Input Element를 둡니다.
    /// </summary>
    /// <param name="listInputEvent_RegistHere"></param>

    protected void Step2_OnRegist_InputEvent()
    {
        DoCreate_InputEvent_Normal(EInputEventName.InputEvent_1, GetInputElement(EInputElementName.Virtual_KeyboardInput_1));
        DoCreate_InputEvent_Normal(EInputEventName.InputEvent_2, GetInputElement(EInputElementName.Virtual_KeyboardInput_2));

        DoCreate_InputEvent_Normal(EInputEventName.InputEvent_3, GetInputElement(EInputElementName.Virtual_KeyboardInput_3));

        // 1과 2를 동시에 눌렀을 때를 만들고 싶을 땐 & 연산자를 씁니다.
        DoCreate_InputEvent_Normal(EInputEventName.InputEvent_1_And_2,
            GetInputElement(EInputElementName.Virtual_KeyboardInput_1) & GetInputElement(EInputElementName.Virtual_KeyboardInput_2));

        DoCreate_InputEvent_Normal(EInputEventName.InputEvent_2_And_3,
            GetInputElement(EInputElementName.Virtual_KeyboardInput_2) & GetInputElement(EInputElementName.Virtual_KeyboardInput_3));
    }

    /// <summary>
    /// Command란 게임에 실질적으로 들어갈 로직입니다.
    /// Command는 여러개의 Input Event로 묶을 수 있습니다.
    /// </summary>
    /// <param name="listCommand_RegistHere"></param>

    protected void Step3_OnRegist_Command()
    {
        DoCreate_CommandWrapper(new CommandTest_Print_1(), GetInputEvent(EInputEventName.InputEvent_1));
        DoCreate_CommandWrapper(new CommandTest_Print_2(),GetInputEvent(EInputEventName.InputEvent_2));
        DoCreate_CommandWrapper(new CommandTest_Print_3(),GetInputEvent(EInputEventName.InputEvent_3));

        DoCreate_CommandWrapper(new CommandTest_Print_1_And_2(), GetInputEvent(EInputEventName.InputEvent_1_And_2));
        DoCreate_CommandWrapper(new CommandTest_Print_2_And_3(), GetInputEvent(EInputEventName.InputEvent_2_And_3));
    }

    // ========================================================
    // Test 시작

    Dictionary<EInputElementName, VirtualInputElement> mapVirtualInput_ForTest;

    [Test]
    public void Simple_Test()
    {
        CManagerCommandTest pManagerTest = InitCommandManager();
        g_strBuffer = "";
        Assert.AreEqual(g_strBuffer, "");

        // 키입력을 하려면 true를 한다음 update 이후 false를 해야 합니다.
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;
        Assert.AreEqual(g_strBuffer, "1");

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;
        Assert.AreEqual(g_strBuffer, "12");

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = false;
        Assert.AreEqual(g_strBuffer, "1223");
    }

    [Test]
    public void Undo_Redo_Test()
    {
        CManagerCommandTest pManagerTest = InitCommandManager();
        g_strBuffer = "";
        Assert.AreEqual(g_strBuffer, "");

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;
        Assert.AreEqual(g_strBuffer, "1");

        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, "");
        pManagerTest.DoRedo_Command(); Assert.AreEqual(g_strBuffer, "1");
        pManagerTest.DoRedo_Command(); Assert.AreEqual(g_strBuffer, "1"); // 두번 Redo를 해도 변함이 없습니다.
        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, "");
        pManagerTest.DoRedo_Command(); Assert.AreEqual(g_strBuffer, "1");
        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, "");
        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, ""); // 두번 Undo를 해도 변함이 없습니다.

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;
        Assert.AreEqual(g_strBuffer, "12");

        pManagerTest.DoUndo_Command(); Assert.AreEqual(g_strBuffer, "");
    }

    [Test]
    public void Replay_Test()
    {
        CManagerCommandTest pManagerTest = InitCommandManager();
        g_strBuffer = "";
        Assert.AreEqual(g_strBuffer, "");

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

        g_strBuffer += "_";
        Assert.AreEqual(g_strBuffer, "1212_");

        pManagerTest.DoReplay_Command(); // 이전에 1212를 입력한 채로 Replay를 요청하면 1212를 또 입력합니다.
        Assert.AreEqual(g_strBuffer, "1212_1212");
    }

    [UnityTest]
    public IEnumerator Replay_Test_WithTimeScale() // 로그로 실행 시간을 출력했습니다. 
    {
        CManagerCommandTest pManagerTest = InitCommandManager();
        g_strBuffer = "";
        Assert.AreEqual(g_strBuffer, "");

        List<float> listWaitTime = new List<float>();

        Time.timeScale = 5f;

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

        float fRandomWait_1 = CreateWaitSecond_And_PrintLog(listWaitTime);
        yield return new WaitForSeconds(fRandomWait_1);

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

        float fRandomWait_2 = CreateWaitSecond_And_PrintLog(listWaitTime);
        yield return new WaitForSeconds(fRandomWait_2);

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = false;

        float fRandomWait_3 = CreateWaitSecond_And_PrintLog(listWaitTime);
        yield return new WaitForSeconds(fRandomWait_3);

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

        g_strBuffer += "_";
        Assert.AreEqual(g_strBuffer, "2132_");

        float fReplayTimeAgo = fRandomWait_2 + fRandomWait_3 + 0.3f;  // 0.3을 더한 이유는 WaitForSecound 특성상 딱맞게 기다리지 않기 때문입니다.
        float fReplayStartTime = Time.time;
        Debug.Log("");
        Debug.Log("Replay Start : " + fReplayStartTime.ToString("F2") + " Time Ago : " + fReplayTimeAgo.ToString("F2"));
        Debug.Log("Start Time : " + (fReplayStartTime - fReplayTimeAgo).ToString("F2"));
        Debug.Log("");

        int iLoopCount = 0;
        var pCoroutine = pManagerTest.CoReplay_Command(fReplayTimeAgo);
        while (pCoroutine.MoveNext())
        {
            yield return pCoroutine.Current;

            float fCurrentTime = Time.time;
            float fWaitSecond = fCurrentTime - fReplayStartTime;
            Debug.Log("Wait Second : " + fWaitSecond.ToString("F2"));
            fReplayStartTime = fCurrentTime;

            // 실행시 기다리는 시간과 리플레이시 기다리는 시간의 값의 차이가 0.2초이하면 True / 0.2초의 경우 위와 마찬가지로 WaitForSecond 특성상 정확하지 않다.
            // 로그 확인
            if (iLoopCount != 0)
                Assert.IsTrue(fWaitSecond.IsSimilar(listWaitTime[iLoopCount], 0.2f));

            iLoopCount++;
        }

        Assert.AreEqual(g_strBuffer, "2132_132");
        Time.timeScale = 1f;

        yield break;
    }

    // ========================================================

    /// <summary>
    /// 커맨드 매니져 초기화 코드
    /// </summary>
    /// <returns></returns>
    private CManagerCommandTest InitCommandManager()
    {
        GameObject pObjectCommand = new GameObject();
        CManagerCommandTest pManagerTest = pObjectCommand.AddComponent<CManagerCommandTest>();

        mapVirtualInput_ForTest = new Dictionary<EInputElementName, VirtualInputElement>();
        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_1, pManagerTest.GetInputElement(EInputElementName.Virtual_KeyboardInput_1) as VirtualInputElement);
        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_2, pManagerTest.GetInputElement(EInputElementName.Virtual_KeyboardInput_2) as VirtualInputElement);
        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_3, pManagerTest.GetInputElement(EInputElementName.Virtual_KeyboardInput_3) as VirtualInputElement);

        pManagerTest.DoInit_CommandAll();

        return pManagerTest;
    }

    private static float CreateWaitSecond_And_PrintLog(List<float> listWaitTime)
    {
        float fRandomWait = Random.Range(0.8f, 1.5f);
        Debug.Log("Wait Second : " + fRandomWait.ToString("F2"));
        listWaitTime.Add(fRandomWait);
        return fRandomWait;
    }
}

#endif
#endregion Test