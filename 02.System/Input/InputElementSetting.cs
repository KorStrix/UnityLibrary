#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-18 오전 11:37:00
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using static CManagerCommand;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public class InputElementSetting : ScriptableObject
{
    [System.Serializable]
    public class InputElement_Editor : IDictionaryItem<string>
    {
        public enum EInputElementOption
        {
            KeyboardInput,
            MouseDragInput,
        }

        public enum EInputElementType
        {
            Button,
            ButtonDown,
            ButtonUp,
            GetAxis,

            GetAxis_Detail_Greater,
            GetAxis_Detail_Greater_ABS,
            GetAxis_Detail_Lesser,
            GetAxis_Detail_Lesser_ABS,
        }

        [HorizontalGroup("1")]
        [Rename_Inspector("인풋 타입")]
        public EInputElementOption p_eInputElementOption = EInputElementOption.KeyboardInput;

        [HorizontalGroup("1")]
        [LabelWidth(70)]
        [Rename_Inspector("Enable")]
        public bool bEnable = true;

        [InfoBox("Error - InputID Is Invalid", InfoMessageType.Error, VisibleIf = nameof(IsInvalidID))]
        [Rename_Inspector("Input Element Name")]
        public string strInputElementName;

        [Rename_Inspector("Input ID")]
        [ValueDropdown(nameof(GetInputID))]
        [HorizontalGroup("2")]
        public string strInputID;

        [ShowIf(nameof(CheckIs_KeyboardInput))]
        [Rename_Inspector("타입")]
        [HorizontalGroup("2")]
        public EInputElementType eType = EInputElementType.ButtonDown;

        [ShowIf(nameof(IsGetAxisDetail))]
        [Rename_Inspector("비교할 값")]
        public float fComparisonValue;

        [ShowIf(nameof(IsGetAxis))]
        [Rename_Inspector("축 값을 반전시킬지")]
        public bool bAxisReverse;

        public InputElement DoCreate_InputElement()
        {
            InputElement pInputElement = null;
            if(p_eInputElementOption == EInputElementOption.KeyboardInput)
            {
                switch (eType)
                {
                    case EInputElementType.Button:                      pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key(strInputElementName, strInputID, EInputType.Button); break;
                    case EInputElementType.ButtonDown:                  pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key(strInputElementName, strInputID, EInputType.ButtonDown); break;
                    case EInputElementType.ButtonUp:                    pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key(strInputElementName, strInputID, EInputType.ButtonUp); break;
                    case EInputElementType.GetAxis:                     pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key(strInputElementName, strInputID, EInputType.GetAxis); break;


                    case EInputElementType.GetAxis_Detail_Greater:      pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key_AxisDetail(strInputElementName, strInputID, EGetAxisDetail.Greater, fComparisonValue); break;
                    case EInputElementType.GetAxis_Detail_Greater_ABS:  pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key_AxisDetail(strInputElementName, strInputID, EGetAxisDetail.Greater_ABS, fComparisonValue); break;
                    case EInputElementType.GetAxis_Detail_Lesser:       pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key_AxisDetail(strInputElementName, strInputID, EGetAxisDetail.Lesser, fComparisonValue); break;
                    case EInputElementType.GetAxis_Detail_Lesser_ABS:   pInputElement = CManagerCommand.instance.DoCreate_InputElement_Key_AxisDetail(strInputElementName, strInputID, EGetAxisDetail.Lesser_ABS, fComparisonValue); break;
                }
            }
            else if(p_eInputElementOption == EInputElementOption.MouseDragInput)
            {
                pInputElement = CManagerCommand.instance.DoCreate_InputElement_MouseDrag(strInputElementName, strInputID);
            }

            if(bAxisReverse)
                pInputElement = pInputElement.DoSet_ReverseAxis();
            pInputElement.bEnable = bEnable;

            return pInputElement;
        }

        private string[] GetInputID()
        {
            return ProjectInputSetting.GetInputID();
        }

        public bool IsGetAxis()
        {
            return CheckIs_KeyboardInput() && eType >= EInputElementType.GetAxis;
        }

        public bool IsGetAxisDetail()
        {
            return CheckIs_KeyboardInput() && eType >= EInputElementType.GetAxis_Detail_Greater;
        }

        public bool IsInvalidID()
        {
            var IDs = ProjectInputSetting.GetInputID();
            if (string.IsNullOrEmpty(strInputID))
                return true;
            
            for(int i = 0; i < IDs.Length; i++)
            {
                if (IDs[i].Equals(strInputID))
                    return false;
            }

            return true;
        }

        public bool CheckIs_KeyboardInput()
        {
            return p_eInputElementOption == EInputElementOption.KeyboardInput;
        }

        public string IDictionaryItem_GetKey()
        {
            if(bAxisReverse)
                return strInputElementName + InputElement.const_strRevereAxis;
            else
                return strInputElementName;
        }
    }

    // ========================================================================== //

    [Rename_Inspector("현재 세팅된 Input Element List")]
    public List<InputElement_Editor> p_listInputElement = new List<InputElement_Editor>();

    [ShowIf(nameof(CheckIsOpend_Editor))]
    [InlineButton(nameof(Load_From_File_Into_Setting), "Load")]
    [Rename_Inspector("로드할 파일")]
    public InputElementSetting p_pConfig_ForLoad;

    Dictionary<string, InputElement_Editor> _mapCashed_InputElement = new Dictionary<string, InputElement_Editor>();

    // ========================================================================== //

    public InputElementSetting(List<InputElement_Editor> listInputElement)
    {
        p_listInputElement = listInputElement;
    }

    public void DoInit()
    {
        _mapCashed_InputElement = new Dictionary<string, InputElement_Editor>();
        _mapCashed_InputElement.DoAddItem(p_listInputElement);
    }

    public InputElement DoCreate_InputElement(string strInputElementName)
    {
        if (_mapCashed_InputElement.ContainsKey(strInputElementName))
            return _mapCashed_InputElement[strInputElementName].DoCreate_InputElement();
        else
            return null;
    }

    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Save to File", ButtonSizes.Large)]
    public void Save_To_File_From_Setting()
    {
#if UNITY_EDITOR
        Debug.Log(nameof(Save_To_File_From_Setting));

        string strPath = EditorUtility.SaveFilePanelInProject("Save", nameof(InputElementSetting), "asset", "Please enter a file name to save");
        if (strPath.Length != 0)
            ScriptableObjectUtility.CreateAsset(new InputElementSetting(p_listInputElement), strPath);
#endif
    }

    [HorizontalGroup("Enable Button")]
    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Enable All InputElement", ButtonSizes.Large)]
    public void AllButton_Enable()
    {
        for (int i = 0; i < p_listInputElement.Count; i++)
            p_listInputElement[i].bEnable = true;
    }

    [HorizontalGroup("Enable Button")]
    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Disable All InputElement", ButtonSizes.Large)]
    public void AllButton_Disable()
    {
        for (int i = 0; i < p_listInputElement.Count; i++)
            p_listInputElement[i].bEnable = false;
    }

    public void Load_From_File_Into_Setting()
    {
        Debug.Log(nameof(Load_From_File_Into_Setting));

        if (p_pConfig_ForLoad == null)
        {
            Debug.LogError("로드할 파일이 없습니다");
            return;
        }

        ProjectInputSetting.DoSet_InputElementSetting(Instantiate(p_pConfig_ForLoad));
    }

    public bool CheckIsOpend_Editor()
    {
        return ProjectInputSetting.Instance.p_pInputElementSetting == this;
    }
}

#else

[System.Serializable]
public class InputElementSetting : ScriptableObject
{
}
#endif