#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-13 오후 3:57:50
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif


#if !ODIN_INSPECTOR
public struct ValueDropdownItem<T>
{
    public string Text;
    public T Value;

    public ValueDropdownItem(string strText, T pValue)
    {
        this.Text = strText; this.Value = pValue;
    }
}

public class ValueDropdownList<T> : List<ValueDropdownItem<T>>
{
}
#endif

static public class ValueDropdownListHelper
{
    static public ValueDropdownList<T> Create_Enum_ValueDropdownList<T>()
    {
        ValueDropdownList<T> pListReturn = new ValueDropdownList<T>();
        System.Type pEnumType = typeof(T);

        if (pEnumType.IsEnum)
        {
            string[] arrEnums = pEnumType.GetEnumNames();
            for (int i = 0; i < arrEnums.Length; i++)
            {
                string strCurrentEnumName = arrEnums[i];
                pListReturn.Add(strCurrentEnumName, (T)System.Enum.Parse(typeof(T), strCurrentEnumName));
            }
        }

        return pListReturn;
    }

    static public void Add<T>(this ValueDropdownList<T> list, string strName, T pItem)
    {
        list.Add(new ValueDropdownItem<T>(strName, pItem));
    }

    static public string[] GetNameList<T>(this ValueDropdownList<T> list)
    {
        string[] arrName = new string[list.Count];
        for (int i = 0; i < list.Count; i++)
            arrName[i] = list[i].Text;

        return arrName;
    }

    static public int Calculate_SelectIndex<T>(this ValueDropdownList<T> list, T pItem)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (Object.ReferenceEquals(list[i].Value, null))
                continue;

            if (list[i].Value.Equals(pItem))
                return i;
        }

        return 0;
    }

    static public T GetValue_ByText<T>(this ValueDropdownList<T> list, string pItem)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Text.Equals(pItem))
                return list[i].Value;
        }

        return default(T);
    }
}
