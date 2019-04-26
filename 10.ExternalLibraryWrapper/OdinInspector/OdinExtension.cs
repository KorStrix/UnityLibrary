#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-12 오후 3:11:35
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

public interface IHasName
{
    string IHasName_GetName();
}

#if ODIN_INSPECTOR

#else

public class ValueDropdownItem<T>
{
    public T Value;
}

public class ValueDropdownList<T> : List<ValueDropdownItem<T>>
{
    public void Add(string strName, T pItem)
    {

    }
}

#endif

/// <summary>
/// 
/// </summary>
public static class OdinExtension
{
    static public ValueDropdownList<T> GetValueDropDownList_EnumSubString<T>()
    {
        ValueDropdownList<T> list = new ValueDropdownList<T>();

        string[] arrStateName = System.Enum.GetNames(typeof(T));
        for (int i = 0; i < arrStateName.Length; i++)
        {
            T eProjectileName = arrStateName[i].ConvertEnum<T>();
            System.Enum pEnum = eProjectileName as System.Enum;
            if(pEnum != null)
                list.Add(pEnum.ToStringSub(), eProjectileName);
        }

        return list;
    }

    static public ValueDropdownList<T> GetValueDropDownList_SubString<T>()
        where T : class
    {
        ValueDropdownList<T> list = new ValueDropdownList<T>();

        var pFilteredTypeList = GetTypeFilter(typeof(T));
        foreach(var pType in pFilteredTypeList)
        {
            T pCurrentT = System.Activator.CreateInstance(pType) as T;
            if(pCurrentT != null)
                list.Add(pCurrentT.ToStringSub(), pCurrentT);
        }

        return list;
    }

    static List<System.Type> list_For_GetValueDropDownList_HasName = new List<System.Type>();
    static public ValueDropdownList<T> GetValueDropDownList_HasName<T>()
    where T : class, IHasName
    {
        ValueDropdownList<T> list = new ValueDropdownList<T>();

        var pFilteredTypeList = GetTypeFilter(typeof(T));
        foreach (var pType in pFilteredTypeList)
        {
            T pCurrentT = System.Activator.CreateInstance(pType) as T;
            if (pCurrentT != null)
                list.Add(pCurrentT.IHasName_GetName(), pCurrentT);
        }

        return list;
    }

    static public IEnumerable<System.Type> GetTypeFilter(System.Type pType)
    {
        return pType.Assembly.GetTypes()
            .Where(x => !x.IsAbstract)
            .Where(x => pType.IsAssignableFrom(x));
    }
}