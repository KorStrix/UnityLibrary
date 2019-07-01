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
using static CSpawnerBase;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

/// <summary>
/// 
/// </summary>
[RegistSubString("없음")]
[TypeInfoBox("스폰 시 오브젝트의 위치에 로직")]
public class CSpawnLogic
{
    virtual public void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
    {
        listSpawnInfo.Add(new SpawnInfo(pTransform.position, pTransform.rotation));
    }

    virtual public void DoDrawGizmo_OnSelected(Transform pTransform)
    {
        if (GetMuzzleCount() <= 0)
            return;

        Gizmos.color = Color.gray;
        GUIStyle pStyle = new GUIStyle();
        pStyle.normal.textColor = Color.gray;
        UnityEditor.Handles.Label(pTransform.position, pTransform.name + " " + this.ToStringSub(), pStyle);
    }

    virtual protected int GetMuzzleCount()
    {
        return 1;
    }
}

namespace SpawnLogic
{
    [RegistSubString("멀티샷")]
    [TypeInfoBox("스폰 시 오브젝트의 위치에 로직")]
    public class CSpawnLogic_MultipleShot : CSpawnLogic
    {
        const float const_fGizmoDistance = 10f;

        [DisplayName("총구 개수")]
        public int iMuzzleCount = 2;
        [DisplayName("총구 각도")]
        public float fAngleMuzzle = 15f;
        [DisplayName("총구 회전 축")]
        public Vector3 vecAxis = Vector3.up;
        [DisplayName("시작 위치")]
        public float fStartPos = 0f;

        public override void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
        {
            Vector3 vecPos = pTransform.position;
            Quaternion rot = pTransform.rotation;
            bool bUseStartPos = fStartPos != 0f;

            // 좌측부터 기울어진 상태에서 시작
            int iStartGap = (iMuzzleCount / 2);
            float fAngle = fAngleMuzzle * -iStartGap;
            for (int i = 0; i < iMuzzleCount; i++)
            {
                Quaternion RotateAngle = Quaternion.AngleAxis(fAngle, vecAxis);
                Quaternion SpawnAngle = rot * RotateAngle;
                if(bUseStartPos)
                {
                    Vector3 vecSpawnPos = vecPos + (RotateAngle.Forward() * fStartPos);
                    listSpawnInfo.Add(new SpawnInfo(vecSpawnPos, SpawnAngle));
                }
                else
                {
                    listSpawnInfo.Add(new SpawnInfo(vecPos, SpawnAngle));
                }

                fAngle += fAngleMuzzle;
            }
        }

        public override void DoDrawGizmo_OnSelected(Transform pTransform)
        {
            base.DoDrawGizmo_OnSelected(pTransform);

            Vector3 vecPos = pTransform.position;
            Quaternion rot = pTransform.rotation;

            // 좌측부터 기울어진 상태에서 시작
            int iStartGap = (iMuzzleCount / 2);
            float fAngle = fAngleMuzzle * -iStartGap;
            for (int i = 0; i < iMuzzleCount; i++)
            {
                Quaternion RotateAngle = rot * Quaternion.AngleAxis(fAngle, vecAxis);
                Vector3 vecSpawnPos = vecPos + (RotateAngle.Forward() * fStartPos);
                Gizmos.DrawLine(vecSpawnPos, vecSpawnPos + (RotateAngle).Forward() * const_fGizmoDistance);

                fAngle += fAngleMuzzle;
            }
        }
    }

    [RegistSubString("원형샷")]
    public class CSpawnLogic_Circle : CSpawnLogic
    {
        const float const_fGizmoDistance = 10f;

        [DisplayName("총구 개수")]
        public int iMuzzleCount = 10;
        [DisplayName("총구 회전 축")]
        public Vector3 vecAxis = Vector3.forward;

        [Space(10)]
        [DisplayName("시작 위치 축")]
        public Vector3 vecStartPosAxis = Vector3.up;
        [DisplayName("시작 위치")]
        public float fStartPos = 0f;

        public override void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
        {
            Vector3 vecCurrentPos = pTransform.position;
            Quaternion rot = pTransform.rotation;

            float fAngleGap = 360 / iMuzzleCount;
            float fAngle = 0;
            bool bUseStartPos = fStartPos != 0f;

            for (int i = 0; i < iMuzzleCount; i++)
            {
                Quaternion RotateAngle = Quaternion.Euler(vecAxis * fAngle);
                if (bUseStartPos)
                {
                    Vector3 vecPos = vecCurrentPos + ((RotateAngle * vecStartPosAxis) * fStartPos);
                    listSpawnInfo.Add(new SpawnInfo(vecPos, RotateAngle));
                }
                else
                {
                    listSpawnInfo.Add(new SpawnInfo(vecCurrentPos, RotateAngle));
                }

                fAngle += fAngleGap;
            }
        }

