#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-18 오전 11:29:27
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;


[System.Serializable]
public class InputAxis
{
    const string strNativeDetail = "NativeDetail";
    const string strJoystick = "Joystick";

    public enum AxisType
    {
        KeyOrMouseButton = 0,
        MouseMovement = 1,
        JoystickAxis = 2
    };

#if UNITY_EDITOR
    [InfoBox("Error", InfoMessageType.Error, VisibleIf = nameof(CheckIs_InValidAxis))]
#endif
    [Rename_Inspector("인풋 ID")]
    public string strInputID;

    [HorizontalGroup("2")]
    public string positiveButton;
    [HorizontalGroup("2")]
    public AxisType type;


    #region NativeDetail
    [FoldoutGroup(strNativeDetail, expanded: false)] public string negativeButton;

    [FoldoutGroup(strNativeDetail)] public string descriptiveName;
    [FoldoutGroup(strNativeDetail)] public string descriptiveNegativeName;

    [FoldoutGroup(strNativeDetail)] public string altNegativeButton;
    [FoldoutGroup(strNativeDetail)] public string altPositiveButton;

    [FoldoutGroup(strNativeDetail)] public float gravity;
    [FoldoutGroup(strNativeDetail)] public float dead;
    [FoldoutGroup(strNativeDetail)] public float sensitivity = 1f;

    [FoldoutGroup(strNativeDetail)] public bool snap = false;
    [FoldoutGroup(strNativeDetail)] public bool invert = false;

#if UNITY_EDITOR
    [ShowIf(nameof(CheckIs_JoyStickAxis))]
#endif
    [Header("Joystick axis")]
    public int axis;

#if UNITY_EDITOR
    [ShowIf(nameof(CheckIs_JoyStickAxis))]
#endif
    public int joyNum;

    #endregion NativeDetail

    public InputAxis() { }

#if UNITY_EDITOR
    public InputAxis(SerializedProperty pAxesProperty)
    {
        strInputID = GetChildProperty(pAxesProperty, "m_Name").stringValue;
        descriptiveName = GetChildProperty(pAxesProperty, "descriptiveName").stringValue;
        descriptiveNegativeName = GetChildProperty(pAxesProperty, "descriptiveNegativeName").stringValue;
        negativeButton = GetChildProperty(pAxesProperty, "negativeButton").stringValue;
        positiveButton = GetChildProperty(pAxesProperty, "positiveButton").stringValue;
        altNegativeButton = GetChildProperty(pAxesProperty, "altNegativeButton").stringValue;
        altPositiveButton = GetChildProperty(pAxesProperty, "altPositiveButton").stringValue;
        gravity = GetChildProperty(pAxesProperty, "gravity").floatValue;
        dead = GetChildProperty(pAxesProperty, "dead").floatValue;
        sensitivity = GetChildProperty(pAxesProperty, "sensitivity").floatValue;
        snap = GetChildProperty(pAxesProperty, "snap").boolValue;
        invert = GetChildProperty(pAxesProperty, "invert").boolValue;
        type = (AxisType)GetChildProperty(pAxesProperty, "type").intValue;
        axis = GetChildProperty(pAxesProperty, "axis").intValue + 1;
        joyNum = GetChildProperty(pAxesProperty, "joyNum").intValue;
    }

    public void DoSave_ToProperty(SerializedProperty pAxesProperty)
    {
        GetChildProperty(pAxesProperty, "m_Name").stringValue = strInputID;
        GetChildProperty(pAxesProperty, "descriptiveName").stringValue = descriptiveName;
        GetChildProperty(pAxesProperty, "descriptiveNegativeName").stringValue = descriptiveNegativeName;
        GetChildProperty(pAxesProperty, "negativeButton").stringValue = negativeButton;
        GetChildProperty(pAxesProperty, "positiveButton").stringValue = positiveButton;
        GetChildProperty(pAxesProperty, "altNegativeButton").stringValue = altNegativeButton;
        GetChildProperty(pAxesProperty, "altPositiveButton").stringValue = altPositiveButton;
        GetChildProperty(pAxesProperty, "gravity").floatValue = gravity;
        GetChildProperty(pAxesProperty, "dead").floatValue = dead;
        GetChildProperty(pAxesProperty, "sensitivity").floatValue = sensitivity;
        GetChildProperty(pAxesProperty, "snap").boolValue = snap;
        GetChildProperty(pAxesProperty, "invert").boolValue = invert;
        GetChildProperty(pAxesProperty, "type").intValue = (int)type;
        GetChildProperty(pAxesProperty, "axis").intValue = axis - 1;
        GetChildProperty(pAxesProperty, "joyNum").intValue = joyNum;
    }

    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty pProperty = parent.Copy();
        pProperty.Next(true);
        do
        {
            if (pProperty.name == name)
                return pProperty;
        }
        while (pProperty.Next(false));

