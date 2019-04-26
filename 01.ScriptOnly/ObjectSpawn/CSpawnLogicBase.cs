#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-31 오후 5:36:45
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static CSpawnPointBase;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
public class CSpawnLogicBase : IHasName
{
    virtual public int p_iExecuterOrder => 0;

    virtual public void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
    {
        listSpawnInfo.Add(new SpawnInfo(pTransform.position, pTransform.rotation));
    }

    virtual public void DoDrawGizmo(Transform pTransform) { }

    public string IHasName_GetName() { return this.ToStringSub(); }
}

namespace SpawnLogic
{
    [RegistSubString("멀티샷")]
    public class CSpawnLogic_MultipleShot : CSpawnLogicBase
    {
        [Rename_Inspector("총구 개수")]
        public int iMuzzleCount = 2;
        [Rename_Inspector("총구 각도")]
        public float fAngleMuzzle = 15f;
        [Rename_Inspector("총구 회전 축")]
        public Vector3 vecAxis = Vector3.up;

        public override void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
        {
            Vector3 vecPos = pTransform.position;

            // 좌측부터 기울어진 상태에서 시작
            int iStartGap = (iMuzzleCount / 2);
            float fAngle = fAngleMuzzle * -iStartGap;
            for (int i = 0; i < iMuzzleCount; i++)
            {
                Quaternion rotAngle = pTransform.rotation * Quaternion.AngleAxis(fAngle, vecAxis);
                listSpawnInfo.Add(new SpawnInfo(vecPos, rotAngle));
                fAngle += fAngleMuzzle;
            }
        }
    }

    [RegistSubString("원형샷")]
    public class CSpawnLogic_Circle : CSpawnLogicBase
    {
        [Rename_Inspector("총구 개수")]
        public int iMuzzleCount = 10;

        public override void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
        {
            Vector3 vecCurrentPos = pTransform.position;
            float fAngleGap = 360 / iMuzzleCount;
            float fAngle = 0;
            for (int i = 0; i < iMuzzleCount; i++)
            {
                listSpawnInfo.Add(new SpawnInfo(vecCurrentPos, Quaternion.Euler(new Vector3(0f, 0f, fAngle))));
                fAngle += fAngleGap;
            }
        }
    }

    [RegistSubString("큐브 내에 랜덤 스폰")]
    public class CSpawnModule_Random_InArea_Cube : CSpawnLogicBase
    {
        [Rename_Inspector("스폰 개수")]
        public int iSpawnCount = 5;

#if ODIN_INSPECTOR
        [HorizontalGroup("X")]
#endif
        [Rename_Inspector("위치 X 최소값")]
        public float _fRandomPosX_Min = -5f;
#if ODIN_INSPECTOR
        [HorizontalGroup("X")]
#endif
        [Rename_Inspector("위치 X 최대값")]
        public float _fRandomPosX_Max = 5f;

#if ODIN_INSPECTOR
        [HorizontalGroup("Y")]
#endif
        [Rename_Inspector("위치 Y 최소값")]
        public float _fRandomPosY_Min = -5f;
#if ODIN_INSPECTOR
        [HorizontalGroup("Y")]
#endif
        [Rename_Inspector("위치 Y 최대값")]
        public float _fRandomPosY_Max = 5f;

#if ODIN_INSPECTOR
        [HorizontalGroup("Z")]
#endif
        [Rename_Inspector("위치 Z 최소값")]
        public float _fRandomPosZ_Min = -5f;
#if ODIN_INSPECTOR
        [HorizontalGroup("Z")]
#endif
        [Rename_Inspector("위치 Z 최대값")]
        public float _fRandomPosZ_Max = 5f;


        public override void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
        {
            Vector3 vecCurrentPos = pTransform.position;
            for (int i = 0; i < iSpawnCount; i++)
            {
                float fPosX = Random.Range(_fRandomPosX_Min, _fRandomPosX_Max);
                float fPosY = Random.Range(_fRandomPosY_Min, _fRandomPosY_Max);
                float fPosZ = Random.Range(_fRandomPosZ_Min, _fRandomPosZ_Max);

                Vector3 vecRandomOffset = new Vector3(fPosX, fPosY, fPosZ);
                listSpawnInfo.Add(new SpawnInfo(vecCurrentPos + vecRandomOffset, Quaternion.identity));
            }
        }

#if UNITY_EDITOR
        public override void DoDrawGizmo(Transform pTransform)
        {
            base.DoDrawGizmo(pTransform);

            Gizmos.color = Color.green;

            Vector3 vecPos = pTransform.position;
            vecPos.x = vecPos.x - _fRandomPosX_Min + _fRandomPosX_Max;
            vecPos.y = vecPos.y - _fRandomPosY_Min + _fRandomPosY_Max;
            vecPos.z = vecPos.z - _fRandomPosZ_Min + _fRandomPosZ_Max;

            Vector3 vecSize = Vector3.zero;
            vecSize.x = _fRandomPosX_Max - _fRandomPosX_Min;
            vecSize.y = _fRandomPosY_Max - _fRandomPosY_Min;
            vecSize.z = _fRandomPosZ_Max - _fRandomPosZ_Min;

            Gizmos.DrawWireCube(vecPos, vecSize);
            UnityEditor.Handles.Label(vecPos, nameof(CSpawnModule_Random_InArea_Cube));
        }
#endif
    }
}

