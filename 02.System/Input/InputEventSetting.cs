#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-18 오후 5:22:57
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using Sirenix.Utilities.Editor;
using static CManagerCommand;
using System;
using System.Runtime.Remoting;
using System.Text;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class InputEventSetting : ScriptableObject
{
    public enum EOperatorType
    {
        ONLY,
        [RegistEnumSubString("_AND_")]
        AND,
        [RegistEnumSubString("_OR_")]
        OR,
    }
    
    [System.Serializable]
    public class InputElementWrapper
    {
        static StringBuilder g_pStrBuilder = new StringBuilder();
        static List<string> listStringTemp = new List<string>();

        [Rename_Inspector("Input Element A")] [ValueDropdown(nameof(GetInputElementName_A))] [HorizontalGroup("A")] [LabelWidth(120)]
        public string strInputElementName_A;

        [Rename_Inspector("Is Reverse")] [HorizontalGroup("A")] [LabelWidth(70)]
        public bool bIsReverse_A;

        [Rename_Inspector("버튼을 누른채로 홀드해야 동작")] [HorizontalGroup("Press_A")]
        public bool bSet_PressTime_A = false;
        [ShowIf(nameof(bSet_PressTime_A))] [HorizontalGroup("Press_A")] [Rename_Inspector("홀딩 타임(초)")]
        public float fPressTime_A = 0f;

        [Rename_Inspector("항상 Input을 들어오게 할지")]
        public bool bAlwaysCall_And_InputValue_Is_BoolValue_A;


        [LabelWidth(120)] [Rename_Inspector("A와 B의 관계")]
        public EOperatorType eOperatorType = EOperatorType.ONLY;

        [ShowIf(nameof(IsNot_Only_A))] [Rename_Inspector("Input Element B")] [ValueDropdown(nameof(GetInputElementName_B))] [HorizontalGroup("B")] [LabelWidth(120)]
        public string strInputElementName_B;

        [ShowIf(nameof(IsNot_Only_A))] [Rename_Inspector("Is Reverse")] [HorizontalGroup("B")] [LabelWidth(70)]
        public bool bIsReverse_B;

        [ShowIf(nameof(IsNot_Only_A))] [Rename_Inspector("버튼을 누른채로 홀드해야 동작")] [HorizontalGroup("Press_B")]
        public bool bSet_PressTime_B = false;
        [ShowIf(nameof(IsNot_Only_A_And_HasPressTime))] [HorizontalGroup("Press_B")] [Rename_Inspector("홀딩 타임(초)")]
        public float fPressTime_B = 0f;

        [ShowIf(nameof(IsNot_Only_A))] [Rename_Inspector("항상 Input을 들어오게 할지")]
        public bool bAlwaysCall_And_InputValue_Is_BoolValue_B;

        [ShowIf(nameof(IsNot_Only_A))]
        [Rename_Inspector("컴바인 후 추출값 옵션")]
        public InputElement.EAxisValueCalculate eAxisValueCalculate = InputElement.EAxisValueCalculate.A_Plus_B;

        protected InputElementGroup _pInputElementGroup_Owner;


        public InputElementWrapper(InputElementGroup pInputElementGroup_Owner)
        {
            _pInputElementGroup_Owner = pInputElementGroup_Owner;
        }

        public InputElementWrapper(InputElementGroup pInputElementGroup_Owner, InputElementWrapper pInputElementWrapper)
        {
            _pInputElementGroup_Owner = pInputElementGroup_Owner;

            strInputElementName_A = pInputElementWrapper.strInputElementName_A;
            bIsReverse_A = pInputElementWrapper.bIsReverse_A;
            bSet_PressTime_A = pInputElementWrapper.bSet_PressTime_A;
            bAlwaysCall_And_InputValue_Is_BoolValue_A = pInputElementWrapper.bAlwaysCall_And_InputValue_Is_BoolValue_A;

            eOperatorType = pInputElementWrapper.eOperatorType;

            strInputElementName_B = pInputElementWrapper.strInputElementName_B;
            bIsReverse_B = pInputElementWrapper.bIsReverse_B;
            bSet_PressTime_B = pInputElementWrapper.bSet_PressTime_B;
            fPressTime_B = pInputElementWrapper.fPressTime_B;
            bAlwaysCall_And_InputValue_Is_BoolValue_B = pInputElementWrapper.bAlwaysCall_And_InputValue_Is_BoolValue_B;

            eAxisValueCalculate = pInputElementWrapper.eAxisValueCalculate;
        }

        private string[] GetInputElementName_A()
        {
            return ProjectInputSetting.GetInputElementNameList();
        }

        private string[] GetInputElementName_B()
        {
            listStringTemp.Clear();
            listStringTemp.AddRange(ProjectInputSetting.GetInputElementNameList());

            if (_pInputElementGroup_Owner == null)
                return listStringTemp.ToArray();

            for (int i = 0; i < _pInputElementGroup_Owner.listInputElement.Count; i++)
            {
                // if (_pInputElementGroup_Owner.listInputElement[i] != this)
                // 같은 A를 가질 확률은 거의 없으므로 일단 이거로 체크..
                // 오딘에서 이 함수를 실행시키면 카피본이랑 비교해서 이상해진다..
                // 일단 다음 인덱스 아이템만 할 수 있게끔 적용
                if (_pInputElementGroup_Owner.listInputElement[i].strInputElementName_A == this.strInputElementName_A && i != _pInputElementGroup_Owner.listInputElement.Count - 1)
                    listStringTemp.Add(_pInputElementGroup_Owner.listInputElement[i + 1].ToString());
            }

            return listStringTemp.ToArray();
        }

        private bool IsNot_Only_A()
        {
            return eOperatorType != EOperatorType.ONLY;
        }

        private bool IsNot_Only_A_And_HasPressTime()
        {
            return IsNot_Only_A() && bSet_PressTime_B;
        }

        public override string ToString()
        {
            g_pStrBuilder.Length = 0;
            if (eOperatorType == EOperatorType.ONLY)
            {
                g_pStrBuilder.Append(strInputElementName_A);
            }
            else
            {
                g_pStrBuilder.Append("(").Append(strInputElementName_A).Append(eOperatorType.ToStringSub()).Append(strInputElementName_B).Append(")");
            }

            return g_pStrBuilder.ToString();
        }
    }

    [System.Serializable]
    public class InputElementGroup
    {
        static StringBuilder g_pStrBuilder = new StringBuilder();

        [LabelText("$" + nameof(Name))]
        [InfoBox("Error - listInputElement Is Invalid", InfoMessageType.Error, VisibleIf = nameof(IsInvalid_IgnoreList))]
        [ListDrawerSettings(CustomAddFunction = nameof(AddInputElement))]
        public List<InputElementWrapper> listInputElement = new List<InputElementWrapper>();

        public InputElementGroup()
        {
        }

        public InputElementGroup(InputElementGroup pInputElementGroup)
        {
            for (int i = 0; i < pInputElementGroup.listInputElement.Count; i++)
                listInputElement.Add(new InputElementWrapper(this, pInputElementGroup.listInputElement[i]));
        }

        public string Name
        {
            get
            {
                g_pStrBuilder.Length = 0;
                for(int i = 0; i < listInputElement.Count; i++)
                {
                    if (i == 0 || (i != 0 && Find_InputElement(listInputElement[i - 1], listInputElement[i - 1].ToString()) == false))
                    {
                        g_pStrBuilder.Append(listInputElement[i].ToString());
                    }
                    else if (i != listInputElement.Count - 1)
                    {
                        if (Find_InputElement(listInputElement[i + 1], listInputElement[i + 1].ToString()))
                            g_pStrBuilder.Append(listInputElement[i + 1].ToString());
                        else
                            g_pStrBuilder.Append(" || ");
                    }
                }

                return g_pStrBuilder.ToString();
            }
        }

        public override string ToString()
        {
            return Name;
        }


        public bool IsInvalid_IgnoreList()
        {
            var arr = ProjectInputSetting.GetInputElementNameList();
            for (int i = 0; i < listInputElement.Count; i++)
            {
                InputElementWrapper pInputElementWrapper = listInputElement[i];
                if (string.IsNullOrEmpty(pInputElementWrapper.strInputElementName_A))
                    return true;

                bool bIsFound = false;
                foreach (var pValue in arr)
                {
                    if (pValue.Equals(pInputElementWrapper.strInputElementName_A))
                    {
                        bIsFound = true;
                        break;
                    }
                }
                if (bIsFound == false)
                    return true;

                if (pInputElementWrapper.eOperatorType != EOperatorType.ONLY)
                {
                    if (string.IsNullOrEmpty(pInputElementWrapper.strInputElementName_B))
                        return true;

                    bIsFound = false;
                    foreach (var pValue in arr)
                    {
                        if (pValue.Equals(pInputElementWrapper.strInputElementName_B))
                        {
                            bIsFound = true;
                            break;
                        }
                    }

                    if(bIsFound == false)
                        bIsFound = Find_InputElement(pInputElementWrapper, pInputElementWrapper.strInputElementName_B);
                }

                if (bIsFound == false)
                    return true;
            }

            return false;
        }

        private bool Find_InputElement(InputElementWrapper pInputElementWrapper, string strEqualElementName)
        {
            foreach (var pInputElement in listInputElement)
            {
                if (pInputElement.ToString().Equals(strEqualElementName))
                    return true;
            }

            return false;
        }

        public void AddInputElement()
        {
            listInputElement.Add(new InputElementWrapper(this));
        }
    }

    [System.Serializable]
    public class InputEvent_Editor
    {
        [InfoBox("Error - CommandName Is Invalid", InfoMessageType.Error, VisibleIf = nameof(IsInvalid))]
        [ValueDropdown(nameof(GetCommandList))]
        [Rename_Inspector("Excute Command Name")]
        public string strCommandName;

        [Rename_Inspector("InputElementGroup")]
        [ListDrawerSettings(CustomAddFunction = nameof(AddInputElementGroup))]
        public List<InputElementGroup> InputElementGroupList = new List<InputElementGroup>();

        [GUIColor(1f, 0f, 0f, 1f )]
        [ValueDropdown(nameof(GetCommandList))]
        [InfoBox("Error - IgnoreList Is Invalid", InfoMessageType.Error, VisibleIf = nameof(IsInvalid_IgnoreList))]
        public List<string> listIgnoreCommandName = new List<string>();

        public InputEvent_Editor()
        {
        }

        public InputEvent_Editor(InputEvent_Editor pInputEvent)
        {
            strCommandName = pInputEvent.strCommandName;

            for(int i = 0; i < pInputEvent.InputElementGroupList.Count; i++)
                InputElementGroupList.Add(new InputElementGroup(pInputEvent.InputElementGroupList[i]));

            for(int i = 0; i < pInputEvent.listIgnoreCommandName.Count; i++)
                listIgnoreCommandName.Add(pInputEvent.listIgnoreCommandName[i]);
        }

        public void DoInit()
        {
            CManagerCommand pManagerCommand = CManagerCommand.instance;

            for (int i = 0; i < InputElementGroupList.Count; i++)
            {
                InputElementGroup pElementGroup = InputElementGroupList[i];
                for(int j = 0; j < pElementGroup.listInputElement.Count; j++)
                {
                    InputEventBase pInputEvent = null;
                    InputElementWrapper pInputElementWrapper = pElementGroup.listInputElement[j];

                    InputElement pElement_A = pManagerCommand.GetInputElement(pInputElementWrapper.strInputElementName_A);
                    if (pInputElementWrapper.bIsReverse_A)                              pElement_A = !pElement_A;
                    if (pInputElementWrapper.bSet_PressTime_A)                          pElement_A = pElement_A.DoSet_PressTime(pInputElementWrapper.fPressTime_A);
                    if (pInputElementWrapper.bAlwaysCall_And_InputValue_Is_BoolValue_A) pElement_A = pElement_A.DoAlwaysCall_And_InputValue_Is_BoolValue();

                    if(pInputElementWrapper.eOperatorType== EOperatorType.ONLY)
                    {
                        pInputEvent = pManagerCommand.DoCreate_InputEvent_Normal(strCommandName, pElement_A);
                    }
                    else
                    {
                        InputElement pElement_B = pManagerCommand.GetInputElement(pInputElementWrapper.strInputElementName_B);
                        if(pElement_B == null)
                        {
                            //pInputElementWrapper.strInputElementName_B = pInputElementWrapper.strInputElementName_B.Replace(" ", "");
                            //pInputElementWrapper.strInputElementName_B = pInputElementWrapper.strInputElementName_B.Replace("&&", "_AND_");
                            //pInputElementWrapper.strInputElementName_B = pInputElementWrapper.strInputElementName_B.Replace("||", "_OR_");

                            pElement_B = pManagerCommand.GetInputElement(pInputElementWrapper.strInputElementName_B);
                            if(pElement_B == null)
                                Debug.LogError("Not Found Input Element" + pInputElementWrapper.strInputElementName_B);
                        }

                        if (pInputElementWrapper.bIsReverse_B) pElement_B = !pElement_B;
                        if (pInputElementWrapper.bSet_PressTime_B) pElement_B = pElement_B.DoSet_PressTime(pInputElementWrapper.fPressTime_B);
                        if (pInputElementWrapper.bAlwaysCall_And_InputValue_Is_BoolValue_B) pElement_B = pElement_B.DoAlwaysCall_And_InputValue_Is_BoolValue();

                        CombineInputBase pInputEvent_Combine = null;
                        if (pInputElementWrapper.eOperatorType == EOperatorType.OR)
                            pInputEvent_Combine = pElement_A | pElement_B;
                        if (pInputElementWrapper.eOperatorType == EOperatorType.AND)
                            pInputEvent_Combine = pElement_A & pElement_B;

                        if (pInputEvent_Combine != null)
                            pInputEvent = pManagerCommand.DoCreate_InputEvent_Normal(strCommandName, pInputEvent_Combine.DoSet_AxisValueCalculate(pInputElementWrapper.eAxisValueCalculate));
                    }
                    
                    if (pInputEvent != null && string.IsNullOrEmpty(strCommandName) == false)
                    {
                        object pCommand = Activator.CreateInstance(ProjectInputSetting.GetCommandType(strCommandName));
                        pManagerCommand.DoCreate_CommandWrapper(pCommand as CCommandBase, pInputEvent);
                    }
                }
            }
        }

        public void DoInit_Second()
        {
            CManagerCommand pManagerCommand = CManagerCommand.instance;
            for(int i = 0; i < listIgnoreCommandName.Count; i++)
            {
                CommandWrapper pCommandWrapper = pManagerCommand.GetCommandWrapper(listIgnoreCommandName[i]);
                if(pCommandWrapper != null)
                    pManagerCommand.GetCommandWrapper(strCommandName).DoAdd_IgnoreCommand(pCommandWrapper);
            }
        }

        IEnumerable GetCommandList()
        {
            return ProjectInputSetting.GetCommandList();
        }

        public bool IsInvalid()
        {
            var arr = ProjectInputSetting.GetCommandList();
            if (string.IsNullOrEmpty(strCommandName))
                return true;

            foreach(var pValue in arr)
            {
                if (pValue.Equals(strCommandName))
                    return false;
            }

            return true;
        }

        public bool IsInvalid_IgnoreList()
        {
            var arr = ProjectInputSetting.GetCommandList();
            for (int i = 0; i < listIgnoreCommandName.Count; i++)
            {
                if (string.IsNullOrEmpty(listIgnoreCommandName[i]))
                    return true;

                bool bIsFound = false;
                foreach (var pValue in arr)
                {
                    if (pValue.Equals(listIgnoreCommandName[i]))
                    {
                        bIsFound = true;
                        break;
                    }
                }

                if(bIsFound == false)
                    return true;
            }

            return false;
        }

        public void AddInputElementGroup()
        {
            InputElementGroupList.Add(new InputElementGroup());
        }
    }

    // ========================================================================== //

    [Rename_Inspector("현재 세팅된 Input Event List")]
    [ListDrawerSettings(OnBeginListElementGUI = nameof(BeginDrawListElement), OnEndListElementGUI = nameof(EndDrawListElement))]
    public List<InputEvent_Editor> p_listEvent = new List<InputEvent_Editor>();

    [ShowIf(nameof(CheckIsOpend_Editor))]
    [InlineButton(nameof(Load_From_File_Into_Setting), "Load")]
    [Rename_Inspector("로드할 파일")]
    public InputEventSetting p_pSetting_ForLoad;

    // ========================================================================== //

    public InputEventSetting(List<InputEvent_Editor> listEvent)
    {
        p_listEvent = listEvent;
    }

    public void DoInit()
    {
        for (int i = 0; i < p_listEvent.Count; i++)
            p_listEvent[i].DoInit();

        for (int i = 0; i < p_listEvent.Count; i++)
            p_listEvent[i].DoInit_Second();
    }

    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Add Input Event List", ButtonSizes.Large)]
    public void Add_InputEvent()
    {
        if(p_listEvent.Count == 0)
        {
            p_listEvent.Add(new InputEvent_Editor());
        }
        else
        {
            InputEvent_Editor pInputEventLast = p_listEvent[p_listEvent.Count - 1];
            p_listEvent.Add(new InputEvent_Editor(pInputEventLast));
        }
    }

    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Save to File", ButtonSizes.Large)]
    public void Save_To_File_From_Setting()
    {
        Debug.Log(nameof(Save_To_File_From_Setting));

        string strPath = EditorUtility.SaveFilePanelInProject("Save", nameof(InputEventSetting), "asset", "Please enter a file name to save");
        if (strPath.Length != 0)
            ScriptableObjectUtility.CreateAsset(new InputEventSetting(p_listEvent), strPath);
    }

    public void Load_From_File_Into_Setting()
    {
        Debug.Log(nameof(Load_From_File_Into_Setting));

        if (p_pSetting_ForLoad == null)
        {
            Debug.LogError("로드할 파일이 없습니다");
            return;
        }

        ProjectInputSetting.DoSet_InputEventSetting(Instantiate(p_pSetting_ForLoad));
    }

    public bool CheckIsOpend_Editor()
    {
        return ProjectInputSetting.Instance.p_pInputEventSetting == this;
    }

    // ========================================================================== //

    private void BeginDrawListElement(int index)
    {
        SirenixEditorGUI.BeginBox(this.p_listEvent[index].strCommandName);
    }

    private void EndDrawListElement(int index)
    {
        SirenixEditorGUI.EndBox();
    }
}
#endif