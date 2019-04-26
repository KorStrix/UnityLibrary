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
using System.IO;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

#endif

[ExecuteInEditMode]
public abstract class CSpawnPointBase : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Serializable]
    public struct SpawnInfo
    {
        public Vector3 vecPos;
        public Quaternion rot;

        public SpawnInfo(Vector3 vecPos, Quaternion rot)
        {
            this.vecPos = vecPos;
            this.rot = rot;
        }
    }

    [System.Flags]
    public enum ESpawn_WhenFlag
    {
        None = 0,
        OnAwake = 1 << 1,
        OnEnable = 1 << 2,
        OnDisable = 1 << 3,
    }

    /* public - Field declaration            */

    public CObserverSubject<List<GameObject>> p_Event_OnSpawnObject { get; private set; } = new CObserverSubject<List<GameObject>>();

#if ODIN_INSPECTOR
    [HideInEditorMode]
    [ShowInInspector]
    [Rename_Inspector("스폰된 오브젝트 리스트")]
#endif
    public List<GameObject> p_listSpawnedObject { get; private set; } = new List<GameObject>();
    public GameObject p_pObject_Spawned_Last
    {
        get
        {
            if (p_listSpawnedObject.Count == 0)
                return null;

            return p_listSpawnedObject[p_listSpawnedObject.Count - 1];
        }
    }

    public GameObject p_pObject_Spawned_First
    {
        get
        {
            if (p_listSpawnedObject.Count == 0)
                return null;

            return p_listSpawnedObject[0];
        }
    }

#if ODIN_INSPECTOR
    [ShowIf(nameof(OnCheck_IsDrawHelpBox))]
    [ValueDropdown(nameof(GetSpawnObjectName))]
#endif
    [Space(10)]
    [Rename_Inspector("스폰대상이름(리소스폴더안)")]
    public GameObject p_pSpawnObject;

    [Rename_Inspector("프리뷰 생성")]
    public bool p_bPreview = true;

#if ODIN_INSPECTOR
    [EnumToggleButtons]
#endif
    [Rename_Inspector("언제 자동 생성할지")]
    public ESpawn_WhenFlag p_eSpawn_When = ESpawn_WhenFlag.OnAwake;

    [Space(10)]
    [Rename_Inspector("스폰시 칼라")]
    public Color p_pColor_Spawn = Color.white;

    [Rename_Inspector("스폰모듈")]
#if ODIN_INSPECTOR
    [TypeFilter(nameof(GetTypeFilter)), ValueDropdown(nameof(GetValueDownList)), ListDrawerSettings(ShowIndexLabels = false, ListElementLabelName = "IHasName_GetName")]
#endif
    public CSpawnLogicBase p_pSpawnLogic = new CSpawnLogicBase();

    /* protected & private - Field declaration         */

    List<SpawnInfo> _listSpawnInfo = new List<SpawnInfo>();
    List<GameObject> _listSpawnedObject_Temp = new List<GameObject>();

    string _strObjectOriginalNameLast;
    string _strResourcesPath;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public string[] Get_ExistFileNameList(string strDataPath)
    {
#if UNITY_EDITOR
        DirectoryInfo pDirectoryInfo = new DirectoryInfo(strDataPath);
        if (pDirectoryInfo == null || pDirectoryInfo.Exists == false)
        {
            Debug.LogError("Error - GetSpawnName DirectoryInfo Is Null - Path : " + strDataPath);
            return null;
        }

        FileInfo[] fileInfo = pDirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
        List<string> listString = new List<string>();

        for (int i = 0; i < fileInfo.Length; i++)
        {
            if (fileInfo[i].Extension.Contains("meta"))
                continue;

            listString.Add(fileInfo[i].Name.Replace(fileInfo[i].Extension, ""));
        }

        return listString.ToArray();
#endif
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
        return null;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
    }

