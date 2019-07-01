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

#pragma warning disable CS0162 // 접근할 수 없는 코드가 있습니다.

[ExecuteInEditMode]
public abstract class CSpawnerBase : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    [System.Flags]
    public enum ESpawn_WhenFlag
    {
        None = 0,
        OnAwake = 1 << 1,
        OnEnable = 1 << 2,
        OnDisable = 1 << 3,
    }

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

    [System.Serializable]
    public class SpawnSettingInfo
    {
        [DisplayName("스폰 오브젝트")]
        public GameObject pSpawnObject;

        [Range(0, 100)]
        [DisplayName("스폰확률 가중치")]
        public int iRandomPercent;

        public SpawnSettingInfo(GameObject pSpawnObject)
        {
            this.pSpawnObject = pSpawnObject;
        }
    }

    /* public - Field declaration            */

    public ObservableCollection<List<GameObject>> p_Event_OnSpawnObject { get; private set; } = new ObservableCollection<List<GameObject>>();

#if ODIN_INSPECTOR
    [HideInEditorMode]
    [ShowInInspector]
    [DisplayName("스폰된 오브젝트 리스트")]
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

    [DisplayName("프리뷰 생성")]
    public bool p_bPreview = true;

#if ODIN_INSPECTOR
    [EnumToggleButtons]
#endif
    [DisplayName("언제 자동 생성할지")]
    public ESpawn_WhenFlag p_eSpawn_When = ESpawn_WhenFlag.OnAwake;

    [DisplayName("스폰모듈")]
#if ODIN_INSPECTOR
    [ValueDropdown(nameof(GetValueDownList), DrawDropdownForListElements = false, ExcludeExistingValuesInList = true)/*, TypeFilter(nameof(GetTypeFilter))*/]
#endif
    public CSpawnLogic p_pSpawnLogic = new CSpawnLogic();

#if ODIN_INSPECTOR
    [ShowIf(nameof(OnCheck_IsDraw_SelectSpawnObject))]
    [ValueDropdown(nameof(GetSpawnObjectName_SpawnSettingInfo), DrawDropdownForListElements = false)]
    [LabelText("스폰할 오브젝트")]
#endif
    [Space(20)]
    public List<SpawnSettingInfo> p_listSpawnSetting;

    /* protected & private - Field declaration         */

    List<SpawnInfo> _listSpawnInfo = new List<SpawnInfo>();
    List<GameObject> _listSpawnedObject_Temp = new List<GameObject>();

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

    static public ValueDropdownList<T> Get_ExistFileNameList_UnityObject<T>(string strDataPath)
        where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(strDataPath) || Application.isEditor == false)
            return null;

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
    }

    [ShowIf(nameof(CheckIs_DrawDebugButton_And_PlayModeOnly))]
    [ButtonGroup("Debug_2", Order = 100)]
    [Button("Spawn One", ButtonSizes.Large)]
    public List<GameObject> DoSpawnObject_OnlyOne()
    {
        Delete_AllChildren();
        //for (int i = 0; i < p_listSpawnedObject.Count; i++)
        //    p_listSpawnedObject[i].SetActive(false);
        //p_listSpawnedObject.Clear();

        return DoSpawnObject();
    }

    [ShowIf(nameof(CheckIs_DrawDebugButton_And_PlayModeOnly))]
    [ButtonGroup("Debug_2", Order = 100)]
    [Button("Spawn", ButtonSizes.Large)]
    public List<GameObject> DoSpawnObject()
    {
        _listSpawnedObject_Temp.Clear();
        SpawnObject(GetSpawnObject_OriginalList());
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
        _strResourcesPath = Application.dataPath + "/Resources/";
        OnInit(ref _strResourcesPath);
        if(string.IsNullOrEmpty(_strResourcesPath) == false)
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

        if(Application.isPlaying && p_eSpawn_When.ContainEnumFlag(ESpawn_WhenFlag.OnEnable))
            DoSpawnObject();
    }

    protected override void OnDisableObject(bool bIsQuitApplciation)
    {
        base.OnDisableObject(bIsQuitApplciation);

        if (bIsQuitApplciation)
            return;

        if (Application.isPlaying && p_eSpawn_When.ContainEnumFlag(ESpawn_WhenFlag.OnDisable))
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

    private void OnDrawGizmosSelected()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
        {
            if (p_pSpawnLogic == null)
                p_pSpawnLogic = new CSpawnLogic();
            p_pSpawnLogic.DoDrawGizmo_OnSelected(transform);
        }
    }
#endif

    /* protected - [abstract & virtual]         */

    abstract protected void OnInit(ref string strSpawnObject_ContainFolderPath_Default_Is_ResourcesPath);
    virtual protected void OnSpawnObject(GameObject pObjectSpawned) { }
    virtual protected bool OnCheck_IsDraw_SelectSpawnObject() { return true; }
    virtual protected void OnCheck_IsPossibleDestroyChild(GameObject pObject, out bool bIsDestroy) { bIsDestroy = true; }

    virtual protected List<SpawnSettingInfo> GetSpawnObject_OriginalList()
    {
        return p_listSpawnSetting;
    }

    protected string GetDataPath()
    {
        return _strResourcesPath;
    }

