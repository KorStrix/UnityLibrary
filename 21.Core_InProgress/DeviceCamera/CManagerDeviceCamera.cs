#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-11 오전 10:14:03
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CManagerDeviceCamera : CSingletonMonoBase<CManagerDeviceCamera>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    [DisplayName("시작시 자동으로 카메라 플레이")]
    public bool bAutoPlayCamera = false;

    public WebCamTexture p_pWebCamTexture { get; private set; }
    public bool p_bCameraAvailable { get; private set; }

    /* protected & private - Field declaration         */

    //AspectRatioFitter _pRatioFit;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoPlayCamera()
    {
        if (p_pWebCamTexture)
            p_pWebCamTexture.Play();

    }

    public void DoStopCamera()
    {
        if (p_pWebCamTexture)
            p_pWebCamTexture.Stop();
    }

public float GetCameraRatio()
    {
        if (p_pWebCamTexture)
            return (float)p_pWebCamTexture.width / p_pWebCamTexture.height;
        else
            return 0f;
    }

    public bool GetVideoVerticallyMirrored()
    {
        return p_pWebCamTexture.videoVerticallyMirrored;
    }

    public int GetVideoRotationAngle()
    {
        return p_pWebCamTexture.videoRotationAngle;
    }


    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        FindDeviceCamera();
        if (bAutoPlayCamera)
            DoPlayCamera();
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void FindDeviceCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length == 0)
        {
            Debug.Log("No Camera Detected");
            p_bCameraAvailable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if(devices[i].isFrontFacing == false)
                p_pWebCamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
        }

        if (p_pWebCamTexture == null)
        {
            for (int i = 0; i < devices.Length; i++)
                p_pWebCamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);


            Debug.Log("Unable to find back camera");
            return;
        }
    }

    #endregion Private
}
