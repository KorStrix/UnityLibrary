using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/* ============================================ 
   Editor      : Strix                               
   Date        : 2017-05-30 오전 1:32:45
   Description : 
   Edit Log    : http://ordinarybk.tistory.com/entry/Unity3D-%EC%B5%9C%EC%A0%81%ED%99%94%EB%A5%BC-%EC%9C%84%ED%95%9C-CombineMesh
   ============================================ */

public class CMeshCombiner : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public Vector3 vecOffsetCombineModel;
    public bool _bShadowOff = true;
    public bool _bIsStatic = false;
    public bool _bDestroyChildMesh;

    /* protected - Field declaration         */

    /* private - Field declaration           */

    private GameObject _pObjectMakeInstance;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    public void EventStartMerge()
    {
        MeshRenderer pMeshRenderer = _pObjectMakeInstance.GetComponent<MeshRenderer>();
        MeshFilter pMeshFilter = _pObjectMakeInstance.GetComponent<MeshFilter>();
        pMeshFilter.mesh.Clear();

        MeshFilter[] arrMeshFilter = GetComponentsInChildren<MeshFilter>(true);
        pMeshRenderer.material = arrMeshFilter[1].GetComponent<MeshRenderer>().sharedMaterial;

        CombineInstance[] combine = new CombineInstance[arrMeshFilter.Length - 1];

        int i = 0;
        int ci = 0;

        
        while (i < arrMeshFilter.Length)
        {
            MeshFilter pMeshFilterCurrent = arrMeshFilter[i];
            if (pMeshFilter != pMeshFilterCurrent)
            {
                combine[ci].mesh = pMeshFilterCurrent.sharedMesh;
                Matrix4x4 tmp = Matrix4x4.identity;

                Transform pTransMesh = pMeshFilterCurrent.transform;
                tmp.SetTRS(pTransMesh.position, pTransMesh.rotation, pTransMesh.lossyScale);
                combine[ci].transform = tmp;

                if (_bDestroyChildMesh)
                    Destroy(arrMeshFilter[i].gameObject);
                else
                    pMeshFilterCurrent.gameObject.SetActive(false);

                ++ci;
            }
            i++;
        }

        pMeshFilter.mesh.CombineMeshes(combine);
        _pObjectMakeInstance.SetActive(true);
        //pObject.GetComponent<MeshCollider>().sharedMesh = pMeshFilter.sharedMesh;
        //pObject.GetComponent<MeshCollider>().sharedMesh.Optimize();
    }

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _pObjectMakeInstance = new GameObject("MeshCombine");
        _pObjectMakeInstance.AddComponent<MeshFilter>();
        MeshRenderer pRenderer = _pObjectMakeInstance.AddComponent<MeshRenderer>();

        _pObjectMakeInstance.transform.SetParent(transform);
        _pObjectMakeInstance.transform.localPosition = vecOffsetCombineModel;

        EventStartMerge();

        if (_bShadowOff)
        {
            pRenderer.receiveShadows = false;
            pRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        gameObject.isStatic = _bIsStatic;
    }

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    /* private - Other[Find, Calculate] Function 
       찾기, 계산 등의 비교적 단순 로직         */

}
