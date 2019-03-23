#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-04 오후 12:06:04
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// 
/// </summary>
public partial class CManagerCommand : CSingletonMonoBase<CManagerCommand>, IUpdateAble
{
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


        static StringBuilder g_pStrBuilder = new StringBuilder();

        public CObserverSubject<InputElement, bool, SInputValue> p_Event_OnInputElement { get; private set; } = new CObserverSubject<InputElement, bool, SInputValue>();
        public bool bIsInputed { get; private set; }
        public SInputValue sValueLast { get; private set; }

        public string strInputElement_ID;

        protected float _fPressWaitTime;
        protected float _fPressTimeStart;
        protected bool _bIsInputPressed;
        protected bool _bAlwaysCall_And_InputValue_Is_BValue = false;

        public void DoCheck_IsReceved()
        {
            bool bIsInput;
            SInputValue sValue;
            DoCalculate_InputValue(out bIsInput, out sValue);

            if (sValue.vecValue.Equals(Vector2.zero) == false && strInputElement_ID.Contains("Teleport"))
            {
            }


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
            g_pStrBuilder.Append(IDictionaryItem_GetKey());

            if (bIsInputed)
                g_pStrBuilder.Append("/Inputed");
            else
                g_pStrBuilder.Append("/Not Inputed");

            if(sValueLast.fAxisValue_0_1 != 0f)
                g_pStrBuilder.Append("/AxisValue : " + sValueLast.fAxisValue_0_1.ToString("F2"));

            if(sValueLast.vecValue.Equals(Vector2.zero) == false)
                g_pStrBuilder.Append("/vecValue : " + sValueLast.vecValue.ToString("F2"));

            if (_fPressWaitTime != 0f)
            {
                g_pStrBuilder.Append("/PressWait : " + _fPressWaitTime.ToString("F2"));

                if(_bIsInputPressed)
                    g_pStrBuilder.Append("/Pressing : " + (Time.time - _fPressTimeStart).ToString("F2"));
            }

            return g_pStrBuilder.ToString();
        }

