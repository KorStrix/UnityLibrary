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

[CreateAssetMenu(menuName = "StrixSO/" + nameof(HierachyIconList))]
public class HierachyIconList : CScriptableObject
{
    [System.Serializable]
    public class IconMappingDataBase
    {
#if ODIN_INSPECTOR
        [PreviewField(Alignment = ObjectFieldAlignment.Center)]
#endif
        [DisplayName("대상 텍스쳐")]
        [SerializeField]
        private Texture2D pTexture = null;
        [DisplayName("아이콘 순서 높을수록 왼쪽")]
        public int iOrder;
        [DisplayName("활성화 유무")]
        public bool bEnable = true;

        public Texture2D GetTexture()
        {
            if (bEnable)
                return pTexture;
            else
                return null;
        }
    }


    [System.Serializable]
    public class IconMappingData_Type : IconMappingDataBase, IDictionaryItem<string>
    {
#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetTypeName))]
#endif
        [DisplayName("대상 타입 이름")]
        public string strTypeName;

        public string IDictionaryItem_GetKey()
        {
            return strTypeName;
        }

        public IEnumerable<string> GetTypeName()
        {
            System.Type pComponentType = typeof(Component);

            return pComponentType.Assembly.GetTypes().Union(GetType().Assembly.GetTypes())
                    .Where(x => pComponentType.IsAssignableFrom(x))
                    .Select(x => x.GetFriendlyName());
        }
    }

    [System.Serializable]
    public class IconMappingData_Tag : IconMappingDataBase, IDictionaryItem<string>
    {
#if ODIN_INSPECTOR
        [ValueDropdown(nameof(GetTagName))]
#endif
        [DisplayName("대상 태그 이름")]
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

#if ODIN_INSPECTOR
    [LabelText("타입 별 아이콘 목록")]
#else
    [Header("타입 별 아이콘 목록")]
#endif
    public List<IconMappingData_Type> p_listIconMappingData_Type = new List<IconMappingData_Type>();

#if ODIN_INSPECTOR
    [LabelText("태그 별 아이콘 목록")]
#else
    [Header("태그 별 아이콘 목록")]
#endif
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
