#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-11 오전 9:35:07
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class CStreammingAssetGetter 
{
    public delegate void OnGetStreamingResource(string strPath_With_Extension, WWW pWWW, bool bResult, object pParam);

    Dictionary<string, WWW> _mapResourceCashing = new Dictionary<string, WWW>();
    StringBuilder _pStrBuilder = new StringBuilder();

    MonoBehaviour _pCoroutineExcuter;

    // ========================== [ Division ] ========================== //

    public CStreammingAssetGetter(MonoBehaviour pCoroutineExcuter)
    {
        _pCoroutineExcuter = pCoroutineExcuter;
    }


    public void GetResource(string strResourceName_With_Extension, OnGetStreamingResource OnGetResource, bool bIsCashing)
    {
        if (bIsCashing)
        {
            WWW pFindResource;
            if (_mapResourceCashing.TryGetValue(strResourceName_With_Extension, out pFindResource))
            {
                OnGetResource(strResourceName_With_Extension, pFindResource, true, null);
                return;
            }
        }

        _pCoroutineExcuter.StartCoroutine(CoGetStreammingAsset(strResourceName_With_Extension, OnGetResource, bIsCashing, null));
    }

    public void GetResource(string strResourceName_With_Extension, OnGetStreamingResource OnGetResource, bool bIsCashing, object pParam)
    {
        if (bIsCashing)
        {
            WWW pFindResource;
            if (_mapResourceCashing.TryGetValue(strResourceName_With_Extension, out pFindResource))
            {
                OnGetResource(strResourceName_With_Extension, pFindResource, true, pParam);
                return;
            }
        }

        _pCoroutineExcuter.StartCoroutine(CoGetStreammingAsset(strResourceName_With_Extension, OnGetResource, bIsCashing, pParam));
    }


    private IEnumerator CoGetStreammingAsset(string strResourceName_With_Extension, OnGetStreamingResource OnGetResource, bool bIsCashing, object pParam)
    {
        _pStrBuilder.Length = 0;
        _pStrBuilder.Append(Application.streamingAssetsPath).Append("/").Append(strResourceName_With_Extension);

        WWW www = new WWW(_pStrBuilder.ToString());
        yield return www;

        bool bResult = (www.error != null && www.error.Contains("Error")) == false;
        OnGetResource(strResourceName_With_Extension, www, bResult, pParam);
        if (bIsCashing)
            _mapResourceCashing[strResourceName_With_Extension] = www;
    }
}
