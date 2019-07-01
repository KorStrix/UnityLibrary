#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-13 오후 4:45:34
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static InputInfo;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[System.Serializable]
public class InputInfoList : IEnumerable<InputInfo> // : List<CommandInfo> 원래 List를 상속받아 구현하려 했으나, Unity Serialize가 동작하지 않는다..
{
    public List<InputInfo> listCommandInfo = new List<InputInfo>();
    public CommandListBase pCommandListTarget;

    public IEnumerator<InputInfo> GetEnumerator()
    {
        return listCommandInfo.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return listCommandInfo.GetEnumerator();
    }
}


/// <summary>
/// 
/// </summary>
[System.Serializable]
public class InputInfo : ISerializationCallbackReceiver
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EInputRelateType
    {
        None,
        [RegistSubString("&&")]
        And,
        [RegistSubString("||")]
        Or,
    }

    [System.Serializable]
    public class InputInfoElement : CRelateByOther<InputInfoElement, EInputRelateType, SInputValue>
    {
        public enum EInputInfoType
        {
            GetKeyDown,
            GetKey,
            GetKeyUp,

            GetAxis,

            GetMouseButtonDown,
            GetMouseButton,
            GetMouseButtonUp,
        }

        public enum EMouseButtonType
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2,
        }

        public string strInputAxisName;
        public bool bAlwaysInput;
        public EInputInfoType eInputInfoType;

        public KeyCode eKeyCode;
        public EMouseButtonType eMouseButtonType;

        public bool IsMouseInput()
        {
            return eInputInfoType == EInputInfoType.GetMouseButtonDown || eInputInfoType == EInputInfoType.GetMouseButton || eInputInfoType == EInputInfoType.GetMouseButtonUp;
        }


        public override SInputValue GetResult()
        {
            SInputValue pInputValue = new SInputValue();
            pInputValue.bIsInput = false;

            switch (eInputInfoType)
            {
                case EInputInfoType.GetKeyDown:
                    pInputValue.bIsInput = Input.GetKeyDown(eKeyCode);
                    break;

                case EInputInfoType.GetKey:
                    pInputValue.bIsAlwaysInput = true;
                    pInputValue.bIsInput = Input.GetKey(eKeyCode);
                    break;

                case EInputInfoType.GetKeyUp:
                    pInputValue.bIsInput = Input.GetKeyUp(eKeyCode);
                    break;

                case EInputInfoType.GetAxis:
                    pInputValue.bIsAlwaysInput = true;
                    pInputValue.fAxisValue_Minus1_1 = Input.GetAxis(strInputAxisName);
                    pInputValue.bIsInput = pInputValue.fAxisValue_Minus1_1 != 0f;
                    break;

                case EInputInfoType.GetMouseButtonDown:
                    pInputValue.bIsInput = Input.GetMouseButtonDown((int)eMouseButtonType);
                    break;

                case EInputInfoType.GetMouseButton:
                    pInputValue.bIsAlwaysInput = true;
                    pInputValue.bIsInput = Input.GetMouseButton((int)eMouseButtonType);
                    break;

                case EInputInfoType.GetMouseButtonUp:
                    pInputValue.bIsInput = Input.GetMouseButtonUp((int)eMouseButtonType);
                    break;
            }
            pInputValue.bValue = pInputValue.bIsInput;

            if(pInputValue.bIsAlwaysInput == false)
                pInputValue.bIsAlwaysInput = bAlwaysInput;

            if (bAlwaysInput)
                pInputValue.bIsInput = true;

            return pInputValue;
        }

        public override string IRelateByOther_GetDisplayName()
        {
            if(eInputInfoType == EInputInfoType.GetAxis)
                return $"({strInputAxisName}.{eInputInfoType})";
            else
                return $"({eKeyCode}.{eInputInfoType})";
        }

        public override string IRelateByOther_GetDisplayRelateName(EInputRelateType pRelateType)
        {
            return pRelateType.ToStringSub();
        }

        public override ValueDropdownList<EInputRelateType> IRelateByOther_GetEditorDisplayNameList()
        {
            return OdinExtension.GetValueDropDownList_EnumSubString<EInputRelateType>();
        }

        public override bool IRelateByOther_IsRequireOtherItem()
        {
            return pRelateType != EInputRelateType.None;
        }
    }

    [System.Serializable]
    public class InputInfoElementGroup : RelatedByElementsListBase<InputInfoElement, EInputRelateType, SInputValue>
    {
        protected override SInputValue Calculate_Relation(SInputValue pResult_A, EInputRelateType pRelateType, SInputValue pResult_B)
        {
            switch (pRelateType)
            {
                case EInputRelateType.And: return pResult_A & pResult_B;
                case EInputRelateType.Or: return pResult_A | pResult_B;
            }

            return pResult_A;
        }
    }

    /* public - Field declaration            */

    public string strAssemblyName; // Assembly가 다르면 GetType을 통해 얻어올수가 없다..
    public string strCommandTypeName;

    public CommandBase pCommand; // Editor Code에선 Serialize가 안되면 인스펙터에 그려도 세팅이 되지 않는다. 그래서 Init()을 통해 따로 세팅
    public InputInfoElementGroup pInputInfoGroup;

    /* protected & private - Field declaration         */

    public void OnBeforeSerialize()
    {
        InitCommandInstance();
    }

    public void OnAfterDeserialize()
    {
        InitCommandInstance();
    }


    private void InitCommandInstance()
    {
        if (string.IsNullOrEmpty(strAssemblyName) == false &&
            string.IsNullOrEmpty(strCommandTypeName) == false &&
            pCommand == null)
        {
            System.Type pCommandType = System.Type.GetType(strCommandTypeName);
            if(pCommandType == null)
            {
                System.Reflection.Assembly pAssembly = System.Reflection.Assembly.Load(strAssemblyName);
                pCommandType = pAssembly.GetType(strCommandTypeName);
            }

            if (pCommandType != null)
                pCommand = System.Activator.CreateInstance(pCommandType) as CommandBase;
        }
    }
}

