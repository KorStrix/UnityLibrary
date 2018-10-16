#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-05-19 오후 1:58:28
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct CRaycastHitWrapper
{
    public RaycastHit p_pHitOrigin { get; private set; }
    public RaycastHit2D p_pHitOrigin2D { get; private set; }

    public bool p_bIs3D { get; private set; }
    public bool p_bIs2D { get; private set; }

    // 래핑
    public Vector3 normal { get; private set; }
    public Vector3 point { get; private set; }

    public Transform transform { get; private set; }
    public float distance { get; private set; }
    public Collider collider3D { get; private set; }
    public Collider2D collider2D { get; private set; }

    public bool p_bCollider_IsTrigger
    {
        get
        {
            if (p_bIs3D)
                return p_pHitOrigin.collider.isTrigger;
            else if (p_bIs2D)
                return p_pHitOrigin2D.collider.isTrigger;
            else
                return false;
        }
    }

    public CRaycastHitWrapper(RaycastHit pHitOrigin)
    {
        p_pHitOrigin = pHitOrigin;
        p_bIs3D = pHitOrigin.transform != null;
        collider3D = pHitOrigin.collider;

        p_pHitOrigin2D = default(RaycastHit2D);
        p_bIs2D = false;
        collider2D = null;

        point = pHitOrigin.point;
        normal = pHitOrigin.normal;
        transform = pHitOrigin.transform;
        distance = pHitOrigin.distance;
    }

    public CRaycastHitWrapper(RaycastHit2D pHitOrigin)
    {
        p_pHitOrigin = default(RaycastHit);
        p_bIs3D = false;
        collider3D = null;

        p_pHitOrigin2D = pHitOrigin;
        p_bIs2D = pHitOrigin.Equals(default(RaycastHit2D)) == false;
        collider2D = pHitOrigin.collider;

        point = pHitOrigin.point;
        normal = pHitOrigin.normal;
        transform = pHitOrigin.transform;
        distance = pHitOrigin.distance;
    }

    public static CRaycastHitWrapper Raycast2D(Vector2 vecOrigin, Vector2 vecDirection, float fDistance, int iLayerMask)
    {
        return new CRaycastHitWrapper(Physics2D.Raycast(vecOrigin, vecDirection, fDistance, iLayerMask));
    }

    public static CRaycastHitWrapper Raycast3D(Vector2 vecOrigin, Vector2 vecDirection, float fDistance, int iLayerMask)
    {
        RaycastHit pHit3D;
        Physics.Raycast(new Ray(vecOrigin, vecDirection), out pHit3D, fDistance, iLayerMask);

        return new CRaycastHitWrapper(pHit3D);
    }


    public static implicit operator CRaycastHitWrapper(RaycastHit2D hit)
    {
        return new CRaycastHitWrapper(hit);
    }

    public static implicit operator CRaycastHitWrapper(RaycastHit hit)
    {
        return new CRaycastHitWrapper(hit);
    }

    public static implicit operator RaycastHit(CRaycastHitWrapper hit)
    {
        return hit.p_pHitOrigin;
    }


    public static implicit operator bool(CRaycastHitWrapper hit)
    {
        return hit.p_bIs3D || hit.p_bIs2D;
    }
}