        public override void DoDrawGizmo_OnSelected(Transform pTransform)
        {
            base.DoDrawGizmo_OnSelected(pTransform);

            Vector3 vecCurrentPos = pTransform.position;
            float fAngleGap = 360 / iMuzzleCount;
            float fAngle = 0;
            for (int i = 0; i < iMuzzleCount; i++)
            {
                Quaternion RotateAngle = Quaternion.Euler(new Vector3(0f, 0f, fAngle));
                fAngle += fAngleGap;

                Vector3 vecDirection = RotateAngle * vecStartPosAxis;
                Vector3 vecPos = vecCurrentPos + (vecDirection * fStartPos);
                Gizmos.DrawLine(vecPos, vecPos + (vecDirection * const_fGizmoDistance));
            }
        }

        protected override int GetMuzzleCount()
        {
            return iMuzzleCount;
        }
    }

    [RegistSubString("큐브 내에 랜덤 스폰")]
    public class CSpawnLogic_Random_InArea_Cube : CSpawnLogic
    {
        [DisplayName("스폰 개수")]
        public int iMuzzleCount = 5;

#if ODIN_INSPECTOR
        [HorizontalGroup("X")]
#endif
        [DisplayName("위치 X 최소값")]
        public float _fRandomPosX_Min = -5f;
#if ODIN_INSPECTOR
        [HorizontalGroup("X")]
#endif
        [DisplayName("위치 X 최대값")]
        public float _fRandomPosX_Max = 5f;

#if ODIN_INSPECTOR
        [HorizontalGroup("Y")]
#endif
        [DisplayName("위치 Y 최소값")]
        public float _fRandomPosY_Min = -5f;
#if ODIN_INSPECTOR
        [HorizontalGroup("Y")]
#endif
        [DisplayName("위치 Y 최대값")]
        public float _fRandomPosY_Max = 5f;

#if ODIN_INSPECTOR
        [HorizontalGroup("Z")]
#endif
        [DisplayName("위치 Z 최소값")]
        public float _fRandomPosZ_Min = -5f;
#if ODIN_INSPECTOR
        [HorizontalGroup("Z")]
#endif
        [DisplayName("위치 Z 최대값")]
        public float _fRandomPosZ_Max = 5f;


        public override void DoSpawnObject(Transform pTransform, ref List<SpawnInfo> listSpawnInfo)
        {
            Vector3 vecCurrentPos = pTransform.position;
            for (int i = 0; i < iMuzzleCount; i++)
            {
                float fPosX = Random.Range(_fRandomPosX_Min, _fRandomPosX_Max);
                float fPosY = Random.Range(_fRandomPosY_Min, _fRandomPosY_Max);
                float fPosZ = Random.Range(_fRandomPosZ_Min, _fRandomPosZ_Max);

                Vector3 vecRandomOffset = new Vector3(fPosX, fPosY, fPosZ);
                listSpawnInfo.Add(new SpawnInfo(vecCurrentPos + vecRandomOffset, Quaternion.identity));
            }
        }

        public override void DoDrawGizmo_OnSelected(Transform pTransform)
        {
            base.DoDrawGizmo_OnSelected(pTransform);

            Vector3 vecPos = pTransform.position;
            vecPos.x = vecPos.x + ((_fRandomPosX_Max + _fRandomPosX_Min) * 0.5f);
            vecPos.y = vecPos.y + ((_fRandomPosY_Min + _fRandomPosY_Max) * 0.5f);
            vecPos.z = vecPos.z + ((_fRandomPosZ_Min + _fRandomPosZ_Max) * 0.5f);

            Vector3 vecSize = Vector3.zero;
            vecSize.x = _fRandomPosX_Max - _fRandomPosX_Min;
            vecSize.y = _fRandomPosY_Max - _fRandomPosY_Min;
            vecSize.z = _fRandomPosZ_Max - _fRandomPosZ_Min;

            Gizmos.DrawWireCube(vecPos, vecSize);
        }

        protected override int GetMuzzleCount()
        {
            return iMuzzleCount;
        }
    }
}

