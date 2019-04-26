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
using static HierachyIIconList;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
[CreateAssetMenu(menuName ="StrixSO/" + nameof(HierachyIconConfig))]
public class HierachyIconConfig : CSingletonSOBase<HierachyIconConfig>
{
    public HierachyIIconList[] arrIconMapping;

    static Dictionary<string, KeyValuePair<Texture2D, int>> mapIconPer_Type = new Dictionary<string, KeyValuePair<Texture2D, int>>();
    static Dictionary<string, KeyValuePair<Texture2D, int>> mapIconPer_Tag = new Dictionary<string, KeyValuePair<Texture2D, int>>();

    static public bool GetTexture_Per_Type(System.Type pType, out Texture2D pTexture, out int iOrder)
    {
        KeyValuePair<Texture2D, int> sValue;
        bool bResult = mapIconPer_Type.TryGetValue(pType.GetFriendlyName(), out sValue);
        pTexture = sValue.Key;
        iOrder = sValue.Value;

        return bResult;
    }

    static public bool GetTexture_Per_Tag(string strTag, out Texture2D pTexture, out int iOrder)
    {
        KeyValuePair<Texture2D, int> sValue;
        bool bResult = mapIconPer_Tag.TryGetValue(strTag, out sValue);
        pTexture = sValue.Key;
        iOrder = sValue.Value;

        return bResult;
    }

    protected override void OnAwake(bool bAppIsPlaying)
    {
        base.OnAwake(bAppIsPlaying);

        UpdateIcon();
    }

#if ODIN_INSPECTOR
    [Button]
#endif
    public void UpdateIcon()
    {
        mapIconPer_Type.Clear();
        mapIconPer_Tag.Clear();

        for (int i = 0; i < arrIconMapping.Length; i++)
        {
            for (int j = 0; j < arrIconMapping[i].p_listIconMappingData_Type.Count; j++)
                mapIconPer_Type.Add(arrIconMapping[i].p_listIconMappingData_Type[j].strTypeName, new KeyValuePair<Texture2D, int>(arrIconMapping[i].p_listIconMappingData_Type[j].pTexture, arrIconMapping[i].p_listIconMappingData_Type[j].iOrder));

            for (int j = 0; j < arrIconMapping[i].p_listIconMappingData_Tag.Count; j++)
                mapIconPer_Tag.Add(arrIconMapping[i].p_listIconMappingData_Tag[j].strTag, new KeyValuePair<Texture2D, int>(arrIconMapping[i].p_listIconMappingData_Tag[j].pTexture, arrIconMapping[i].p_listIconMappingData_Tag[j].iOrder));
        }
    }
}