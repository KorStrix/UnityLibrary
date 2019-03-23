#region Header
/* ============================================ 
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item_Test : CObjectBase, IMagneticListener
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    #region Field
    /* public - Field declaration            */

    /* protected - Field declaration         */

    /* private - Field declaration           */


    #endregion Field

    #region Public
    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    #endregion Public

    // ========================================================================== //

    #region Protected
    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    public void IMagneticListener_OnStartMagnetic(ref bool bIsStartMagnetic)
    {
        Debug.Log(name + nameof(IMagneticListener_OnStartMagnetic), this);
    }

    public void IMagneticListener_OnArriveMagnetic()
    {
        Debug.Log(name + nameof(IMagneticListener_OnArriveMagnetic), this);

        gameObject.SetActive(false);
    }

    public void IMagneticListener_OnExitMagnetic()
    {
        Debug.Log(name + nameof(IMagneticListener_OnExitMagnetic), this);
    }

    /* protected - Override & Unity API         */

    #endregion Protected

    // ========================================================================== //

    #region Private
    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    #endregion Private
}
