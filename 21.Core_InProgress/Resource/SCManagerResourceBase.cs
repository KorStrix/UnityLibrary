using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

// ============================================ 
// Editor      : Strix                               
// Date        : 2017-01-29 오후 3:25:00
// Description : Core 레벨에서 사용할 수 있도록 Monobehaviour로 안되있는 Resource 매니져. 싱글톤.
// Edit Log    : 
// ============================================ 

public enum EResourcePath
{
	Resources,
	StreamingAssets,
	PersistentDataPath
}

public class SCManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>
    where CLASS : SCManagerResourceBase<CLASS, ENUM_RESOURCE_NAME, RESOURCE>, new()
    where ENUM_RESOURCE_NAME : System.IConvertible, System.IComparable
    where RESOURCE : UnityEngine.Object
{
    // ===================================== //
    // public - Variable declaration         //
    // ===================================== //

    public Dictionary<ENUM_RESOURCE_NAME, RESOURCE> p_mapResourceOrigin { get { return _mapResourceOrigin; } }
    static public CLASS instance {  get { return _pInstance; } }

    // ===================================== //
    // protected - Variable declaration      //
    // ===================================== //

    protected Dictionary<ENUM_RESOURCE_NAME, RESOURCE> _mapResourceOrigin = new Dictionary<ENUM_RESOURCE_NAME, RESOURCE>();
    protected MonoBehaviour _pBase;

	// ===================================== //
	// private - Variable declaration        //
	// ===================================== //

	static private CLASS _pInstance;

    protected string _strResourceLocalPath = null;
    protected EResourcePath _eResourcePath;
    protected string _strFolderPath;
    private StringBuilder _pStrBuilder = new StringBuilder();

	// ========================================================================== //

	// ===================================== //
	// public - [Do] Function                //
	// 외부 객체가 요청                      //
	// ===================================== //

	static public CLASS DoMakeInstance(MonoBehaviour pBaseClass, string strFolderPath, EResourcePath eResourcePath = EResourcePath.Resources)
    {
		CLASS pInstance = new CLASS();
		pInstance._pBase = pBaseClass;
        pInstance._eResourcePath = eResourcePath;
        pInstance._strResourceLocalPath = strFolderPath;
        bool bIsMultipleResource = false;
        pInstance.OnMakeClass(pBaseClass, ref bIsMultipleResource);

        if (eResourcePath == EResourcePath.Resources)
        {
            pInstance.InitResourceOrigin();
            pInstance.OnMakeClass_AfterInitResource(pBaseClass);
        }

		_pInstance = pInstance;

		return pInstance;
    }

    public void DoStartCo_GetStreammingAssetResource<TResource>(string strFolderPath, ENUM_RESOURCE_NAME eResourceName, System.Action<bool, TResource> OnGetResource)
    {
        _pBase.StartCoroutine(CoGetResource_StreammingAsset(strFolderPath, eResourceName.ToString(), OnGetResource));
    }

    public void DoStartCo_GetStreammingAssetResource<TResource>(ENUM_RESOURCE_NAME eResourceName, System.Action<bool, TResource> OnGetResource)
    {
        _pBase.StartCoroutine(CoGetResource_StreammingAsset(_strFolderPath, eResourceName.ToString(), OnGetResource));
    }

    public void DoStartCo_GetStreammingAssetResource_Array<TResource>(ENUM_RESOURCE_NAME eResourceName, System.Action<bool, TResource[]> OnGetResource)
    {
        _pBase.StartCoroutine(CoGetResource_StreammingAsset_Array(eResourceName.ToString(), OnGetResource));
    }

    // ===================================== //
    // public - [Getter And Setter] Function //
    // ===================================== //

    public RESOURCE DoGetResource_Origin(ENUM_RESOURCE_NAME eResourceName)
    {
        RESOURCE pFindResource;
        if (_mapResourceOrigin.TryGetValue(eResourceName, out pFindResource) == false)
            Debug.LogWarning(string.Format("{0}을 찾을 수 없습니다.", eResourceName));

        return pFindResource;
    }

	// ========================================================================== //

	// ===================================== //
	// protected - [Event] Function          //
	// 프랜드 객체가 요청                    //
	// ===================================== //

	protected virtual void OnMakeClass(MonoBehaviour pBaseClass, ref bool bIsMultipleResource) { }
    protected virtual void OnMakeClass_AfterInitResource(MonoBehaviour pBaseClass) { }
    protected virtual bool OnWWWToResource<TResource>(WWW www, ref TResource pResource) { return false; }
    protected virtual bool OnWWWToResource_Array<TResource>(WWW www, ref TResource[] arrResource) { return false; }
    protected virtual string OnGetFileExtension() { return ""; }

    // ===================================== //
    // protected - Unity API                 //
    // ===================================== //

    // ========================================================================== //

    // ===================================== //
    // private - [Proc] Function             //
    // 중요 로직을 처리                      //
    // ===================================== //

    protected void InitResourceOrigin()
    {
		_mapResourceOrigin.Clear();
		RESOURCE[] arrResources = Resources.LoadAll<RESOURCE>(_strResourceLocalPath + "/");
		if(typeof(ENUM_RESOURCE_NAME).IsEnum)
		{
			for (int i = 0; i < arrResources.Length; i++)
			{
				ENUM_RESOURCE_NAME eResourceName = default( ENUM_RESOURCE_NAME );
				if (arrResources[i].name.ConvertEnum( out eResourceName ))
				{
					if(_mapResourceOrigin.ContainsKey( eResourceName ))
					{
						Debug.LogWarning( "Error - Already Contas Key : " + eResourceName );
						continue;
					}
					_mapResourceOrigin.Add( eResourceName, arrResources[i] );
				}
			}
		}
		else
		{
			for (int i = 0; i < arrResources.Length; i++)
            {
                if (_mapResourceOrigin.ContainsKey((ENUM_RESOURCE_NAME)(object)arrResources[i].name))
                    Debug.LogWarning("InitResourceOrigin Key : " + (ENUM_RESOURCE_NAME)(object)arrResources[i].name, arrResources[i]);
                else
                    _mapResourceOrigin.Add((ENUM_RESOURCE_NAME)(object)arrResources[i].name, arrResources[i]);
            }
        }
	}

    private IEnumerator CoGetResource_StreammingAsset<TResource>(string strFolderPath, string strResourceName, System.Action<bool, TResource> OnGetResource)
    {
        _pStrBuilder.Length = 0;
#if UNITY_EDITOR
		_pStrBuilder.Append("file://");
#endif
        _pStrBuilder.Append(strFolderPath).Append("/").Append(strResourceName).Append(OnGetFileExtension());

        // Debug.Log(" _pStrBuilder : " + _pStrBuilder.ToString());
        WWW www = new WWW(_pStrBuilder.ToString());
        yield return www;
		
		if (www.error != null && www.error.Length != 0)
        {
            Debug.LogWarning(www.error);
            OnGetResource(false, default(TResource));
        }
        else
        {
            TResource pResource = default(TResource);
            if (OnWWWToResource(www, ref pResource))
                OnGetResource(true, pResource);
            else
                Debug.LogWarning(string.Format("{0}이 {1}을 WWW To Resource 변환 중 에러가 났다.", GetType().ToString(), strResourceName));
        }

        yield break;
    }

    private IEnumerator CoGetResource_StreammingAsset_Array<TResource>(string strResourceName, System.Action<bool, TResource[]> OnGetResource)
    {
        _pStrBuilder.Length = 0;
//#if UNITY_EDITOR
//		_pStrBuilder.Append("file://");
//#endif
		_pStrBuilder.Append(_strFolderPath);
        _pStrBuilder.Append("/");
        _pStrBuilder.Append(strResourceName + OnGetFileExtension());

        if(System.IO.File.Exists(_pStrBuilder.ToString()) == false)
        {
            Debug.LogWarning(_pStrBuilder.ToString() + " 파일이 존재하지 않습니다");
            OnGetResource(false, null);
            yield break;
        }

        WWW www = new WWW(_pStrBuilder.ToString());
        yield return www;

        if (www.error != null && www.error.Length != 0)
        {
            Debug.LogWarning(www.error);
            OnGetResource(false, null);
            yield break;
        }

        TResource[] arrResource = null;
        if (OnWWWToResource_Array(www, ref arrResource))
            OnGetResource(true, arrResource);
        else
            Debug.LogWarning(string.Format("{0}이 {1}을 WWW To Resource 변환 중 에러가 났다.", GetType().ToString(), strResourceName));

        yield break;
    }

    // ===================================== //
    // private - [Other] Function            //
    // 찾기, 계산 등의 비교적 단순 로직      //
    // ===================================== //


}