#if ODIN_INSPECTOR
    virtual protected ValueDropdownList<CSpawnLogic> GetValueDownList() { return OdinExtension.GetValueDropDownList_SubString<CSpawnLogic>(); }
    virtual protected IEnumerable<System.Type> GetTypeFilter() { return OdinExtension.GetTypeFilter(typeof(CSpawnLogic)); }
#endif

    // ========================================================================== //

#region Private

    private void SpawnObject_EditorOnly()
    {
        UpdateSpawnInfo();

        // 전에 세팅한 스폰 오브젝트와 현재 세팅한 스폰오브젝트가 같은지 확인해야 함
        if (p_listSpawnedObject.Count == _listSpawnInfo.Count)
        {
            for (int i = 0; i < p_listSpawnedObject.Count; i++)
            {
                if (i >= _listSpawnInfo.Count)
                    continue;

                Transform pTransformCurrent = p_listSpawnedObject[i].transform;
                pTransformCurrent.rotation = _listSpawnInfo[i].rot;
                pTransformCurrent.position = _listSpawnInfo[i].vecPos;
            }
            return;
        }

        Delete_AllChildren();
        _listSpawnedObject_Temp.Clear();

        if (p_listSpawnSetting != null && p_listSpawnSetting.Count != 0)
        {
            SpawnObject(GetSpawnObject_OriginalList());
        }
    }

    private void SpawnObject(List<SpawnSettingInfo> listSpawnSetting)
    {
        if (listSpawnSetting == null || listSpawnSetting.Count == 0)
            return;

        UpdateSpawnInfo();

        for (int i = 0; i < _listSpawnInfo.Count; i++)
            _listSpawnedObject_Temp.Add(SpawnObject(GetRandomSpawnObject(listSpawnSetting), _listSpawnInfo[i].vecPos, _listSpawnInfo[i].rot));
    }

    private void UpdateSpawnInfo()
    {
        _listSpawnInfo.Clear();
        if (p_pSpawnLogic == null)
            p_pSpawnLogic = new CSpawnLogic();
        p_pSpawnLogic.DoSpawnObject(transform, ref _listSpawnInfo);
    }

    private GameObject SpawnObject(GameObject pObjectOriginal, Vector3 vecPosition, Quaternion rotRotation)
    {
        if(pObjectOriginal == null)
        {
            Debug.LogError(name + " SpawnObject Error - pObjectOriginal == null", this);
            return null;
        }

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

        if(gameObject.activeSelf)
        {
            pObjectCopy.transform.SetParent(transform);
            pObjectCopy.transform.position = vecPosition;
            pObjectCopy.transform.rotation = rotRotation;
            pObjectCopy.SetActive(true);
        }

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

    private GameObject GetRandomSpawnObject(List<SpawnSettingInfo> listSpawnSettingInfo)
    {
        int iTotalPercent = 0;
        for(int i = 0; i < listSpawnSettingInfo.Count; i++)
            iTotalPercent += listSpawnSettingInfo[i].iRandomPercent;

        int iRandomPercent = Random.Range(0, iTotalPercent);
        iTotalPercent = 0;
        for(int i = 0; i < listSpawnSettingInfo.Count; i++)
        {
            iTotalPercent += listSpawnSettingInfo[i].iRandomPercent;
            if (iRandomPercent < iTotalPercent)
                return listSpawnSettingInfo[i].pSpawnObject;
        }

        return null;
    }

#if ODIN_INSPECTOR
    private ValueDropdownList<GameObject> GetSpawnObjectName()
    {
        return Get_ExistFileNameList_UnityObject<GameObject>(GetDataPath());
    }

    private ValueDropdownList<SpawnSettingInfo> GetSpawnObjectName_SpawnSettingInfo()
    {
        ValueDropdownList<SpawnSettingInfo> listReturn = new ValueDropdownList<SpawnSettingInfo>();
        var listPrefab = Get_ExistFileNameList_UnityObject<GameObject>(GetDataPath());
        if (listPrefab == null)
            return null;

        for (int i = 0; i < listPrefab.Count; i++)
            listReturn.Add(listPrefab[i].Text, new SpawnSettingInfo(listPrefab[i].Value));

        return listReturn;
    }
#endif

    #endregion Private
}
// ========================================================================== //

#if ODIN_INSPECTOR
#if UNITY_EDITOR

public class CSpawnerBaseDrawer<CLASS_SPAWNPOIONT_DERIVED> : OdinValueDrawer<CLASS_SPAWNPOIONT_DERIVED>
    where CLASS_SPAWNPOIONT_DERIVED : CSpawnerBase
{
    protected override void DrawPropertyLayout(GUIContent label)
    {
        if (this.ValueEntry.SmartValue != null && this.ValueEntry.SmartValue.p_pSpawnLogic == null)
            this.ValueEntry.SmartValue.p_pSpawnLogic = new CSpawnLogic();

        base.CallNextDrawer(label);
    }
}

#endif
#endif