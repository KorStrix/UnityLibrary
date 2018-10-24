#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-10 오전 10:34:37
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public class CCompoGizmoDrawer : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EGizmoShape
    {
        Sphere,
        WireSphere,
        Cube,
        WireCube
    }

    /* public - Field declaration            */

    [Rename_Inspector("기즈모 모양")]
    public EGizmoShape p_eGizmoShape = EGizmoShape.WireSphere;
    [Rename_Inspector("기즈모 포지션 오프셋")]
    public Vector3 p_vecPositionOffset;
    [Rename_Inspector("기즈모 색상")]
    public Color p_pGizmoColor = Color.red;
    [Rename_Inspector("기즈모 사이즈")]
    public float p_fGizmoSize = 1f;

    [Header("기즈모 텍스트 옵션")]
    [Rename_Inspector("기즈모에 출력할 텍스트")]
    public string p_strGizmoName;
    [Rename_Inspector("기즈모 텍스트 포지션 오프셋")]
    public Vector3 p_vecPositionOffset_Name;
    [Rename_Inspector("기즈모 텍스트 색상")]
    public Color p_pGizmoTextColor = Color.green;
    [Rename_Inspector("기즈모 텍스트 사이즈")]
    public int p_iFontSize = 10;
    [Rename_Inspector("기즈모 텍스트를 게임 오브젝트 이름으로 출력")]
    public bool p_bPrintObjectName = true;

    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = p_pGizmoColor;
        switch (p_eGizmoShape)
        {
            case EGizmoShape.Sphere:
                Gizmos.DrawSphere(transform.position, p_fGizmoSize);
                break;
            case EGizmoShape.WireSphere:
                Gizmos.DrawWireSphere(transform.position, p_fGizmoSize);
                break;
            case EGizmoShape.Cube:
                Gizmos.DrawCube(transform.position, Vector3.one * p_fGizmoSize);
                break;
            case EGizmoShape.WireCube:
                Gizmos.DrawWireCube(transform.position, Vector3.one * p_fGizmoSize);
                break;
        }

        GUIStyle pStyle = new GUIStyle();
        pStyle.normal.textColor = p_pGizmoTextColor;
        pStyle.fontSize = p_iFontSize;

        if(p_bPrintObjectName)
            UnityEditor.Handles.Label(transform.position + p_vecPositionOffset_Name, name, pStyle);
        else
            UnityEditor.Handles.Label(transform.position + p_vecPositionOffset_Name, p_strGizmoName, pStyle);
    }
#endif

    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test