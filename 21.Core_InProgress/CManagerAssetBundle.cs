#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-31 오후 10:01:22
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class CManagerAssetBundle : CSingletonMonoBase<CManagerAssetBundle>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public UnityEngine.UI.Image _pImage;

    /* protected & private - Field declaration         */

    AssetBundle _pAssetBundle;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoLoadBundle_StreamingAsset(string strBundleName)
    {
        if (_pAssetBundle == null)
            StartCoroutine(LoadAssetBundle(GetAssetBundlePath(strBundleName)));
    }

    IEnumerator LoadAssetBundle(string strBundleURL)
    {
        WWW www = new WWW(strBundleURL);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            _pAssetBundle = www.assetBundle;
            StartCoroutine(ShowImage());
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    IEnumerator ShowImage()
    {
        AssetBundleRequest pAssetBundleTexture = _pAssetBundle.LoadAllAssetsAsync<Sprite>();
        yield return pAssetBundleTexture;

        _pImage.sprite = pAssetBundleTexture.asset as Sprite;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        DoLoadBundle_StreamingAsset("cardbase");
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    StringBuilder _pStrBuilder = new StringBuilder();
    string GetAssetBundlePath(string strAssetBundleName)
    {
        _pStrBuilder.Append(Application.streamingAssetsPath);
        _pStrBuilder.Append("/AssetBundle");
#if UNITY_EDITOR
        if (Application.isEditor)
        {
            DirectoryInfo pDirectoryInfo = new DirectoryInfo(_pStrBuilder.ToString());
            if (pDirectoryInfo.Exists == false)
            {
                Debug.Log("pDirectoryInfo.Exists == false // Create : " + _pStrBuilder.ToString());
                pDirectoryInfo.Create();
            }
        }
#endif

        _pStrBuilder.Append("/" + strAssetBundleName);
        return _pStrBuilder.ToString();
    }

    #endregion Private

    // ========================================================================== //

    #region Test

    #endregion Test
}