        abstract public void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue);
        public virtual InputElement DoSet_PressTime(float fPressTime) { return null; }
        public virtual InputElement DoAlwaysCall_And_InputValue_Is_BoolValue() { _bAlwaysCall_And_InputValue_Is_BValue = true; return this; }

        // ====================================================================================

        public static InputElement_Reverse operator !(InputElement a)
        {
            return new InputElement_Reverse(a);
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

        bool _bIsInputPressPrev;

        public override InputElement DoSet_PressTime(float fPressTime)
        {
            KeyInputElement pInputNew = new KeyInputElement();
            pInputNew.strInputElement_ID = this.strInputElement_ID + "/PressTime:" + fPressTime + "s/";
            pInputNew.eInputType = this.eInputType;
            pInputNew.eAxisDetail = this.eAxisDetail;
            pInputNew.strInputName = this.strInputName;
            pInputNew.fComparisonValue = this.fComparisonValue;
            pInputNew._fPressWaitTime = fPressTime;
            pInputNew._fPressTimeStart = -1f;

            return pInputNew;
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            sValue = SInputValue.Dummy;

            switch (eInputType)
            {
                case EInputType.ButtonDown: _bIsInputPressed = Input.GetButtonDown(strInputName); break;
                case EInputType.ButtonUp: _bIsInputPressed = Input.GetButtonUp(strInputName); break;
                case EInputType.Button: _bIsInputPressed = Input.GetButton(strInputName); break;

                case EInputType.GetAxis:
                    {
                        sValue.fAxisValue_0_1 = Input.GetAxis(strInputName);
                        switch (eAxisDetail)
                        {
                            case EGetAxisDetail.Normal:
                                _bIsInputPressed = true;
                                break;

                            case EGetAxisDetail.Greater:
                                _bIsInputPressed = sValue.fAxisValue_0_1 > fComparisonValue;
                                break;

                            case EGetAxisDetail.Greater_ABS:
                                _bIsInputPressed = Mathf.Abs(sValue.fAxisValue_0_1) > fComparisonValue;
                                break;

                            case EGetAxisDetail.Lesser:
                                _bIsInputPressed = sValue.fAxisValue_0_1 < fComparisonValue;
                                break;

                            case EGetAxisDetail.Lesser_ABS:
                                _bIsInputPressed = Mathf.Abs(sValue.fAxisValue_0_1) < fComparisonValue;
                                break;
                        }
                    }

                    break;
            }

            bIsInput = Calculate_IsInput();
            sValue.bValue = bIsInput;

            if (_bAlwaysCall_And_InputValue_Is_BValue)
                bIsInput = true;
        }

        private bool Calculate_IsInput()
        {
            bool bIsInput;
            float fCurrentTime = Time.time;

            if (_fPressWaitTime == 0f)
            {
                bIsInput = _bIsInputPressed;
            }
            else
            {
                if (_fPressTimeStart == -1f || _bIsInputPressed == false)
                    bIsInput = false;
                else
                    bIsInput = _fPressTimeStart + _fPressWaitTime < fCurrentTime;

                if (_bIsInputPressPrev != _bIsInputPressed)
                {
                    _bIsInputPressPrev = _bIsInputPressed;
                    if (_bIsInputPressed)
                        _fPressTimeStart = fCurrentTime;
                    else
                        _fPressTimeStart = -1f;
                }
            }

            return bIsInput;
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
                sValue.fAxisValue_0_1 = Vector3.Distance(Input.mousePosition, _vecMousePosOrigin);
            }
        }
    }

    public class InputElement_Reverse : InputElement
    {
        InputElement _pInputElement_Original;

        public InputElement_Reverse(InputElement pInputElement_Original)
        {
            _pInputElement_Original = pInputElement_Original;
            _pInputElement_Original.p_Event_OnInputElement.Subscribe += OnInputElement;

            strInputElement_ID = string.Format("({0}_Reverse)", pInputElement_Original.strInputElement_ID);
        }

        private void OnInputElement(InputElement pInputElement, bool bInput, SInputValue sValue)
        {
            sValue.bValue = !sValue.bValue;
            p_Event_OnInputElement.DoNotify(this, !bInput, sValue);
        }

        public override void DoCalculate_InputValue(out bool bIsInput, out SInputValue sValue)
        {
            _pInputElement_Original.DoCalculate_InputValue(out bIsInput, out sValue);
            bIsInput= !bIsInput;
        }
    }

    public abstract class CombineInputBase : InputElement
    {
        InputElement pInputElement_A;
        InputElement pInputElement_B;
        EAxisValueCalculate eAxisValueCalculate = EAxisValueCalculate.A_Plus_B;

        protected SInputValue sValue_A, sValue_B;
        protected bool bIsInput_A, bIsInput_B;
        bool bIsLock;


        public CombineInputBase(InputElement a, InputElement b)
        {
            this.pInputElement_A = a;
            this.pInputElement_B = b;

            a.p_Event_OnInputElement.Subscribe += Event_OnInputElement;
            b.p_Event_OnInputElement.Subscribe += Event_OnInputElement;

            OnSet_InputElementID(a, b, out strInputElement_ID);
        }

        private void Event_OnInputElement(InputElement pElement, bool bIsInput, SInputValue sValue)
        {
            if (bIsLock)
                return;

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

            if (bIsInput_A && bIsInput_B)
            {
                bIsLock = true;
                pInputElement_A.p_Event_OnInputElement.DoNotify(pInputElement_A, false, SInputValue.Dummy);
                pInputElement_B.p_Event_OnInputElement.DoNotify(pInputElement_B, false, SInputValue.Dummy);
                bIsLock = false;
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
                    case EAxisValueCalculate.A_Plus_B: sValue.fAxisValue_0_1 = sValue_A.fAxisValue_0_1 + sValue_B.fAxisValue_0_1; break;
                    case EAxisValueCalculate.A_Only: sValue.fAxisValue_0_1 = sValue_A.fAxisValue_0_1; break;
                    case EAxisValueCalculate.B_Only: sValue.fAxisValue_0_1 = sValue_B.fAxisValue_0_1; break;

                    case EAxisValueCalculate.Extract_Vector2:
                        sValue.vecValue = new Vector2(sValue_A.fAxisValue_0_1, sValue_B.fAxisValue_0_1);
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

        // ====================================================================================

        public string IDictionaryItem_GetKey()
        {
            return strInputEvent_ID;
        }

        public void DoNoityInputEvent_IfRecieveInputElement()
        {
            bool bIsInput;
            SInputValue sValue;
            DoCalculate_InputEvent(out bIsInput, out sValue);
            p_Event_OnInputEvent.DoNotify(this, bIsInput, sValue);

            bIsInputed_Prev = bIsInputed;
            sValuePrev = this.sValue;

            bIsInputed = bIsInput;
            this.sValue = sValue;
        }

        abstract public void DoCalculate_InputEvent(out bool bIsInput, out SInputValue sValue);

        // ====================================================================================

        public static InputEventBase operator &(InputEventBase a, InputEventBase b)
        {
            return new CombineInputEvent_AND(a, b);
        }

        static StringBuilder _pStrBuilder = new StringBuilder();

        public override string ToString()
        {
            _pStrBuilder.Length = 0;
            _pStrBuilder.Append(IDictionaryItem_GetKey());
            if (bIsInputed)
                _pStrBuilder.Append(" / Inputed");
            else
                _pStrBuilder.Append(" / Not Inputed");

            if(sValue.fAxisValue_0_1 != 0f)
                _pStrBuilder.Append("/ AxisValue : " + sValue.fAxisValue_0_1.ToString("F2"));

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

        public override void DoCalculate_InputEvent(out bool bIsInput, out SInputValue sValue)
        {
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
                    
                    if (Mathf.Abs(sValue.fAxisValue_0_1) < Mathf.Abs(sValueCurrent.fAxisValue_0_1))
                        sValue.fAxisValue_0_1 = sValueCurrent.fAxisValue_0_1;

                    if (sValueCurrent.vecValue.Equals(Vector2.zero) == false)
                        sValue.vecValue = sValueCurrent.vecValue;
                }
            }
        }
    }

    public class InputEvent_UIEvent : InputEventBase
    {
        bool bIsInputUI;

        public InputEvent_UIEvent(CUIObjectBase pUIObject_Panel, UnityEngine.UI.Button pButtonTarget)
        {
            this.strInputEvent_ID = string.Format("({0}_{1})", pUIObject_Panel.name, pButtonTarget.name);
            pButtonTarget.onClick.AddListener(Event_OnInput);
        }

        public void Event_OnInput()
        {
            bIsInputUI = true;
        }

        public override void DoCalculate_InputEvent(out bool bIsInput, out SInputValue fAxisValue)
        {
            fAxisValue = SInputValue.Dummy;
            bIsInput = bIsInputUI;
            if (bIsInputUI)
                bIsInputUI = false;
        }
    }

    public class CombineInputEvent_AND : InputEventBase
    {
        InputEventBase pInputEvent_A;
        InputEventBase pInputEvent_B;

        bool bIsInput_A, bIsInput_B;
        SInputValue sValue_A, sValue_B;

        public CombineInputEvent_AND(InputEventBase a, InputEventBase b)
        {
            pInputEvent_A = a;
            pInputEvent_B = b;

            a.p_Event_OnInputEvent.Subscribe += Event_OnInputElement_Subscribe;
            b.p_Event_OnInputEvent.Subscribe += Event_OnInputElement_Subscribe;

            strInputEvent_ID = string.Format("({0}_AND_{1})", a.strInputEvent_ID, b.strInputEvent_ID);
        }
        
        public override void DoCalculate_InputEvent(out bool bIsInput, out SInputValue sValue)
        {
            bIsInput = bIsInput_A && bIsInput_B;
            sValue = SInputValue.Dummy;
            sValue.bValue = sValue_A.bValue && sValue_B.bValue;

            if (bIsInput)
                sValue.fAxisValue_0_1 = sValue_A.fAxisValue_0_1 + sValue_B.fAxisValue_0_1;
        }

        private void Event_OnInputElement_Subscribe(InputEventBase pInput, bool bIsInput, SInputValue sValue)
        {
            if (pInputEvent_A == pInput)
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
}