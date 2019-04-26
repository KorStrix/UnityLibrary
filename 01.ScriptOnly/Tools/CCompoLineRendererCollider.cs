#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-11-16 오후 3:19:25
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

/// <summary>
/// 참고한 코드
/// http://www.theappguruz.com/blog/add-collider-to-line-renderer-unity
/// </summary>
public class CCompoLineRendererCollider : MonoBehaviour
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EPhysicsState
    {
        Enter,
        Stay,
        Exit,
    }

    public CObserverSubject<Collider2D, EPhysicsState> p_Event_OnTrigger2D { get; private set; } = new CObserverSubject<Collider2D, EPhysicsState>();
    public CObserverSubject<Collision2D, EPhysicsState> p_Event_OnCollision2D { get; private set; } = new CObserverSubject<Collision2D, EPhysicsState>();

    /* public - Field declaration            */

    public LineRenderer pLineRenderer;

    /* protected & private - Field declaration         */

    BoxCollider _pBoxCollider;
    BoxCollider2D _pBoxCollider_2D;

    Vector3 vecSize;
    Vector3 vecMidPoint;
    float fAngle;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static protected Vector3 DevideVector3(Vector3 vecX, Vector3 vecY)
    {
        Vector3 vecNewVector = vecX;
        vecNewVector.x /= vecY.x;
        vecNewVector.y /= vecY.y;
        vecNewVector.z /= vecY.z;

        return vecNewVector;
    }


    // ========================================================================== //

    /* protected - Override & Unity API         */

    private void Awake()
    {
        _pBoxCollider = GetComponent<BoxCollider>();
        _pBoxCollider_2D = GetComponent<BoxCollider2D>();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
    }

    private void Update()
    {
        CalculateColliderShape();
        UpdateColliderShape();
    }

    private void OnTriggerEnter2D(Collider2D collision) { p_Event_OnTrigger2D.DoNotify(collision, EPhysicsState.Enter); }
    private void OnTriggerStay2D(Collider2D collision) { p_Event_OnTrigger2D.DoNotify(collision, EPhysicsState.Stay); }
    private void OnTriggerExit2D(Collider2D collision) { p_Event_OnTrigger2D.DoNotify(collision, EPhysicsState.Exit); }

    private void OnCollisionEnter2D(Collision2D collision) { p_Event_OnCollision2D.DoNotify(collision, EPhysicsState.Enter); }
    private void OnCollisionStay2D(Collision2D collision) { p_Event_OnCollision2D.DoNotify(collision, EPhysicsState.Stay); }
    private void OnCollisionExit2D(Collision2D collision) { p_Event_OnCollision2D.DoNotify(collision, EPhysicsState.Exit); }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private void CalculateColliderShape()
    {
        Vector3 vecLossyScale = transform.lossyScale;
        Vector3 vecPosStart = pLineRenderer.GetPosition(0);
        Vector3 vecPosEnd = pLineRenderer.GetPosition(pLineRenderer.positionCount - 1);

        Vector3 vecPosStart_Unscaled = DevideVector3(vecPosStart, vecLossyScale);
        Vector3 vecPosEnd_Unscaled = DevideVector3(vecPosEnd, vecLossyScale);

        float fMagnitude = vecLossyScale.magnitude;
        float fLineLength = Vector3.Distance(vecPosStart_Unscaled, vecPosEnd_Unscaled); // length of line
        float fLineWidth = pLineRenderer.startWidth / fMagnitude;

        vecSize = new Vector3(fLineLength, fLineWidth, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement

        Vector3 vecMiddlePoint = (vecPosStart + vecPosEnd) / 2;
        vecMidPoint = vecMiddlePoint; // setting position of collider object

        // Following lines calculate the angle between startPos and endPos
        //float angle = (Mathf.Abs(vecPosStart.y - vecPosEnd.y) / Mathf.Abs(vecPosStart.x - vecPosEnd.x));
        //if ((vecPosStart.y < vecPosEnd.y && vecPosStart.x > vecPosEnd.x) || (vecPosEnd.y < vecPosStart.y && vecPosEnd.x > vecPosStart.x))
        //{
        //    angle *= -1;
        //}
        //fAngle = Mathf.Rad2Deg * Mathf.Atan(angle);
    }

    private void UpdateColliderShape()
    {
        if(_pBoxCollider)
        {
            _pBoxCollider.size = vecSize;
        }
        else if(_pBoxCollider_2D)
        {
            _pBoxCollider_2D.size = vecSize;
        }

        transform.position = vecMidPoint;
    }

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test