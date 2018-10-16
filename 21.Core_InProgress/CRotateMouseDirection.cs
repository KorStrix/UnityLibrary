#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-04-23 오후 3:24:11
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CRotateMouseDirection : CObjectBase
{
    public float fSpeed = 0.2f;

    void Update()
    {
        Vector3 vecMousePositionOrigin = Input.mousePosition;
        Vector3 vecMousePosition = vecMousePositionOrigin;
        vecMousePosition.x = (Screen.height / 2) - vecMousePositionOrigin.y;
        vecMousePosition.y = -(Screen.width / 2) + vecMousePositionOrigin.x;

        transform.Rotate(vecMousePosition * Time.deltaTime * fSpeed, Space.Self);
    }
}