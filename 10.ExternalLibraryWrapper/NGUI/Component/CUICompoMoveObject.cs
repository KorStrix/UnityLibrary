using UnityEngine;
using System.Collections;
#if NGUI
[RequireComponent(typeof(BoxCollider))]
public class CUICompoMoveObject : CNGUIPanelBase
{
    [SerializeField]
    private Vector2 vecPosLimitMin = Vector2.zero;
    [SerializeField]
    private Vector2 vecPosLimitMax = Vector2.zero;

    //private Camera _pCamUI;
    // Vector2 vecPosOld;
    //Vector2 oldTouchVector;
    //float oldTouchDistance;
    //private bool _bIsDrag = false;

    // ========================== [ Division ] ========================== //

    void Reset()
    {
        BoxCollider pBoxCollider = GetComponent<BoxCollider>();
        if (pBoxCollider == null)
            NGUITools.AddWidgetCollider(gameObject);
    }
	
    protected override void OnUpdate(float fTimeScale_Individual)
    {
        base.OnUpdate(fTimeScale_Individual);

        //int iTouchCount = UICamera.CountInputSources();
        //if (_bIsDrag == false && iTouchCount == 0)
        //    vecPosOld = Vector2.zero;
        //else if (iTouchCount == 1)
        //{
        //    if (vecPosOld == Vector2.zero)
        //        vecPosOld = UICamera.lastEventPosition;
        //    else
        //    {
        //        Vector2 vecPosNew = UICamera.lastEventPosition;
        //        Vector2 vecConvert = vecPosNew - vecPosOld;
        //        _pTransformCached.localPosition = (Vector2)_pTransformCached.localPosition + vecConvert;
        //        vecPosOld = vecPosNew;
        //    }
        //}

        Vector2 vecPos = (Vector2)_pTransformCached.localPosition;
        if (vecPos.x < vecPosLimitMin.x) vecPos.x = vecPosLimitMin.x;
        if (vecPos.x > vecPosLimitMax.x) vecPos.x = vecPosLimitMax.x;
        if (vecPos.y < vecPosLimitMin.y) vecPos.y = vecPosLimitMin.y;
        if (vecPos.y > vecPosLimitMax.y) vecPos.y = vecPosLimitMax.y;
        _pTransformCached.localPosition = vecPos;
    }
}
#endif