#if ODIN_INSPECTOR
    static public ValueDropdownList<T> Get_ExistFileNameList_UnityObject<T>(string strDataPath)
        where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        ValueDropdownList<T> list = new ValueDropdownList<T>();

        DirectoryInfo pDirectoryInfo = new DirectoryInfo(strDataPath);
        if (pDirectoryInfo == null || pDirectoryInfo.Exists == false)
        {
            Debug.LogError("Error - GetSpawnName DirectoryInfo Is Null - Path : " + strDataPath);
            return null;
        }

        FileInfo[] fileInfo = pDirectoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
        for (int i = 0; i < fileInfo.Length; i++)
        {
            FileInfo pFileInfo = fileInfo[i];
            if (pFileInfo.Extension.Contains("meta"))
                continue;

            string strName = pFileInfo.FullName.Replace("\\", "/");
            int iCutIndex = strName.IndexOf("Assets");
            string strPath = strName.Substring(iCutIndex);

            T pObject = AssetDatabase.LoadAssetAtPath<T>(strPath);
            list.Add(fileInfo[i].Name.Replace(pFileInfo.Extension, ""), pObject);
        }

        return list;
#endif
#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.
        return null;
#pragma warning restore CS0162 // 접근할 수 없는 코드가 있습니다.
    }
#endif

    public List<GameObject> DoSpawnObject_OnlyOne()
    {
        for (int i = 0; i < p_listSpawnedObject.Count; i++)
            p_listSpawnedObject[i].SetActive(false);
        p_listSpawnedObject.Clear();

        return DoSpawnObject();
    }

    public List<GameObject> DoSpawnObject()
    {
        _listSpawnedObject_Temp.Clear();
        SpawnObject(GetOriginalObject());
        p_Event_OnSpawnObject.DoNotify(_listSpawnedObject_Temp);

        return _listSpawnedObject_Temp;
    }

    public void DoDisable_AllSpawnObject()
    {
        var arrSpawnObject = p_listSpawnedObject.ToArray();
        for (int i = 0; i < arrSpawnObject.Length; i++)
            arrSpawnObject[i].SetActive(false);
    }

    public void Event_OnReturnSpawnObject(CSpawnedObject pSpawnedObject)
    {
        p_listSpawnedObject.Remove(pSpawnedObject.gameObject);
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        _listSpawnedObject_Temp.Clear();
        OnInit(out _strResourcesPath);
        _strResourcesPath += "/";

        if(Application.isPlaying)
        {
            if (p_eSpawn_When.ContainEnumFlag(ESpawn_WhenFlag.OnAwake) && transform.childCount == 0)
                DoSpawnObject();

            if (p_eSpawn_When.ContainEnumFlag(ESpawn_WhenFlag.OnAwake) == false)
                Delete_AllChildren();
        }
    }

    protected override void OnEnableObject()
    {
        base.OnEnableObject();

        if(p_eSpawn_When.ContainEnumFlag(ESpawn_WhenFlag.OnEnable))
            DoSpawnObject();
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
    {
        base.OnDisableObject(bIsQuitApplciation);

        if (p_eSpawn_When.ContainEnumFlag(ESpawn_WhenFlag.OnDisable))
            DoSpawnObject();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.isPlaying)
            return;

        if (p_bPreview)
            SpawnObject_EditorOnly();
        else
            Delete_AllChildren();
    }

    private void OnDrawGizmos()
    {
        if(CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            if (p_pSpawnLogic == null)
                p_pSpawnLogic = new CSpawnLogicBase();
            p_pSpawnLogic.DoDrawGizmo(transform);
        }
    }
#endif

    /* protected - [abstract & virtual]         */

    abstract protected void OnInit(out string strResourcesPath);
    virtual protected void OnSpawnObject(GameObject pObjectSpawned) { }
    virtual protected bool OnCheck_IsDrawHelpBox() { return true; }
    virtual protected void OnCheck_IsPossibleDestroyChild(GameObject pObject, out bool bIsDestroy) { bIsDestroy = true; }

    virtual protected GameObject GetOriginalObject()
    {
        return p_pSpawnObject;
    }

    virtual protected string GetDataPath()
    {
        return Application.dataPath + "/Resources/" + _strResourcesPath;
    }

#if ODIN_INSPECTOR
    virtual protected ValueDropdownList<CSpawnLogicBase> GetValueDownList() { return OdinExtension.GetValueDropDownList_HasName<CSpawnLogicBase>(); }
    virtual protected IEnumerable<System.Type> GetTypeFilter() { return OdinExtension.GetTypeFilter(typeof(CSpawnLogicBase)); }
#endif

    // ========================================================================== //

