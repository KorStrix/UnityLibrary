#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-19 오후 4:43:42
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#endif

/// <summary>
/// 
/// </summary>
#if ODIN_INSPECTOR
public class ProjectInputSetting : CSingletonSOBase<ProjectInputSetting>
{
    static public CObserverSubject p_Event_OnChangeSetting { get; private set; } = new CObserverSubject();

    [Rename_Inspector("Input Setting")]
    public InputSetting p_pInputSetting;
    [Rename_Inspector("Input Element Setting")]
    public InputElementSetting p_pInputElementSetting;
    [Rename_Inspector("Input Event Setting")]
    public InputEventSetting p_pInputEventSetting;

    [ShowInInspector]
    [Rename_Inspector("Command List Setting")]
    public CommandListBase p_pCommandList;

    static List<string> _listTemp = new List<string>();


    // ========================================================================== //

    public static System.Type GetCommandType(string strTypeName)
    {
        return Instance.p_pCommandList.GetCommandType_OrNull(strTypeName);
    }

    // 파일이 존재하는지 확인해야 한다.. 파일이 아니면 컴파일 후 날라감..
    public static void DoSet_InputSetting(InputSetting pSetting)
    {
        Instance.p_pInputSetting = pSetting;
        p_Event_OnChangeSetting.DoNotify();
    }

    public static void DoSet_InputElementSetting(InputElementSetting pSetting)
    {
        Instance.p_pInputElementSetting = pSetting;
        p_Event_OnChangeSetting.DoNotify();
    }

    public static void DoSet_InputEventSetting(InputEventSetting pSetting)
    {
        Instance.p_pInputEventSetting = pSetting;
        p_Event_OnChangeSetting.DoNotify();
    }

    public void DoInit()
    {
        p_pInputElementSetting.DoInit();
        if(p_pInputEventSetting == null)
        {

        }

        p_pInputEventSetting.DoInit(this);
    }


    // ========================================================================== //

    public static IEnumerable GetCommandList()
    {
        if (Instance.p_pCommandList != null)
            return Instance.p_pCommandList.GetCommandList();
        else
            return null;
    }

    public static void DoUpdate_InputSetting_From_ProjectSetting()
    {
#if UNITY_EDITOR
        var pInputSetting = Instance.p_pInputSetting;
        if (pInputSetting.p_listInput == null)
            pInputSetting.p_listInput = new List<InputAxis>();
        pInputSetting.p_listInput.Clear();

        var pProperty = GetAxesProperty();
        for (int i = 0; i < pProperty.arraySize; i++)
        {
            var pInputAxisProperty = pProperty.GetArrayElementAtIndex(i);
            if (pInputAxisProperty != null)
                pInputSetting.p_listInput.Add(new InputAxis(pInputAxisProperty));
            else
                break;
        }
#endif
    }

    public static string[] GetInputID()
    {
        _listTemp.Clear();
        InputSetting pInputSetting = Instance.p_pInputSetting;
        if (pInputSetting.p_listInput != null)
        {
            for (int i = 0; i < pInputSetting.p_listInput.Count; i++)
                _listTemp.Add(pInputSetting.p_listInput[i].strInputID);
        }

        return _listTemp.ToArray();
    }

    public static string[] GetInputElementNameList()
    {
        _listTemp.Clear();

        InputElementSetting pInputElementSetting = Instance.p_pInputElementSetting;
        if (pInputElementSetting.p_listInputElement != null)
        {
            for (int i = 0; i < pInputElementSetting.p_listInputElement.Count; i++)
                _listTemp.Add(pInputElementSetting.p_listInputElement[i].IDictionaryItem_GetKey());
        }

        return _listTemp.ToArray();
    }


    public static void AddAxis(InputAxis pInputAxis)
    {
#if UNITY_EDITOR
        if (AxisDefined(pInputAxis.strInputID))
            return;

        SerializedObject pSerializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        SerializedProperty pAxesProperty = pSerializedObject.FindProperty("m_Axes");

        pAxesProperty.arraySize++;
        pSerializedObject.ApplyModifiedProperties();

        SetAxis(pSerializedObject, pAxesProperty, pInputAxis, pAxesProperty.arraySize - 1);
#endif
    }


#if UNITY_EDITOR
    public static SerializedProperty GetAxesProperty()
    {
        SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        return serializedObject.FindProperty("m_Axes");
    }

    // ========================================================================== //

    // ========================================================================== //

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

    protected static void SetAxis(SerializedObject pSerializedObject, SerializedProperty pAxesProperty, InputAxis pInputAxis, int iIndex)
    {
        pInputAxis.DoSave_ToProperty(pAxesProperty.GetArrayElementAtIndex(iIndex));
        pSerializedObject.ApplyModifiedProperties();
    }
#endif
}

#else
public class ProjectInputSetting
{
    public static ProjectInputSetting Instance;

    public void DoInit() { }
}
#endif
