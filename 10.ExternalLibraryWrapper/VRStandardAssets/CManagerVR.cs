#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-23 오후 3:51:09
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if VRStandardAssets
using VRStandardAssets.Utils;

[RequireComponent(typeof(VRDeviceManager))]
public class CManagerVR : CSingletonDynamicMonoBase<CManagerVR>
{
    [SerializeField]
    private VRInput _pVRInput;  public VRInput p_pVRInput {  get { return _pVRInput; } }

    protected override void OnAwake()
    {
        base.OnAwake();

        if (_pVRInput == null)
            _pVRInput = FindObjectOfType<VRInput>();
    }
}
#endif
