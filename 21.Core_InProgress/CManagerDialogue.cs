#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-09 오후 5:36:53
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CManagerDialogue : CSingletonNotMonoBase<CManagerDialogue>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public class SDataTable_Dialogue : IDictionaryItem<string>, System.IComparable<SDataTable_Dialogue>
    {
        public string str대사이벤트;

        public int i대사순서;

        public string str배우;
        public string str대사;
        public string str표정;

        public string str이벤트함수이름;

        public string IDictionaryItem_GetKey()
        {
            return str대사이벤트;
        }

        public int CompareTo(SDataTable_Dialogue other)
        {
            return i대사순서.CompareTo(other.i대사순서);
        }
    }

    /* public - Field declaration            */


    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}