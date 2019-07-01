#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-24 오후 1:27:29
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ColorAttribute_Example : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public struct STest
    {
        public string strTest;
    }

    /* public - Field declaration            */

    [Color(1f, 0f, 0f)]
    public int Red;

    [Color(0f, 1f, 0f)]
    public List<int> Green;

    /* protected & private - Field declaration         */

    [Color(0f, 0f, 1f)]
    [SerializeField]
    private int Blue;

    [Color(0f, 0f, 0f, 0.8f)]
    [SerializeField]
    private STest Transparency;


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