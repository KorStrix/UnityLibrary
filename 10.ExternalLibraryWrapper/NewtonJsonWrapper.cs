#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-12 오후 8:22:39
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if Newtonsoft
using Newtonsoft.Json;
#endif
/// <summary>
/// 
/// </summary>
public static class NewtonJsonWrapper
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public string ObjectToJson(object obj)
    {
        return "";
#if Newtonsoft
        return JsonConvert.SerializeObject(obj);
#endif
    }

    static public T JsonToOject<T>(string jsonData)
    {
#if Newtonsoft
        return JsonConvert.DeserializeObject<T>(jsonData);
#endif

        return default(T);
    }

    static public Dictionary<string, T> JsonToDictionary<T>(string strJson)
    {
#if Newtonsoft
        return JsonConvert.DeserializeObject<Dictionary<string, T>>(strJson);
#endif

        return null;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

#region Private

#endregion Private
}