#region Private

    private void SpawnObject_EditorOnly()
    {
        _listSpawnedObject_Temp.Clear();

        GameObject pObjectOriginal = GetOriginalObject();
        if (p_pObject_Spawned_Last != null && (pObjectOriginal != null && pObjectOriginal.name.Equals(_strObjectOriginalNameLast)))
            return;
        Delete_AllChildren();

        if (pObjectOriginal != null)
        {
            SpawnObject(pObjectOriginal);
            _strObjectOriginalNameLast = pObjectOriginal.name;
        }
    }

    int iSpawnIgnoreCount = 0;
    private void SpawnObject(GameObject pObjectOriginal)
    {
        if (pObjectOriginal == null || pObjectOriginal.gameObject == null)
        {
            if(iSpawnIgnoreCount++ > 10)
                Debug.LogError(name + "Error - SpawnObject is Null", this);

            return;
        }

        iSpawnIgnoreCount = 0;
        _listSpawnInfo.Clear();
        if (p_pSpawnLogic == null)
            p_pSpawnLogic = new CSpawnLogicBase();
        p_pSpawnLogic.DoSpawnObject(transform, ref _listSpawnInfo);

        for(int i = 0; i < _listSpawnInfo.Count; i++)
            _listSpawnedObject_Temp.Add(SpawnObject(pObjectOriginal, _listSpawnInfo[i].vecPos, _listSpawnInfo[i].rot));
    }

    private GameObject SpawnObject(GameObject pObjectOriginal, Vector3 vecPosition, Quaternion rotRotation)
    {
        GameObject pObjectCopy = null;

#if UNITY_EDITOR
        if (Application.isPlaying == false)
        {
            pObjectCopy = PrefabUtility.InstantiatePrefab(pObjectOriginal) as GameObject;
            if (pObjectCopy == null)
            {
                pObjectCopy = GameObject.Instantiate(pObjectOriginal);
                pObjectCopy.name = pObjectOriginal.name;
            }
        }
#endif

        if (pObjectCopy == null)
        {
            pObjectCopy = CManagerPooling_Component<Transform>.instance.DoPop(pObjectOriginal.transform).gameObject;
            CSpawnedObject pSpawnedObject = pObjectCopy.GetComponent<CSpawnedObject>();
            if (pSpawnedObject == null)
                pSpawnedObject = pObjectCopy.AddComponent<CSpawnedObject>();
            pSpawnedObject.DoInit(this);
        }

        if (pObjectCopy == null)
            return null;

        pObjectCopy.transform.SetParent(transform);
        pObjectCopy.transform.position = vecPosition;
        pObjectCopy.transform.rotation = rotRotation;
        pObjectCopy.SetActive(true);

        p_listSpawnedObject.Add(pObjectCopy);
        OnSpawnObject(pObjectCopy);

        return pObjectCopy;
    }

    private void Delete_AllChildren()
    {
        GameObject[] arrSpawnObject = p_listSpawnedObject.ToArray();
        p_listSpawnedObject.Clear();

        for (int i = 0; i < arrSpawnObject.Length; i++)
            DestroyImmediate(arrSpawnObject[i]);

        Transform[] arrChildren = transform.GetComponentsInChildren<Transform>();
        for(int i = 0; i < arrChildren.Length; i++)
        {
            Transform pTransform = arrChildren[i];
            if (pTransform == null)
                continue;

            if (transform == pTransform || transform != pTransform.parent)
                continue;

            GameObject pObject = pTransform.gameObject;
            bool bIsPossible_Destroy;
            OnCheck_IsPossibleDestroyChild(pObject, out bIsPossible_Destroy);

            if (bIsPossible_Destroy)
                DestroyImmediate(pObject);
        }
    }

#if ODIN_INSPECTOR
    private ValueDropdownList<GameObject> GetSpawnObjectName()
    {
        return Get_ExistFileNameList_UnityObject<GameObject>(GetDataPath());
    }
#endif

#endregion Private
}
// ========================================================================== //

#region Test
#if UNITY_EDITOR

#endif
#endregion Test

#if ODIN_INSPECTOR
#if UNITY_EDITOR

public class CSpawnPointBaseDrawer<CLASS_SPAWNPOIONT_DRIVEN> : OdinValueDrawer<CLASS_SPAWNPOIONT_DRIVEN>
    where CLASS_SPAWNPOIONT_DRIVEN : CSpawnPointBase
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (this.ValueEntry.SmartValue != null && this.ValueEntry.SmartValue.p_pSpawnLogic == null)
            this.ValueEntry.SmartValue.p_pSpawnLogic = new CSpawnLogicBase();

        base.CallNextDrawer(label);
    }
}

#endif
#endif