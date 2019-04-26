#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-08 오후 8:27:45
 *	개요 : https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class SCManagerScreenShot
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public static Texture2D GetScreenShot(Camera pCamera)
    {
        return GetScreenShot(pCamera.pixelWidth, pCamera.pixelHeight, pCamera);
    }

    public static Texture2D GetScreenShot(int iWidth, int iHeight, Camera pCamera)
    {
        RenderTexture pRenderTexture = new RenderTexture(iWidth, iHeight, 24);
        pCamera.targetTexture = pRenderTexture;
        Texture2D pTextureScreenShot = new Texture2D(iWidth, iHeight, TextureFormat.RGB24, false);
        pCamera.Render();

        RenderTexture.active = pRenderTexture;
        pTextureScreenShot.ReadPixels(new Rect(0, 0, iWidth, iHeight), 0, 0);
        pTextureScreenShot.Apply();

        pCamera.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        GameObject.Destroy(pRenderTexture);

        //byte[] arrByte = pTextureScreenShot.EncodeToPNG();
        //System.IO.File.WriteAllBytes(ScreenShotName(iWidth, iHeight), arrByte);

        return pTextureScreenShot;
    }

    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}