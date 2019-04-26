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

using Sirenix.OdinInspector;
using System;
using Sirenix.OdinInspector.Editor;
using System.Reflection;

[CreateAssetMenu(menuName = "StrixSO/" + nameof(HierachyIIconList))]
public class HierachyIIconList : CScriptableObject
{
    [System.Serializable]
    public class IconMappingDataBase
    {
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
        [LabelText("대상 텍스쳐")]
        public Texture2D pTexture;
        [LabelText("아이콘 순서 높을수록 왼쪽")]
        public int iOrder;
    }


    [System.Serializable]
    public class IconMappingData_Type : IconMappingDataBase, IDictionaryItem<string>
    {
        [ShowInInspector]
        [LabelText("대상 타입 이름")]
        [ValueDropdown(nameof(GetTypeName))]
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
        [LabelText("대상 태그 이름")]
        [ValueDropdown(nameof(GetTagName))]
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

    [Button]
    public void UpdateIcon()
    {
        HierachyIconConfig.instance.UpdateIcon();
    }

}