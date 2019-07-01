#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-03-31 오후 4:23:32
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class CSpawnedObject : CObjectBase
{
    [DisplayName("스폰 포인트")]
    [SerializeField]
    CSpawnerBase _pSpawnPointOwner;

    public void DoInit(CSpawnerBase pSpawnPointOwner)
    {
        _pSpawnPointOwner = pSpawnPointOwner;
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
    {
        base.OnDisableObject(bIsQuitApplciation);

        if (bIsQuitApplciation)
            return;

        CManagerPooling_Component<Transform>.instance.DoPush(gameObject);

        if(_pSpawnPointOwner != null)
        {
            _pSpawnPointOwner.Event_OnReturnSpawnObject(this);
            _pSpawnPointOwner = null;
        }
    }

    protected override void OnDestroyObject(bool bIsQuitApplciation)
    {
        base.OnDestroyObject(bIsQuitApplciation);

        if (bIsQuitApplciation)
            return;

        CManagerPooling_Component<Transform>.instance.Event_RemovePoolObject(gameObject);
    }
}