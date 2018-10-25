#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-27 오전 11:48:24
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if VRStandardAssets
using VRStandardAssets.Utils;

public class CVRController : CObjectBase
{
    public float _fRotSpeed;
    public LineRenderer _pLineRenderer;
    public float _fLineLength = 10f;

    private VRInput _pVRInput;                                          // Reference to the VRInput to detect button presses.

    public Ray GetCurrentRay()
    {
        return new Ray(transform.position, transform.forward);

    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        _pVRInput = CManagerVR.instance.p_pVRInput;
        _pVRInput.OnDown += HandleDown;
    }

    protected override void OnDisableObject()
    {
        base.OnDisableObject();

        _pVRInput.OnDown -= HandleDown;
    }

    public override void OnUpdate(ref bool bCheckUpdateCount)
    {
        base.OnUpdate(ref bCheckUpdateCount);
        bCheckUpdateCount = true;

        Quaternion rotController = OVRInput.GetLocalControllerRotation(OVRInput.GetActiveController());
        transform.rotation = Quaternion.Slerp(transform.rotation, rotController, _fRotSpeed * Time.deltaTime);

        if (_pLineRenderer != null)
            ProcDrawLineRenderer();
    }

    private void HandleDown()
    {
    }

    private void ProcDrawLineRenderer()
    {
        _pLineRenderer.SetPosition(0, transform.position);
        _pLineRenderer.SetPosition(1, transform.position + transform.forward * _fLineLength);
    }
}
#endif