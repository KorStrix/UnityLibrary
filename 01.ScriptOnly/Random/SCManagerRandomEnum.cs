#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SCManagerRandomEnum<Enum_Group, Enum_Item>
    where Enum_Group : System.IComparable, System.IConvertible
    where Enum_Item : System.IComparable, System.IConvertible
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    static private SCManagerRandomEnum<Enum_Group, Enum_Item> _instance;

    static public SCManagerRandomEnum<Enum_Group, Enum_Item> instance
    {
        get
        {
            if (_instance == null)
                _instance = new SCManagerRandomEnum<Enum_Group, Enum_Item>();

            return _instance;
        }
    }

    static public CDictionary_ForEnumKey<Enum_Group, List<Enum_Item>> _mapTable = new CDictionary_ForEnumKey<Enum_Group, List<Enum_Item>>();

    /* protected - Field declaration         */

    /* private - Field declaration           */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public void DoAddGroup(Enum_Group eGroup, Enum_Item eItem)
    {
        if (_mapTable.ContainsKey(eGroup) == false)
            _mapTable.Add(eGroup, new List<Enum_Item>());

        _mapTable[eGroup].Add(eItem);
    }

    static public Enum_Item GetRandomEnum(Enum_Group eGroup)
    {
        if (_mapTable.ContainsKey_PrintOnError(eGroup.GetHashCode()) == false)
            return default(Enum_Item);

        List<Enum_Item> listItem = _mapTable[eGroup];
        if (listItem.Count == 0)
            return default(Enum_Item);

        int iRandomIndex = Random.Range(0, listItem.Count);
        return listItem[iRandomIndex];
    }
}