        return null;
    }

    private bool CheckIs_InValidAxis()
    {
        return string.IsNullOrEmpty(strInputID) || (type != AxisType.JoystickAxis && string.IsNullOrEmpty(positiveButton));
    }

    private bool CheckIs_JoyStickAxis()
    {
        return type == AxisType.JoystickAxis;
    }

#endif
}

[System.Serializable]
public class InputSetting : ScriptableObject
{
    const string const_strCurrentSetting = "현재 프로젝트에 세팅된 값";
    const string const_ProjectSetting = "About Project Setting";
    const string const_File = "About File";

    // ========================================================================== //

    [TitleGroup(const_strCurrentSetting, order: 0)]
    [InfoBox("유니티의 기본 프로젝트 세팅과 같은 값입니다.\n캐치할 키보드 입력을 세팅합니다.\n세이브 및 로드를 할 수 있습니다.\n", InfoMessageType.Info)]
    [Rename_Inspector("현재 세팅된 Input List")]
    public List<InputAxis> p_listInput = new List<InputAxis>();

    [TitleGroup("Save & Load", order: 1)]
    [ShowIf(nameof(CheckIsOpend_Editor))]
    [InlineButton(nameof(Load_From_File), "Load")]
    [Rename_Inspector("로드할 파일")]
    public InputSetting p_pConfig_ForLoad;

    // ========================================================================== //

    public InputSetting(List<InputAxis> listInput)
    {
        p_listInput = listInput;
    }

    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Save To Project Setting", ButtonSizes.Large)]
    [HorizontalGroup("1")]
    public void Save_To_Current_ProjectSetting_Clear()
    {
#if UNITY_EDITOR
        Debug.Log(nameof(Save_To_Current_ProjectSetting_Clear));

        if (p_listInput == null)
        {
            Debug.LogError(nameof(Save_To_Current_ProjectSetting_Clear) + "- _pInputConfig == null || _pInputConfig.listInput == null");
            return;
        }

        SerializedObject pSerializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty pAxesProperty = pSerializedObject.FindProperty("m_Axes");
        pAxesProperty.ClearArray();
        pSerializedObject.ApplyModifiedProperties();

        AddAxis();
#endif
    }

    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Add To Project Setting", ButtonSizes.Large)]
    [HorizontalGroup("1")]
    public void Save_To_Current_ProjectSetting_Add()
    {
        Debug.Log(nameof(Save_To_Current_ProjectSetting_Add));

        if (p_listInput == null)
        {
            Debug.LogError(nameof(Save_To_Current_ProjectSetting_Add) + "- _pInputConfig == null || _pInputConfig.listInput == null");
            return;
        }

        AddAxis();
    }



    [TitleGroup("Save & Load")]
    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Load From Project Setting", ButtonSizes.Large)]
    [HorizontalGroup("2")]
    public void Load_From_ProjectSetting()
    {
        Debug.Log(nameof(Load_From_ProjectSetting));

        ProjectInputSetting.DoUpdate_InputSetting_From_ProjectSetting();
    }

    [TitleGroup("Save & Load")]
    [ShowIf(nameof(CheckIsOpend_Editor))]
    [Button("Save To File", ButtonSizes.Large)]
    [HorizontalGroup("2")]
    public void Save_To_File()
    {
        Debug.Log(nameof(Save_To_File));

#if UNITY_EDITOR
        string strPath = EditorUtility.SaveFilePanelInProject("Save", nameof(InputSetting), "asset", "Please enter a file name to save");
        if (strPath.Length != 0)
            ScriptableObjectUtility.CreateAsset(new InputSetting(p_listInput), strPath);
#endif
    }

    public void Load_From_File()
    {
        Debug.Log(nameof(Load_From_File));

        if(p_pConfig_ForLoad == null)
        {
            Debug.LogError("로드할 파일이 없습니다");
            return;
        }

        ProjectInputSetting.DoSet_InputSetting(Instantiate(p_pConfig_ForLoad));
    }

    // ========================================================================== //

    private void AddAxis()
    {
        for (int i = 0; i < p_listInput.Count; i++)
            ProjectInputSetting.AddAxis(p_listInput[i]);
    }

    public bool CheckIsOpend_Editor()
    {
        return ProjectInputSetting.Instance.p_pInputSetting == this;
    }
}
#endif