#region EditorCode
#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(InputInfoElement))]
public class InputInfoElement_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InputInfoElement pDrawTarget = EditorCodeHelper.GetTargetObjectOfProperty(property) as InputInfoElement;

        position.height = EditorCodeHelper.singleLineHeight;
        property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, "Input: " + pDrawTarget.IRelateByOther_GetDisplayName());
        if (property.isExpanded == false)
            return;
        position.y += EditorCodeHelper.singleLineHeight;

        EditorCodeHelper.PropertyField(position, property.FindPropertyRelative(nameof(pDrawTarget.eInputInfoType)), "InputType");
        position.y += EditorCodeHelper.singleLineHeight;

        if (pDrawTarget.eInputInfoType == InputInfoElement.EInputInfoType.GetAxis)
        {
            var listInput = InputSetting.GetInput_ValueDropDownList();
            pDrawTarget.strInputAxisName = EditorCodeHelper.DrawEnumPopup(position, "Select Input", pDrawTarget.strInputAxisName, listInput);
            position.y += EditorCodeHelper.singleLineHeight;
        }
        else if(pDrawTarget.IsMouseInput())
        {
            EditorCodeHelper.PropertyField(position, property.FindPropertyRelative(nameof(pDrawTarget.eMouseButtonType)), "Mouse");
            position.y += EditorCodeHelper.singleLineHeight;
        }
        else
        {
            float fOriginX = position.x;
            float fOriginWidth = position.width;
            position.width *= 0.5f;
            position.x += position.width;

            var pKeyCodeProperty = property.FindPropertyRelative(nameof(pDrawTarget.eKeyCode));
            if (pKeyCodeProperty.isExpanded) // Is Recording..
            {
                if (UpdateKeyCode(property, pDrawTarget))
                    pKeyCodeProperty.isExpanded = false;

                if (GUI.Button(position, "Cancel Recording.."))
                {
                    pKeyCodeProperty.isExpanded = false;
                }
            }
            else
            {
                if (GUI.Button(position, "Record KeyCode"))
                {
                    pKeyCodeProperty.isExpanded = true;
                }
            }

            position.x = fOriginX;
            position.width = fOriginWidth;
            position.y += EditorCodeHelper.singleLineHeight;

            pKeyCodeProperty.intValue = (int)pDrawTarget.eKeyCode;
            EditorCodeHelper.PropertyField(position, pKeyCodeProperty, "KeyCode");
            position.y += EditorCodeHelper.singleLineHeight;
        }

        EditorCodeHelper.PropertyField(position, property.FindPropertyRelative(nameof(pDrawTarget.bAlwaysInput)), "Is Always Input");
        position.y += EditorCodeHelper.singleLineHeight;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if(property.isExpanded)
            return EditorCodeHelper.singleLineHeight * 5;
        else
            return EditorCodeHelper.singleLineHeight * 1;
    }

    private bool UpdateKeyCode(SerializedProperty property, InputInfoElement pDrawTarget)
    {
        Event pCurrentEvent = Event.current;
        if (pCurrentEvent == null)
            return false;

        if ((pCurrentEvent.type == EventType.KeyDown || pCurrentEvent.type == EventType.MouseDown) == false)
            return false;

        KeyCode eKeycode = pCurrentEvent.keyCode;
        if (eKeycode != KeyCode.None)
        {
            pDrawTarget.eKeyCode = eKeycode;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
            return true;
        }

        return false;
    }
}

