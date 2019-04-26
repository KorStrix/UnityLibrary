#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-09 오후 8:22:56
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SCGameConfig
{
    [System.Serializable]
    public class GameConfigData : IDictionaryItem<string>
    {
        public enum EValueType
        {
            NOTHING,
            INT,
            STRING,
            FLOAT,
        }

        public string 타입;
        public string 이름;
        public string 값;

        public int p_iValue { get; private set; }
        public float p_fValue { get; private set; }
        public string p_strValue { get; private set; }

        public string IDictionaryItem_GetKey()
        {
            return 이름;
        }

        public void DoInit()
        {
            EValueType eValueType = 타입.ConvertEnum<EValueType>(true);

            switch (eValueType)
            {
                case EValueType.INT: p_iValue = int.Parse(값); break;
                case EValueType.FLOAT: p_fValue = float.Parse(값); break;
                case EValueType.STRING: p_strValue = 값; break;

                case EValueType.NOTHING: break;

                default:
                    Debug.LogError(이름 + " Error");
                    break;
            }
        }
    }

    static Dictionary<string, GameConfigData> _mapGameConfigData = new Dictionary<string, GameConfigData>();

    static public void DoInit(string strDBArray)
    {
        GameConfigData[] arrConfigData = JsonUtilityExtension.DoReadJsonArray<GameConfigData>(strDBArray);
        _mapGameConfigData.DoClear_And_AddItem(arrConfigData);

        for (int i = 0; i < arrConfigData.Length; i++)
            arrConfigData[i].DoInit();
    }

    static public int GetData_Int(string strKey)
    {
        if (strKey != null && _mapGameConfigData.ContainsKey(strKey))
            return _mapGameConfigData[strKey].p_iValue;
        else
            return 0;
    }

    static public float GetData_Float(string strKey)
    {
        if (strKey != null && _mapGameConfigData.ContainsKey(strKey))
            return _mapGameConfigData[strKey].p_fValue;
        else
            return 0;
    }

    static public string GetData_String(string strKey)
    {
        if (strKey != null && _mapGameConfigData.ContainsKey(strKey))
            return _mapGameConfigData[strKey].p_strValue;
        else
            return "";
    }
}
