#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-02-15 오후 3:07:10
 *	기능 : 
 *	정상 동작하려면 OdinInspector 에셋이 필요합니다.
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public abstract class CSpawnPointBase : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.ValueDropdown(nameof(GetEnumSpawnName))]
#endif
    [Rename_Inspector("스폰대상이름(리소스폴더안)")]
    public string p_strSpawnObjectName;
    [Rename_Inspector("Awake 시 자동 생성")]
    public bool bIsSpawn_OnAwake = true;
    [Rename_Inspector("세팅 칼라")]
    public Color p_pColor_Spawn = Color.white;

    /* protected & private - Field declaration         */

    int _iOriginalSpawnObject_HashCode;
    string _strResourcesPath;
    System.Type _pEnumSpawnNameType;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoSpawnObject_OnlyOne(bool bIsForce)
    {
        GameObject pObjectOriginal = Resources.Load(_strResourcesPath + p_strSpawnObjectName) as GameObject;
        if (bIsForce == false)
        {
            if (transform.childCount != 0 && (pObjectOriginal != null && _iOriginalSpawnObject_HashCode == pObjectOriginal.GetHashCode()))
                return;
        }

        Delete_AllChildren();
        if (pObjectOriginal != null)
        {
            _iOriginalSpawnObject_HashCode = pObjectOriginal.GetHashCode();
            SpawnObject(pObjectOriginal);
        }
        else
            _iOriginalSpawnObject_HashCode = -1;
    }

    public void DoSpawnObject()
    {
        SpawnObject(Resources.Load(_strResourcesPath + p_strSpawnObjectName) as GameObject);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        OnInit(out _strResourcesPath, out _pEnumSpawnNameType);
        _strResourcesPath += "/";

        if (bIsSpawn_OnAwake)
            DoSpawnObject_OnlyOne(Application.isPlaying);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
            return;

        if(transform.childCount != 0)
            PrefabUtility.RevertObjectOverride(transform.GetChild(0), InteractionMode.AutomatedAction);

        DoSpawnObject_OnlyOne(false);
    }
#endif

    /* protected - [abstract & virtual]         */

    abstract protected void OnInit(out string strResourcesPath, out System.Type pEnumSpawnNameType);
    virtual protected void OnSpawnObject(GameObject pObjectSpawned) { }

    // ========================================================================== //

    #region Private

    private void SpawnObject(GameObject pObjectOriginal)
    {
        GameObject pObjectCopy = null;

        if (Application.isPlaying == false)
        {
#if UNITY_EDITOR
            pObjectCopy = PrefabUtility.InstantiatePrefab(pObjectOriginal) as GameObject;
            pObjectCopy.transform.position = transform.position;
            pObjectCopy.transform.rotation = transform.rotation;
#endif
        }
        else
        {
            pObjectCopy = GameObject.Instantiate(pObjectOriginal, transform.position, transform.rotation, transform);
        }

        pObjectCopy.SetActive(true);
        pObjectCopy.name = name + "_" + p_strSpawnObjectName;
        pObjectCopy.transform.SetParent(transform);
        pObjectCopy.transform.DoResetTransform();

        OnSpawnObject(pObjectCopy);
    }

    private void Delete_AllChildren()
    {
        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }

    private string[] GetEnumSpawnName()
    {
        if (_pEnumSpawnNameType == null)
        {
            OnAwake();
            if (_pEnumSpawnNameType == null)
                return null;
        }

        return _pEnumSpawnNameType.GetEnumNames();
    }


    #endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test