[CustomPropertyDrawer(typeof(InputInfoElementGroup))]
public class InputInfoElementGroup_Drawer : RelatedByElementsList_Drawer<InputInfoElement, EInputRelateType, SInputValue>
{
}

[CustomPropertyDrawer(typeof(InputInfoList))]
public class InputInfoList_Drawer : PropertyDrawer
{
    HashSet<KeyCode> _setCurrentInput = new HashSet<KeyCode>();
    ReorderableList _listCommandInfo;

    /// <summary>
    /// ReorderableList 안에 ReorderableList를 그리면, 자식 ReorderableListDrawer의 인스턴스는 한개이기 때문에, ReorderableListDrawer의 멤버에 있는 ReorderableList가 항상 변합니다.
    /// 그래서 상위 ReorderableListDrawer에서 자식 ReorderableListDrawer의 ReorderableList를 변수에 담아 관리해야 합니다.
    /// </summary>
    Dictionary<object, ReorderableList> _mapInputInfo = new Dictionary<object, ReorderableList>();

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        InputInfoElementGroup_Drawer.p_Event_OnGetReorderableList.Subscribe += OnGetReorderableList;

        SerializedObject pSO = property.serializedObject;
        SerializedProperty pSP_ListCommandInfo = Get_List_SerializedProperty(property);
        InputInfoList pDrawTarget = EditorCodeHelper.GetTargetObjectOfProperty(property) as InputInfoList;

        if (_listCommandInfo == null)
        {
            _mapInputInfo.Clear();

            _listCommandInfo = new ReorderableList(pSO, pSP_ListCommandInfo, true, true, true, true);
            _listCommandInfo.drawElementCallback = DrawElement(pSO, pDrawTarget);

            _listCommandInfo.drawHeaderCallback = (Rect rect) =>
            {
                EditorCodeHelper.LabelField(rect, property.displayName);
            };

            _listCommandInfo.elementHeightCallback = (int index) =>
            {
                var element = _listCommandInfo.serializedProperty.GetArrayElementAtIndex(index);
                if (element.isExpanded)
                    return GetElementHeight(element);
                else
                    return EditorCodeHelper.singleLineHeight;
            };
        }

