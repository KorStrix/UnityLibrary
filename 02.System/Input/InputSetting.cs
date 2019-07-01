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

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

[System.Serializable]
public class EditorInputInfo
{
    const string strNativeDetail = "NativeDetail";
    const string strJoystick = "Joystick";

    public enum AxisType
    {
        KeyOrMouseButton = 0,
        MouseMovement = 1,
        JoystickAxis = 2
    };

#if ODIN_INSPECTOR && UNITY_EDITOR
    [InfoBox("Error", InfoMessageType.Error, VisibleIf = nameof(CheckIs_InValidAxis))]
#endif
    [DisplayName("인풋 ID")]
    public string strInputName;

    public string positiveButton;
    public AxisType type;


    #region NativeDetail
#if ODIN_INSPECTOR
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
#else
    public string negativeButton;

    public string descriptiveName;
    public string descriptiveNegativeName;

    public string altNegativeButton;
    public string altPositiveButton;

    public float gravity;
    public float dead;
    public float sensitivity = 1f;

    public bool snap = false;
    public bool invert = false;
#endif

#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowIf(nameof(CheckIs_JoyStickAxis))]
#endif
    [Header("Joystick axis")]
    public int axis;

#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowIf(nameof(CheckIs_JoyStickAxis))]
#endif
    public int joyNum;

    #endregion NativeDetail

    public bool bDetailSettingEditor;

    public EditorInputInfo() { }

#if UNITY_EDITOR
    public EditorInputInfo(SerializedProperty pAxesProperty)
    {
        strInputName = GetChildProperty(pAxesProperty, "m_Name").stringValue;
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
        GetChildProperty(pAxesProperty, "m_Name").stringValue = strInputName;
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
        return string.IsNullOrEmpty(strInputName) || (type != AxisType.JoystickAxis && string.IsNullOrEmpty(positiveButton));
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
    [DisplayName("현재 세팅된 Input List")]
    public List<EditorInputInfo> p_listInput = new List<EditorInputInfo>();
    
    [DisplayName("로드할 파일")]
    public InputSetting p_pConfig_ForLoad;

    // ========================================================================== //

#if UNITY_EDITOR
    static public ValueDropdownList<string> GetInput_ValueDropDownList()
    {
        ValueDropdownList<string> list = new ValueDropdownList<string>();
        var pProperty = GetAxesProperty();
        for (int i = 0; i < pProperty.arraySize; i++)
        {
            var pInputAxisProperty = pProperty.GetArrayElementAtIndex(i);
            if (pInputAxisProperty != null)
            {
                EditorInputInfo pInputInfo = new EditorInputInfo(pInputAxisProperty);
                list.Add(pInputInfo.strInputName, pInputInfo.strInputName);
            }
            else
                break;
        }

        return list;
    }

    public static SerializedProperty GetAxesProperty()
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        return serializedObject.FindProperty("m_Axes");
    }

    public void DoUpdate_InputSetting_From_ProjectSetting()
    {
        if (p_listInput == null)
            p_listInput = new List<EditorInputInfo>();
        p_listInput.Clear();

        var pProperty = GetAxesProperty();
        for (int i = 0; i < pProperty.arraySize; i++)
        {
            var pInputAxisProperty = pProperty.GetArrayElementAtIndex(i);
            if (pInputAxisProperty != null)
                p_listInput.Add(new EditorInputInfo(pInputAxisProperty));
            else
                break;
        }
    }

    public void DoSave_To_Current_ProjectSetting_Clear()
    {

        if (p_listInput == null)
        {
            Debug.LogError(nameof(DoSave_To_Current_ProjectSetting_Clear) + "- _pInputConfig == null || _pInputConfig.listInput == null");
            return;
        }

        SerializedObject pSerializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty pAxesProperty = pSerializedObject.FindProperty("m_Axes");
        pAxesProperty.ClearArray();
        pSerializedObject.ApplyModifiedProperties();

        AddAxis();
    }

    // ========================================================================== //


    public static void AddAxis(EditorInputInfo pInputAxis)
    {
        if (AxisDefined(pInputAxis.strInputName))
            return;

        SerializedObject pSerializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty pAxesProperty = pSerializedObject.FindProperty("m_Axes");

        pAxesProperty.arraySize++;
        pSerializedObject.ApplyModifiedProperties();

        SetAxis(pSerializedObject, pAxesProperty, pInputAxis, pAxesProperty.arraySize - 1);
    }

    protected static bool AxisDefined(string axisName)
    {
        SerializedProperty axesProperty = GetAxesProperty();

        axesProperty.Next(true);
        axesProperty.Next(true);
        while (axesProperty.Next(false))
        {
            SerializedProperty axis = axesProperty.Copy();
            axis.Next(true);
            if (axis.stringValue == axisName)
                return true;
        }
        return false;
    }

    protected static void SetAxis(SerializedObject pSerializedObject, SerializedProperty pAxesProperty, EditorInputInfo pInputAxis, int iIndex)
    {
        pInputAxis.DoSave_ToProperty(pAxesProperty.GetArrayElementAtIndex(iIndex));
        pSerializedObject.ApplyModifiedProperties();
    }

    private void AddAxis()
    {
        for (int i = 0; i < p_listInput.Count; i++)
            AddAxis(p_listInput[i]);
    }
#endif
}
