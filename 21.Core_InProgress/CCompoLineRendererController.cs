#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-08-13 오후 6:49:16
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

public class CCompoLineRendererController : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public float maxLength = 32;
    public int segmentCount = 32;
    public float globalProgressSpeed = 1f;
    public AnimationCurve shaderProgressCurve;
    public AnimationCurve lineWidthCurve;
    public float moveHitToSource;
    public LayerMask p_pLayerMask_Hit;

    /* protected & private - Field declaration         */

    private LineRenderer lr;
    private Vector3[] resultVectors;
    private float dist;
    private float globalProgress;
    private Vector3 hitPosition;
    private Vector3 currentPosition;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoUpdate_LineRenderer()
    {
        globalProgress = 0f;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();


        globalProgress = 1f;
        lr = this.GetComponent<LineRenderer>();
        lr.positionCount = segmentCount;
        resultVectors = new Vector3[segmentCount + 1];
        for (int i = 0; i < segmentCount + 1; i++)
        {
            resultVectors[i] = transform.forward;
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        for (int i = segmentCount - 1; i > 0; i--)
        {
            resultVectors[i] = resultVectors[i - 1];
        }
        resultVectors[0] = transform.forward;
        resultVectors[segmentCount] = resultVectors[segmentCount - 1];
        float blockLength = maxLength / segmentCount;


        currentPosition = new Vector3(0, 0, 0);

        for (int i = 0; i < segmentCount; i++)
        {
            currentPosition = transform.position;
            for (int j = 0; j < i; j++)
            {
                currentPosition += resultVectors[j] * blockLength;
            }
            lr.SetPosition(i, currentPosition);
        }

        //Curvy End

        //Collision Start

        for (int i = 0; i < segmentCount; i++)
        {

            currentPosition = transform.position;
            for (int j = 0; j < i; j++)
            {
                currentPosition += resultVectors[j] * blockLength;
            }

            RaycastHit2D hit = Physics2D.Raycast(currentPosition, resultVectors[i], blockLength, p_pLayerMask_Hit.value);
            if (hit)
            {
                hitPosition = currentPosition + resultVectors[i] * hit.distance;
                hitPosition = Vector3.MoveTowards(hitPosition, transform.position, moveHitToSource);

                dist = Vector3.Distance(hitPosition, transform.position);

                break;
            }
        }

        //Collision End


        //Emit Particles on Collision Start

        //Emit Particles on Collision End

        GetComponent<Renderer>().material.SetFloat("_Distance", dist);
        GetComponent<Renderer>().material.SetVector("_Position", transform.position);

        if (globalProgress <= 1f)
        {
            globalProgress += Time.deltaTime * globalProgressSpeed;
        }

        float progress = shaderProgressCurve.Evaluate(globalProgress);
        GetComponent<Renderer>().material.SetFloat("_Progress", progress);

        float width = lineWidthCurve.Evaluate(globalProgress);
        lr.widthMultiplier = width;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test