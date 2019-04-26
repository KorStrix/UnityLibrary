#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-24 오전 11:08:53
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using System;
using System.Reflection;

#if UNITY_EDITOR

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
#endif

[CreateAssetMenu(menuName = "StrixSO/" + nameof(HierachyIIconList))]
public class HierachyIIconList : CScriptableObject
{
    [System.Serializable]
    public class IconMappingDataBase
    {
#if ODIN_INSPECTOR
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        [LabelText("대상 텍스쳐")]
#endif
        public Texture2D pTexture;
#if ODIN_INSPECTOR
        [LabelText("아이콘 순서 높을수록 왼쪽")]
#endif
        public int iOrder;
    }


    [System.Serializable]
    public class IconMappingData_Type : IconMappingDataBase, IDictionaryItem<string>
    {
#if ODIN_INSPECTOR
        [LabelText("대상 타입 이름")]
        [ValueDropdown(nameof(GetTypeName))]
#endif
        public string strTypeName;

        public string IDictionaryItem_GetKey()
        {
            return strTypeName;
        }

        public IEnumerable<string> GetTypeName()
        {
            System.Type pComponentType = typeof(Component);

            return pComponentType.Assembly.GetTypes().Union(typeof(CObjectBase).Assembly.GetTypes())
                    .Where(x => pComponentType.IsAssignableFrom(x))
                    .Select(x => x.GetFriendlyName());
        }
    }

    [System.Serializable]
    public class IconMappingData_Tag : IconMappingDataBase, IDictionaryItem<string>
    {
#if ODIN_INSPECTOR
        [LabelText("대상 태그 이름")]
        [ValueDropdown(nameof(GetTagName))]
#endif
        public string strTag;

        public string IDictionaryItem_GetKey()
        {
            return strTag;
        }

        public IEnumerable<string> GetTagName()
        {
            return UnityEditorInternal.InternalEditorUtility.tags;
        }
    }

    // ========================================================================== //

    [Rename_Inspector("타입별 아이콘")]
    public List<IconMappingData_Type> p_listIconMappingData_Type = new List<IconMappingData_Type>();

    [Rename_Inspector("태그별 아이콘")]
    public List<IconMappingData_Tag> p_listIconMappingData_Tag = new List<IconMappingData_Tag>();

    // ========================================================================== //

#if ODIN_INSPECTOR
    [Button]
#endif
    public void UpdateIcon()
    {
        HierachyIconConfig.instance.UpdateIcon();
    }

}
#endif
