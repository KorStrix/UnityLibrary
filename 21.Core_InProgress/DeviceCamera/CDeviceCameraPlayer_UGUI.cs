#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-11 오전 10:20:30
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class CDeviceCameraPlayer_UGUI : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public CManagerDeviceCamera _pCameraAccesser;

    /* protected & private - Field declaration         */

    RawImage _pImage;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pImage = GetComponent<RawImage>();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (_pImage.texture != null || _pCameraAccesser == null) return;

        _pImage.texture = _pCameraAccesser.p_pWebCamTexture;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