        _listCommandInfo.DoList(position);
        pSO.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float fHeight = EditorCodeHelper.singleLineHeight * 5;
        InputInfo listDrawTarget = EditorCodeHelper.GetTargetObjectOfProperty(property) as InputInfo;
        SerializedProperty pSP_ListCommandInfo = Get_List_SerializedProperty(property);
        if (pSP_ListCommandInfo != null)
        {
            for (int i = 0; i < pSP_ListCommandInfo.arraySize; i++)
            {
                SerializedProperty pProperty = pSP_ListCommandInfo.GetArrayElementAtIndex(i);
                if (pProperty.isExpanded)
                    fHeight += GetElementHeight(pProperty);
                else
                    fHeight += EditorCodeHelper.singleLineHeight;
            }
        }

        return fHeight;
    }

    private ReorderableList.ElementCallbackDelegate DrawElement(SerializedObject pSO, InputInfoList pDrawTarget)
    {
        return (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            InputInfo pCommandInfo = pDrawTarget.listCommandInfo[index];

            rect.width -= 15f;
            rect.x += 10f;

            float fOrigin_RectX = rect.x;
            float fOriginRectWidth = rect.width;

            var pSP_Element = _listCommandInfo.serializedProperty.GetArrayElementAtIndex(index);
            var pProeprty_ListInputInfoList = pSP_Element.FindPropertyRelative(nameof(pCommandInfo.pInputInfoGroup));
            InputInfoElementGroup pInputInfoGroup = EditorCodeHelper.GetTargetObjectOfProperty(pProeprty_ListInputInfoList) as InputInfoElementGroup;

            rect.height = EditorCodeHelper.singleLineHeight;
            rect.width *= 0.5f;
            rect.x += rect.width;

            if (pDrawTarget.pCommandListTarget != null)
            {
                System.Type pCommandType = null;
                var listCommandType = pDrawTarget.pCommandListTarget.GetCommandList();
                foreach(var pType in listCommandType)
                {
                    if(pType.Value.FullName.Equals(pCommandInfo.strCommandTypeName))
                    {
                        pCommandType = pType.Value;
                        break;
                    }
                }

                int iSelectIndex = EditorGUI.Popup(rect, listCommandType.Calculate_SelectIndex(pCommandType), listCommandType.GetNameList());
                pCommandType = listCommandType[iSelectIndex].Value;
                if(pCommandType != null)
                {
                    pCommandInfo.strCommandTypeName = pCommandType.FullName;
                    pCommandInfo.strAssemblyName = pCommandType.Assembly.FullName;
                }

                if (GUI.changed)
                    EditorUtility.SetDirty(pSO.targetObject);
            }
            else
                EditorGUI.LabelField(rect, "Command is Null");

            rect.width = fOriginRectWidth;
            rect.x = fOrigin_RectX;
            pSP_Element.isExpanded = EditorCodeHelper.Foldout(rect, pSP_Element.isExpanded, "Command");
            if (pSP_Element.isExpanded)
            {
                rect.y += EditorCodeHelper.singleLineHeight;
                EditorGUI.PropertyField(rect, pProeprty_ListInputInfoList, true);
            }
        };
    }

    private void OnGetReorderableList(SerializedProperty pProperty, ReorderableList pValue_Origin, ref ReorderableList pValue_Current)
    {
        object pPropertyData = EditorCodeHelper.GetTargetObjectOfProperty(pProperty);
        if (_mapInputInfo.ContainsKey(pPropertyData) == false)
            _mapInputInfo[pPropertyData] = InputInfoElementGroup_Drawer.Get_ReorderableList(pProperty);

        pValue_Current = _mapInputInfo[pPropertyData];
    }

    private static SerializedProperty Get_List_SerializedProperty(SerializedProperty property)
    {
        InputInfoList plist;
        return property.FindPropertyRelative(nameof(plist.listCommandInfo));
    }

    private float GetElementHeight(SerializedProperty element)
    {
        float fHeight = EditorCodeHelper.singleLineHeight;
        if (element.isExpanded == false)
            element.isExpanded = true;

        fHeight += EditorGUI.GetPropertyHeight(element, true) + EditorCodeHelper.singleLineHeight * 2;
        return fHeight;
    }
}


#endif
#endregion EditorCode