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

public class CManagerCommand : CSingletonMonoBase<CManagerCommand>, IUpdateAble
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

    #region InnerClass


    public enum EInputType
    {
        ButtonDown,
        Button,
        ButtonUp,

        GetAxis,
    }

    public enum EGetAxisDetail
    {
        Normal,
        Greater,
        Greater_ABS,
        Lesser,
        Lesser_ABS,
    }


    public abstract class InputElement : IDictionaryItem<string>
    {
        public enum EAxisValueCalculate
        {
            A_Plus_B,
            A_Only,
            B_Only,

            Extract_Vector2,
        }

        public const string const_strRevereAxis = "(Reverse_Axis)";
        public const string const_strAlwaysInput = "/Always_Input/";

        static protected StringBuilder g_pStrBuilder = new StringBuilder();

        public CObserverSubject<InputElement, bool, SInputValue> p_Event_OnInputElement { get; private set; } = new CObserverSubject<InputElement, bool, SInputValue>();
        public bool bIsInputed { get; private set; }
        public SInputValue sValueLast { get; private set; }

        public string strInputElement_ID;
        public bool bEnable = true;

        public List<InputElement> _listInputElement_Child { get; private set; } = new List<InputElement>();
        protected List<InputEventBase> _listInputEvent = new List<InputEventBase>();

        protected bool _bIsInputed;

        // ====================================================================================

        public void DoAdd_InputEvent(InputEventBase pInputEvent)
        {
            if (_listInputEvent.Contains(pInputEvent) == false)
                _listInputEvent.Add(pInputEvent);
        }

        public void DoClear_InputEvent()
        {
            for (int i = 0; i < _listInputEvent.Count; i++)
                _listInputEvent[i].DoRemove_InputElement(this);
        }

        public void DoCheck_IsReceved()
        {
            if (bEnable == false)
                return;

            bool bIsInput;
            SInputValue sValue;
            DoCalculate_InputValue(out bIsInput, out sValue);
            p_Event_OnInputElement.DoNotify(this, bIsInput, sValue);

            bIsInputed = bIsInput;
            sValueLast = sValue;
        }

        public string IDictionaryItem_GetKey()
        {
            return strInputElement_ID;
        }

        public override string ToString()
        {
            g_pStrBuilder.Length = 0;

            if (bEnable == false)
            {
                g_pStrBuilder.Append("(Disable").Append(IDictionaryItem_GetKey()).Append("(Disable)");
                return g_pStrBuilder.ToString();
            }

            g_pStrBuilder.Append(IDictionaryItem_GetKey());
            if (bIsInputed)
                g_pStrBuilder.Append("/Inputed");
            else
                g_pStrBuilder.Append("/Not Inputed");

            if (sValueLast.fAxisValue_Minus1_1 != 0f)
                g_pStrBuilder.Append("/AxisValue : " + sValueLast.fAxisValue_Minus1_1.ToString("F2"));

            if (sValueLast.vecValue.Equals(Vector2.zero) == false)
                g_pStrBuilder.Append("/vecValue : " + sValueLast.vecValue.ToString("F2"));

            return g_pStrBuilder.ToString();
        }


        public InputElement DoSet_PressTime(float fPressTime)
        {
            return new InputElement_PressTime(this, fPressTime);
        }

        public InputElement DoAlwaysCall_And_InputValue_Is_BoolValue()
        {
            return new InputElement_AlwaysInput(this);
        }

        public InputElement DoSet_ReverseAxis()
        {
            return new InputElement_Reverse_Axis(this);
        }

        // ====================================================================================

        virtual public void Event_OnIgnored() { }
        abstract public void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue);

        // ====================================================================================

        public static InputElement_Reverse_Input operator !(InputElement a)
        {
            return new InputElement_Reverse_Input(a);
        }

        public static CombineInput_OR operator |(InputElement a, InputElement b)
        {
            return new CombineInput_OR(a, b);
        }

        public static CombineInput_AND operator &(InputElement a, InputElement b)
        {
            return new CombineInput_AND(a, b);
        }
    }

    public class VirtualInputElement : InputElement
    {
        public bool bIsInput;
        public SInputValue sValue;

        public VirtualInputElement(string strInput_ID)
        {
            this.strInputElement_ID = strInput_ID;
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            bIsInput = this.bIsInput;
            sValue = this.sValue;
        }
    }

    [System.Serializable]
    public class KeyInputElement : InputElement
    {
        public EInputType eInputType;
        public EGetAxisDetail eAxisDetail = EGetAxisDetail.Normal;
        public string strInputName;
        public float fComparisonValue;
        
        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            sValue = SInputValue.Dummy;

            switch (eInputType)
            {
                case EInputType.ButtonDown: _bIsInputed = Input.GetButtonDown(strInputName); break;
                case EInputType.ButtonUp: _bIsInputed = Input.GetButtonUp(strInputName); break;
                case EInputType.Button: _bIsInputed = Input.GetButton(strInputName); break;

                case EInputType.GetAxis:
                    {
                        sValue.fAxisValue_Minus1_1 = Input.GetAxis(strInputName);
                        switch (eAxisDetail)
                        {
                            case EGetAxisDetail.Normal:
                                _bIsInputed = true;
                                break;

                            case EGetAxisDetail.Greater:
                                _bIsInputed = sValue.fAxisValue_Minus1_1 > fComparisonValue;
                                break;

                            case EGetAxisDetail.Greater_ABS:
                                _bIsInputed = Mathf.Abs(sValue.fAxisValue_Minus1_1) > fComparisonValue;
                                break;

                            case EGetAxisDetail.Lesser:
                                _bIsInputed = sValue.fAxisValue_Minus1_1 < fComparisonValue;
                                break;

                            case EGetAxisDetail.Lesser_ABS:
                                _bIsInputed = Mathf.Abs(sValue.fAxisValue_Minus1_1) < fComparisonValue;
                                break;
                        }
                    }
                    break;
            }

            bIsInput = _bIsInputed;
            sValue.bValue = bIsInput;
        }

    }

    [System.Serializable]
    public class MouseDragInputElement : InputElement
    {
        public string strInputName { get; private set; }
        public Vector3 vecMouseDragDirection { get; private set; }

        Vector3 _vecMousePosOrigin;

        public MouseDragInputElement(string strInputElement_ID, string strInputName)
        {
            this.strInputElement_ID = strInputElement_ID;
            this.strInputName = strInputName;
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            bIsInput = true;
            sValue = SInputValue.Dummy;
            sValue.bValue = bIsInput;

            if (Input.GetButtonDown(strInputName))
            {
                _vecMousePosOrigin = Input.mousePosition;
            }
            else if (Input.GetButton(strInputName))
            {
                sValue.vecValue = (Input.mousePosition - _vecMousePosOrigin).normalized;
                sValue.fAxisValue_Minus1_1 = Vector3.Distance(Input.mousePosition, _vecMousePosOrigin);
            }
        }
    }

    public class InputElement_Reverse_Input : InputElement
    {
        InputElement _pInputElement_Original;

        public InputElement_Reverse_Input(InputElement pInputElement_Original)
        {
            _pInputElement_Original = pInputElement_Original;
            _pInputElement_Original.p_Event_OnInputElement.Subscribe += OnInputElement;

            strInputElement_ID = string.Format("({0}_NotInput)", pInputElement_Original.strInputElement_ID);

            _listInputElement_Child.Add(pInputElement_Original);
        }

        private void OnInputElement(InputElement pInputElement, bool bInput, SInputValue sValue)
        {
            sValue.bValue = !sValue.bValue;
            p_Event_OnInputElement.DoNotify(this, !bInput, sValue);
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            _pInputElement_Original.DoCalculate_InputValue(out bIsInput, out sValue);
            bIsInput = !bIsInput;
        }
    }

    public class InputElement_Reverse_Axis : InputElement
    {
        InputElement _pInputElement_Original;

        public InputElement_Reverse_Axis(InputElement pInputElement_Original)
        {
            _pInputElement_Original = pInputElement_Original;
            _pInputElement_Original.p_Event_OnInputElement.Subscribe += OnInputElement;

            strInputElement_ID = pInputElement_Original.strInputElement_ID + const_strRevereAxis;

            _listInputElement_Child.Add(pInputElement_Original);
        }

        private void OnInputElement(InputElement pInputElement, bool bInput, SInputValue sValue)
        {
            sValue.fAxisValue_Minus1_1 *= -1f;
            p_Event_OnInputElement.DoNotify(this, bInput, sValue);
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            _pInputElement_Original.DoCalculate_InputValue(out bIsInput, out sValue);
            sValue.fAxisValue_Minus1_1 *= -1f;
        }
    }


    public class InputElement_PressTime : InputElement
    {
        InputElement _pInputElement_Original;
        public float fPressWaitTime { get; protected set; } = 0f;

        protected float _fPressTimeStart;
        protected bool _bIsInputPressPrev;

        public InputElement_PressTime(InputElement pInputElement_Original, float fPressTime)
        {
            _pInputElement_Original = pInputElement_Original;
            _pInputElement_Original.p_Event_OnInputElement.Subscribe += OnInputElement;

            strInputElement_ID = pInputElement_Original.strInputElement_ID + "/PressTime:" + fPressTime + "s/";
            this.fPressWaitTime = fPressTime;
            this._fPressTimeStart = -1f;

            _listInputElement_Child.Add(pInputElement_Original);
        }

        private void OnInputElement(InputElement pInputElement, bool bInput, SInputValue sValue)
        {
            p_Event_OnInputElement.DoNotify(this, bInput, sValue);
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            bool bIsInputElement;
            _pInputElement_Original.DoCalculate_InputValue(out bIsInputElement, out sValue);

            float fCurrentTime = Time.time;
            if (_fPressTimeStart == -1f || bIsInputElement == false)
                bIsInput = false;
            else
                bIsInput = _fPressTimeStart + fPressWaitTime < fCurrentTime;

            if (_bIsInputPressPrev != bIsInputElement)
            {
                _bIsInputPressPrev = bIsInputElement;
                if (bIsInputElement)
                    _fPressTimeStart = fCurrentTime;
                else
                    _fPressTimeStart = -1f;
            }
        }

        public override void Event_OnIgnored()
        {
            _fPressTimeStart = Time.time;
        }

        public override string ToString()
        {
            string strBaseString = base.ToString();
            g_pStrBuilder.Length = 0;
            g_pStrBuilder.Append(strBaseString);

            if (_fPressTimeStart != -1f)
                g_pStrBuilder.Append("/Pressing : " + (Time.time - _fPressTimeStart).ToString("F2"));

            return g_pStrBuilder.ToString();
        }
    }

    public class InputElement_AlwaysInput : InputElement
    {
        InputElement _pInputElement_Original;

        public InputElement_AlwaysInput(InputElement pInputElement_Original)
        {
            _pInputElement_Original = pInputElement_Original;
            _pInputElement_Original.p_Event_OnInputElement.Subscribe += OnInputElement;

            strInputElement_ID = pInputElement_Original.strInputElement_ID + const_strAlwaysInput;
            _listInputElement_Child.Add(pInputElement_Original);
        }

        private void OnInputElement(InputElement pInputElement, bool bInput, SInputValue sValue)
        {
            p_Event_OnInputElement.DoNotify(this, true, sValue);
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            _pInputElement_Original.DoCalculate_InputValue(out bIsInput, out sValue);
            sValue.bValue = bIsInput;
            bIsInput = true;
        }
    }


    public abstract class CombineInputBase : InputElement
    {
        InputElement pInputElement_A;
        InputElement pInputElement_B;
        EAxisValueCalculate eAxisValueCalculate = EAxisValueCalculate.A_Plus_B;

        protected SInputValue sValue_A, sValue_B;
        protected bool bIsInput_A, bIsInput_B;

        public CombineInputBase(InputElement a, InputElement b)
        {
            this.pInputElement_A = a;
            this.pInputElement_B = b;

            a.p_Event_OnInputElement.Subscribe += Event_OnInputElement;
            b.p_Event_OnInputElement.Subscribe += Event_OnInputElement;

            OnSet_InputElementID(a, b, out strInputElement_ID);

            _listInputElement_Child.Add(a);
            _listInputElement_Child.Add(b);
        }

        private void Event_OnInputElement(InputElement pElement, bool bIsInput, SInputValue sValue)
        {
            if (pInputElement_A == pElement)
            {
                bIsInput_A = bIsInput;
                sValue_A = sValue;
            }
            else
            {
                bIsInput_B = bIsInput;
                sValue_B = sValue;
            }
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            sValue = SInputValue.Dummy;
            OnCalculate_InputValue(out bIsInput, ref sValue);

            if (bIsInput)
            {
                switch (eAxisValueCalculate)
                {
                    // Axis 뿐만아니라 Vector 벨류나 Subvalue 등도 해야 함
                    case EAxisValueCalculate.A_Plus_B: sValue.fAxisValue_Minus1_1 = sValue_A.fAxisValue_Minus1_1 + sValue_B.fAxisValue_Minus1_1; break;
                    case EAxisValueCalculate.A_Only: sValue.fAxisValue_Minus1_1 = sValue_A.fAxisValue_Minus1_1; break;
                    case EAxisValueCalculate.B_Only: sValue.fAxisValue_Minus1_1 = sValue_B.fAxisValue_Minus1_1; break;

                    case EAxisValueCalculate.Extract_Vector2:
                        sValue.vecValue = new Vector2(sValue_A.fAxisValue_Minus1_1, sValue_B.fAxisValue_Minus1_1);
                        break;
                }
            }
        }

        public CombineInputBase DoSet_AxisValueCalculate(EAxisValueCalculate eAxisValueCalculate)
        {
            this.eAxisValueCalculate = eAxisValueCalculate;
            return this;
        }

        protected abstract void OnCalculate_InputValue(out bool bIsInput, ref SInputValue sValue);
        protected abstract void OnSet_InputElementID(InputElement a, InputElement b, out string strInputElementID);
    }


    public class CombineInput_OR : CombineInputBase
    {
        public CombineInput_OR(InputElement a, InputElement b) : base(a, b) { }

        protected override void OnSet_InputElementID(InputElement a, InputElement b, out string strInputElementID)
        {
            strInputElementID = string.Format("({0}_OR_{1})", a.strInputElement_ID, b.strInputElement_ID);
        }

        protected override void OnCalculate_InputValue(out bool bIsInput, ref SInputValue sValue)
        {
            bIsInput = bIsInput_A || bIsInput_B;
            sValue.bValue = sValue_A.bValue || sValue_B.bValue;
            sValue.vecValue = sValue_A.vecValue + sValue_B.vecValue;
        }
    }

    public class CombineInput_AND : CombineInputBase
    {
        public CombineInput_AND(InputElement a, InputElement b) : base(a, b) { }

        protected override void OnSet_InputElementID(InputElement a, InputElement b, out string strInputElementID)
        {
            strInputElementID = string.Format("({0}_AND_{1})", a.strInputElement_ID, b.strInputElement_ID);
        }

        protected override void OnCalculate_InputValue(out bool bIsInput, ref SInputValue sValue)
        {
            bIsInput = bIsInput_A && bIsInput_B;
            sValue.bValue = sValue_A.bValue && sValue_B.bValue;
            sValue.vecValue = sValue_A.vecValue + sValue_B.vecValue;
        }
    }

    // ====================================================================================

    public abstract class InputEventBase : IDictionaryItem<string>
    {
        public CObserverSubject<InputEventBase, bool, SInputValue> p_Event_OnInputEvent { get; private set; } = new CObserverSubject<InputEventBase, bool, SInputValue>();

        public bool bIsInputed { get; private set; }
        public bool bIsInputed_Prev { get; private set; }

        public SInputValue sValue { get; private set; }
        public SInputValue sValuePrev { get; private set; }

        public string strInputEvent_ID;

        public List<InputElement> listInputElement { get; private set; } = new List<InputElement>();

        public CommandWrapper pCommandWrapper;

        // ====================================================================================

        public void DoAdd_InputElement(InputElement pInputElement)
        {
            if (listInputElement.Contains(pInputElement) == false)
            {
                listInputElement.Add(pInputElement);
                pInputElement.DoAdd_InputEvent(this);
            }
        }

        public void DoRemove_InputElement(InputElement pInputElement)
        {
            listInputElement.Remove(pInputElement);
            OnRemove_InputElement(pInputElement);
        }

        public void DoClear_InputElement()
        {
            listInputElement.Clear();
        }

        public string IDictionaryItem_GetKey()
        {
            return strInputEvent_ID;
        }

        public void DoNoityInputEvent_IfRecieveInputElement()
        {
            if (pCommandWrapper.bEnable == false)
                return;

            bool bIsInput;
            string strInputElement;
            SInputValue sValue;
            DoCalculate_InputEvent(out bIsInput, out sValue, out strInputElement);
            sValue.strInputElement = strInputElement;
            p_Event_OnInputEvent.DoNotify(this, bIsInput, sValue);

            bIsInputed_Prev = bIsInputed;
            sValuePrev = this.sValue;

            bIsInputed = bIsInput;
            this.sValue = sValue;
        }

        abstract public void DoCalculate_InputEvent(out bool bIsInput, out SInputValue sValue, out string strInputElementName);
        virtual protected void OnRemove_InputElement(InputElement pInputElement) { }

        // ====================================================================================

        static StringBuilder _pStrBuilder = new StringBuilder();

        public override string ToString()
        {
            _pStrBuilder.Length = 0;
            if (pCommandWrapper.bEnable == false)
            {
                _pStrBuilder.Append("(Disabled)").Append(IDictionaryItem_GetKey()).Append("(Disabled)");
                return _pStrBuilder.ToString();
            }

            _pStrBuilder.Append(IDictionaryItem_GetKey());
            if (bIsInputed)
                _pStrBuilder.Append(" / Inputed");
            else
                _pStrBuilder.Append(" / Not Inputed");

            if (sValue.fAxisValue_Minus1_1 != 0f)
                _pStrBuilder.Append("/ AxisValue : " + sValue.fAxisValue_Minus1_1.ToString("F2"));

            if (sValue.bValue)
                _pStrBuilder.Append("/ bValue : " + sValue.bValue);

            return _pStrBuilder.ToString();
        }
    }

    [System.Serializable]
    public class InputEvent_Normal : InputEventBase
    {
        protected Dictionary<InputElement, bool> _mapInputElement_IsInput = new Dictionary<InputElement, bool>();
        protected Dictionary<InputElement, SInputValue> _mapInputElement_Value = new Dictionary<InputElement, SInputValue>();

        public void Event_OnInputElement(InputElement pInputElement, bool bIsInput, SInputValue sInputValue)
        {
            _mapInputElement_IsInput[pInputElement] = bIsInput;
            _mapInputElement_Value[pInputElement] = sInputValue;
        }

        public override void DoCalculate_InputEvent(out bool bIsInput, out SInputValue sValue, out string strInputElementName)
        {
            strInputElementName = "";
            bIsInput = false;
            sValue = SInputValue.Dummy;
            foreach (var pInputElement in _mapInputElement_IsInput)
            {
                if (pInputElement.Value)
                {
                    bIsInput = true;

                    SInputValue sValueCurrent = _mapInputElement_Value[pInputElement.Key];
                    if (sValueCurrent.bValue)
                        sValue.bValue = true;

                    if (sValueCurrent.fAxisValue_Minus1_1 != 0f)
                        sValue.fAxisValue_Minus1_1 += sValueCurrent.fAxisValue_Minus1_1;

                    if (sValueCurrent.vecValue.Equals(Vector2.zero) == false)
                        sValue.vecValue = sValueCurrent.vecValue;

                    strInputElementName += pInputElement.Key.IDictionaryItem_GetKey();
                }
            }
        }

        protected override void OnRemove_InputElement(InputElement pInputElement)
        {
            pInputElement.p_Event_OnInputElement.DoRemove_Listener(Event_OnInputElement);
        }
    }

    public class InputEvent_UIEvent : InputEventBase
    {
        UnityEngine.UI.Button pButtonTarget;
        bool bIsInputUI;

        public InputEvent_UIEvent(CUIObjectBase pUIObject_Panel, UnityEngine.UI.Button pButtonTarget)
        {
            this.pButtonTarget = pButtonTarget;
            this.strInputEvent_ID = string.Format("({0}_{1})", pUIObject_Panel.name, pButtonTarget.name);
            pButtonTarget.onClick.AddListener(Event_OnInput);
        }

        public void Event_OnInput()
        {
            bIsInputUI = true;
        }

        public override void DoCalculate_InputEvent(out bool bIsInput, out SInputValue fAxisValue, out string strInputElementName)
        {
            fAxisValue = SInputValue.Dummy;
            bIsInput = bIsInputUI;
            if (bIsInputUI)
                bIsInputUI = false;

            strInputElementName = pButtonTarget.name;
        }
    }

    // ====================================================================================

    public class Command_UIButton : CCommandBase
    {
        UnityEngine.Events.UnityAction OnExcute;

        string strCommandID;

        public Command_UIButton(UnityEngine.UI.Button pButtonTarget, UnityEngine.Events.UnityAction OnExcute)
        {
            this.OnExcute = OnExcute;
            this.strCommandID = pButtonTarget.name;

            bIsInit = true;
        }

        public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
        {
            OnExcute.Invoke();
        }

        public override string IDictionaryItem_GetKey()
        {
            return strCommandID;
        }
    }

    #endregion InnerClass

    // ====================================================================================

    public class CommandWrapper : IDictionaryItem<string>
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

        public List<InputEventBase> listInputEvent { get; private set; } = new List<InputEventBase>();

        public CCommandBase pCommand { get; private set; }
        public int iPriority => pCommand.iPriority;
        public string strCommandCategory => pCommand.strCommandCategory;

        public bool bEnable { get; private set; }
        public bool bIsIgnored { get; private set; }
        public bool bIsInputed { get; private set; }
        public bool bIsExcuted_ThisFrame { get; private set; }
        public SInputValue sValueLast { get; private set; }

        HashSet<CommandWrapper> listCommand_IgnoreOnExcute = new HashSet<CommandWrapper>();

        // ====================================================================================

        public CommandWrapper(CCommandBase pCommandBase)
        {
            pCommand = pCommandBase;
            DoSet_Enable(true);
        }

        public void DoSet_Enable(bool bEnable)
        {
            this.bEnable = bEnable;
        }

        public void DoSet_Ignore_True()
        {
            bIsIgnored = true;
            for (int i = 0; i < listInputEvent.Count; i++)
            {
                var listInputElement = listInputEvent[i].listInputElement;
                for (int j = 0; j < listInputElement.Count; j++)
                    listInputElement[j].Event_OnIgnored();
            }
        }

        public void DoSet_Ignore_False()
        {
            bIsIgnored = false;
        }

        public void DoAdd_IgnoreCommand(CommandWrapper pCommand_IgnoreOnExcute)
        {
            listCommand_IgnoreOnExcute.Add(pCommand_IgnoreOnExcute);
        }

        public void DoAdd_InputEvent(InputEventBase pInputEvent)
        {
            if(listInputEvent.Contains(pInputEvent) == false)
            {
                listInputEvent.Add(pInputEvent);

                pInputEvent.pCommandWrapper = this;
                pInputEvent.p_Event_OnInputEvent.Subscribe += Event_OnInputEvent;
            }
        }

        public void DoExcute(ref SInputValue sValue)
        {
            bIsExcuted_ThisFrame = pCommand.DoExcute(ref sValue);
            if(bIsExcuted_ThisFrame)
            {
                foreach (var pCommand_IgnoreOnExcute in listCommand_IgnoreOnExcute)
                    pCommand_IgnoreOnExcute.DoSet_Ignore_True();
            }
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
            DoSet_Ignore_False();
            bIsExcuted_ThisFrame = false;
        }

        public void Event_OnInputEvent(InputEventBase pInputEvent, bool bIsInput, SInputValue sValueLast)
        {
            if (bEnable == false)
                return;

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

            if (bEnable == false)
            {
                g_pStrBuilder.Append("/Disabled");
                return g_pStrBuilder.ToString();
            }

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

            if(sValueLast.fAxisValue_Minus1_1.Equals(0f) == false)
                g_pStrBuilder.Append("/AxisValue : ").Append(sValueLast.fAxisValue_Minus1_1.ToString("F2"));

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

    /* protected & private - Field declaration         */

    protected Dictionary<string, InputElement> _mapInputElement = new Dictionary<string, InputElement>();
    protected Dictionary<string, InputEventBase> _mapInputEvent = new Dictionary<string, InputEventBase>();
    protected Dictionary<string, CommandWrapper> _mapCommandWrapper = new Dictionary<string, CommandWrapper>();
    protected Dictionary<string, CommandWrapper> _mapCommandWrapper_Category = new Dictionary<string, CommandWrapper>();

    CFixedSizeList<ComandExcuted> _listExcutedCommand = new CFixedSizeList<ComandExcuted>(1024);
    Stack<ComandExcuted> _stackPrevNextCommand = new Stack<ComandExcuted>();
    protected List<CommandWrapper> _listCommandWrapper = new List<CommandWrapper>();

    // For Editor Debug
    Dictionary<InputElement, GameObject> _mapInputElementObject_ForDebug = new Dictionary<InputElement, GameObject>();
    Dictionary<InputEventBase, GameObject> _mapInputEventObject_ForDebug = new Dictionary<InputEventBase, GameObject>();

    Dictionary<EInnerClassType, Transform> _mapTransform_ForDebug = new Dictionary<EInnerClassType, Transform>()
    {
        { EInnerClassType.InputElement, null }, { EInnerClassType.InputEvent, null }, { EInnerClassType.Command, null }
    };

    HashSet<string> _setIgnoreCommandCategory = new HashSet<string>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoAddIngore_CommandCategory(string strCommandCategory)
    {
        _setIgnoreCommandCategory.Add(strCommandCategory);
    }

    public void DoRemoveIngore_CommandCategory(string strCommandCategory)
    {
        _setIgnoreCommandCategory.Remove(strCommandCategory);
    }

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

    public InputEventBase DoCreate_InputEvent_Normal(string strInputEvent_ID, params InputElement[] arrInputElement)
    {
        InputEvent_Normal pInputEvent = null;
        string strKey = strInputEvent_ID.ToString();
        if (_mapInputEvent.ContainsKey(strKey))
        {
            pInputEvent = _mapInputEvent[strKey] as InputEvent_Normal;
            if (pInputEvent == null)
            {
                Debug.LogError("InputManager - " + nameof(DoCreate_InputEvent_Normal) + "pInputEvent == null - strInputEvent_ID : " + strInputEvent_ID);
                return null;
            }
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
            if(pInputElement == null)
            {
                Debug.LogWarning("InputManager - " + nameof(DoCreate_InputEvent_Normal) + " -  strInputEvent_ID : " + strInputEvent_ID + " pInputElement == null");
                continue;
            }

            pInputEvent.DoAdd_InputElement(pInputElement);
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
        pCommandWrapper.DoAdd_InputEvent(pInputEvent);

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

    public InputElement GetInputElement(string strInputElement_ID)
    {
        InputElement pElement;
        _mapInputElement.TryGetValue(strInputElement_ID, out pElement);

        return pElement;
    }

    public InputEventBase GetInputEvent(string strInputEvent_ID)
    {
        InputEventBase pEvent;
        if (_mapInputEvent.TryGetValue(strInputEvent_ID, out pEvent) == false)
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
        {
            CommandWrapper pCommand = _listCommandWrapper[i];
            if(_setIgnoreCommandCategory.Contains(pCommand.strCommandCategory) == false)
                ExcuteCommand_IfInputEvent(pCommand);
        }

        for (int i = 0; i < _listCommandWrapper.Count; i++)
            _listCommandWrapper[i].DoReset();

        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            Print_Hierarchy_ForDebug(EInnerClassType.InputElement, _mapInputElement.Values, _mapInputElementObject_ForDebug, PrintHierarchy_InputElement);
            Print_Hierarchy_ForDebug(EInnerClassType.InputEvent, _mapInputEvent.Values, _mapInputEventObject_ForDebug, PrintHierarchy_InputEvent);
        }
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
        if (_mapInputElement.ContainsKey(strID) == false)
            _mapInputElement.Add(new VirtualInputElement(strInput_ID.ToString()));

        return _mapInputElement[strID];
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

        pTransform_ForDebug.name = strDebugObjectName + " /Input: " + iInputCount;
    }

    private bool PrintHierarchy_InputElement(InputElement pComponent, GameObject pObjectDebug)
    {
        pObjectDebug.name = pComponent.ToString();
        Print_InputElementList(pComponent._listInputElement_Child, pObjectDebug);

        return pComponent.bIsInputed;
    }

    private bool PrintHierarchy_InputEvent(InputEventBase pComponent, GameObject pObjectDebug)
    {
        pObjectDebug.name = pComponent.ToString();
        Print_InputElementList(pComponent.listInputElement, pObjectDebug);

        return pComponent.bIsInputed;
    }

    private void Print_InputElementList(List<InputElement> listInputElement, GameObject pObjectDebug)
    {
        for (int i = 0; i < listInputElement.Count; i++)
        {
            Transform pTransform = null;
            if (pObjectDebug.transform.childCount <= i)
            {
                GameObject pObjectNew = new GameObject();
                pTransform = pObjectNew.transform;
                pTransform.SetParent(pObjectDebug.transform);
            }
            else
            {
                pTransform = pObjectDebug.transform.GetChild(i);
            }

            InputElement pInputElement = listInputElement[i];
            pTransform.name = pInputElement.ToString();

            Print_InputElementList(pInputElement._listInputElement_Child, pTransform.gameObject);
        }
    }

    private bool PrintHierarchy_Command(CommandWrapper pComponent, GameObject pObjectDebug)
    {
        pObjectDebug.name = pComponent.ToString();
        return pComponent.bIsInputed;
    }

    #endregion Private
}
// ========================================================================== //


public struct SInputValue
{
    static public SInputValue Dummy => _Dummy;
    static SInputValue _Dummy = new SInputValue(0f, Vector2.zero, false);

    public bool bValue;
    public float fAxisValue_Minus1_1;
    public Vector2 vecValue;
    public string strInputElement;

    public SInputValue(float fAxisValue_0_1, Vector2 vecValue, bool bValue)
    {
        this.bValue = bValue;
        this.fAxisValue_Minus1_1 = fAxisValue_0_1;
        this.vecValue = vecValue;
        this.strInputElement = null;
    }

    public override string ToString()
    {
        return "SInputValue - bValue : " + bValue + " /fAxisValue_0_1 : " + fAxisValue_Minus1_1 + " /vecValue : " + vecValue;
    }
}

[System.Serializable]
public abstract class CCommandBase : IDictionaryItem<string>
{
    public CManagerCommand pManagerCommand { get; private set; }
    public bool bIsInit { get; protected set; }

    public virtual int iPriority { get; }
    public virtual string strCommandCategory { get; }

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

    abstract public void DoExcute(ref SInputValue sInputValue, ref bool bIsExcuted_DefaultIsTrue);

    virtual public void DoExcute_Undo(ref SInputValue sInputValue) { }
    virtual public void OnInitCommand(out bool bIsInit) { bIsInit = true; }
}


#region Test
#if UNITY_EDITOR

public class CommandTest_Print_1 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "1";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 1);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
}
public class CommandTest_Print_2 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "2";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 1);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
}
public class CommandTest_Print_3 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "3";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 1);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
}
public class CommandTest_Print_1_And_2 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "12";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 2, 2);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
}
public class CommandTest_Print_2_And_3 : CCommandBase
{
    public override void DoExcute(ref SInputValue sValue, ref bool bIsExcuted_DefaultIsTrue)
    {
        CManagerCommandTest.g_strBuffer += "23";
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Excute AxisValue : " + sValue.fAxisValue_Minus1_1);
    }
    public override void DoExcute_Undo(ref SInputValue sValue)
    {
        CManagerCommandTest.g_strBuffer = CManagerCommandTest.g_strBuffer.Remove(CManagerCommandTest.g_strBuffer.Length - 2, 2);
        Debug.Log(Time.time.ToString("F2") + " " + GetType().Name + " Undo AxisValue : " + sValue.fAxisValue_Minus1_1);
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
        base.OnAwake();

        _mapInputElement.Clear();
        _mapInputEvent.Clear();
        _mapCommandWrapper.Clear();
        _mapCommandWrapper_Category.Clear();
        _listCommandWrapper.Clear();

        Step1_OnRegist_InputElement();
        Step2_OnRegist_Command();

        _mapCommandWrapper.Values.ToList(_listCommandWrapper);
        _listCommandWrapper.Sort(CommandWrapper.Comparer);
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
    /// Command란 게임에 실질적으로 들어갈 로직입니다.
    /// </summary>
    /// <param name="listCommand_RegistHere"></param>

    protected void Step2_OnRegist_Command()
    {
        DoCreate_CommandWrapper(new CommandTest_Print_1(), DoCreate_InputEvent_Normal(EInputEventName.InputEvent_1.ToString(), GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_1))));
        DoCreate_CommandWrapper(new CommandTest_Print_2(), DoCreate_InputEvent_Normal(EInputEventName.InputEvent_2.ToString(), GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_2))));
        DoCreate_CommandWrapper(new CommandTest_Print_3(), DoCreate_InputEvent_Normal(EInputEventName.InputEvent_3.ToString(), GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_3))));
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

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_3].bIsInput = false;
        Assert.AreEqual(g_strBuffer, "123");
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
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = false;

        Assert.AreEqual(g_strBuffer, "12");
        pManagerTest.DoUndo_Command();
        pManagerTest.DoUndo_Command();

        Assert.AreEqual(g_strBuffer, "");
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
        pManagerTest.OnUpdate();
        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_1].bIsInput = false;

        mapVirtualInput_ForTest[EInputElementName.Virtual_KeyboardInput_2].bIsInput = true;
        pManagerTest.OnUpdate();
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
                Assert.IsTrue(fWaitSecond.IsSimilar(listWaitTime[iLoopCount], 0.3f));

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
        pManagerTest.EventOnAwake_Force();
        pManagerTest.DoInit_CommandAll();

        mapVirtualInput_ForTest = new Dictionary<EInputElementName, VirtualInputElement>();
        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_1, pManagerTest.GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_1)) as VirtualInputElement);
        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_2, pManagerTest.GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_2)) as VirtualInputElement);
        mapVirtualInput_ForTest.Add(EInputElementName.Virtual_KeyboardInput_3, pManagerTest.GetInputElement(nameof(EInputElementName.Virtual_KeyboardInput_3)) as VirtualInputElement);

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