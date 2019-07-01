using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/* ============================================ 
   Editor      : Strix
   Date        : 2017-04-05 오후 1:24:58
   Description : 
   Edit Log    : 
   ============================================ */

#pragma warning disable CS0618 // 형식 또는 멤버는 사용되지 않습니다.

[RequireComponent(typeof(CCompoDontDestroyObj))]
public class CManagerUILocalize : CSingletonDynamicMonoBase<CManagerUILocalize>
{
	/* const & readonly declaration             */

	private const string const_strLocalePath = "Locale";
	private const string const_strLocaleFileExtension = ".loc";

	private static readonly char[] const_arrChrTrim = new char[] { '\t', '\r', ' ' };

    /* enum & struct declaration                */

    public interface ILocalizeListener
    {
        string ILocalizeListner_GetLocalizeKey();
        void ILocalizeListner_ChangeLocalize(SystemLanguage eLanguage, string strLocalizeValue);
    }

    /* public - Variable declaration            */

    public event System.Action p_EVENT_OnFinishParse_LocFile;
    public event System.Action p_EVENT_OnChangeLocalize;

    public Dictionary<SystemLanguage, Dictionary<string, List<string>>> p_mapLocaleData { get { return _mapLocaleData; } }
    public List<SystemLanguage> p_listLocale { get { return _listLocale; } }

    public SystemLanguage p_eCurrentLocalize { get; private set; }
    public bool p_bIsFinishParse { get; private set; }

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<SystemLanguage, Dictionary<string, List<string>>> _mapLocaleData = new Dictionary<SystemLanguage, Dictionary<string, List<string>>>();
    private Dictionary<string, List<ILocalizeListener>> _mapCompoLocalizeListener = new Dictionary<string, List<ILocalizeListener>>();

    private Dictionary<string, List<string>> _mapLocaleDataCurrent;
    private List<SystemLanguage> _listLocale = new List<SystemLanguage>();
    private StringBuilder _pStrBuilder = new StringBuilder();

	private int _iParsingFinishCount = 0;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출                         */

    //static public StringPair DoSplitText_OrEmpty(string strText, char chSplit = '=')
    //{
    //    string[] arrLang = strText.Split('=');
    //    if (arrLang.Length < 2) return StringPair.Empty;

    //    string strLocKey = arrLang[0].TrimStart(const_arrChrTrim).TrimEnd(const_arrChrTrim);
    //    string strLocValue = arrLang[1].TrimStart(const_arrChrTrim).TrimEnd(const_arrChrTrim).Replace("\\n", "\n");

    //    return new StringPair(strLocKey, strLocValue);
    //}
    
    public void DoStartParse_Locale(System.Action OnFinishParse)
	{
		p_eCurrentLocalize = SystemLanguage.Unknown;
		p_EVENT_OnFinishParse_LocFile += OnFinishParse;
		_iParsingFinishCount = 0;

        for (int i = 0; i <= (int)SystemLanguage.Unknown; i++)
            StartCoroutine(CoProcParse_Locale((SystemLanguage)i));
	}

    public void DoRegist_LocalizeListener(ILocalizeListener pListener, GameObject pObject_ForDebug)
    {
        string strKey = pListener.ILocalizeListner_GetLocalizeKey();
        if(string.IsNullOrEmpty(strKey))
        {
            Debug.LogError(pObject_ForDebug.name + " 로컬라이징 컴포넌트의 로컬라이징 키가 없습니다.", pObject_ForDebug);
            return;
        }

        if (_mapCompoLocalizeListener.ContainsKey(strKey) == false)
			_mapCompoLocalizeListener.Add(strKey, new List<ILocalizeListener>());

		_mapCompoLocalizeListener[strKey].Add(pListener);
	}

    public void DoRemove_LocalizeListener(ILocalizeListener pListener)
    {
        string strKey = pListener.ILocalizeListner_GetLocalizeKey();
        if(_mapCompoLocalizeListener.ContainsKey(strKey))
            _mapCompoLocalizeListener[strKey].Remove(pListener);
    }

	public void DoSetLocalize_CurrentScene()
	{
		if(p_bIsFinishParse == false)
		{
            // 파싱이 안끝났다면, 파싱종료 이벤트에 추가하고 종료
            p_EVENT_OnFinishParse_LocFile += DoSetLocalize_CurrentScene;
			return;
		}

		if (p_eCurrentLocalize == SystemLanguage.Unknown)
            Debug.LogWarning("DoSetLocalize_CurrentScene - p_eCurrentLocalize == SystemLanguage.Unknown");
		else
            DoSet_Localize(p_eCurrentLocalize);
    }

    public void DoSet_Localize(SystemLanguage eLocalize)
	{
        if (_mapLocaleData.ContainsKey(eLocalize) == false)
		{
            Debug.LogWarning("_mapLocaleData ContainsKey (" + eLocalize + ") == false");
            return;
		}

		p_eCurrentLocalize = eLocalize;
		_mapLocaleDataCurrent = _mapLocaleData[p_eCurrentLocalize];

        // 로컬라이징 리스너가 없으면 테스트 시 Awake에서 실행했을 가능성이 높다.
        // 실제는 로컬파일을 www에서 불러오기 때문에 Enable보다 느리게 들어온다.
        if (_mapCompoLocalizeListener.Count == 0)
        {
            CUICompoLocalizeBase[] arrLocalizeBase = FindObjectsOfType<CUICompoLocalizeBase>();
            for (int i = 0; i < arrLocalizeBase.Length; i++)
                DoRegist_LocalizeListener(arrLocalizeBase[i], arrLocalizeBase[i].gameObject);
        }
        IEnumerator<KeyValuePair<string, List<ILocalizeListener>>> pIter = _mapCompoLocalizeListener.GetEnumerator();
        while (pIter.MoveNext())
        {
            KeyValuePair<string, List<ILocalizeListener>> pCurrent = pIter.Current;
			string strKey = pCurrent.Key;
			List<ILocalizeListener> listValue = pCurrent.Value;
			if (_mapLocaleDataCurrent.ContainsKey(strKey))
            {
				for (int i = 0; i < listValue.Count; i++)
                    listValue[i].ILocalizeListner_ChangeLocalize(eLocalize, (DoGetCurrentLocalizeValue(strKey)));
            }
        }

		if (p_EVENT_OnChangeLocalize != null)
			p_EVENT_OnChangeLocalize();
	}

	public List<string> DoGetLocalizeValueContains(string strKey)
	{
		if (ProcCheckValidLocalizeValue(strKey) == false) return null;

		return _mapLocaleData[p_eCurrentLocalize][strKey];
	}

	public string DoGetCurrentLocalizeValue(string strKey, int iIndex = 0)
	{
		if (ProcCheckValidLocalizeValue(strKey) == false) return null;

		return _mapLocaleData[p_eCurrentLocalize][strKey][iIndex];
	}

    public string DoGetCurrentLocalizeValue_Random(string strKey)
    {
        if (ProcCheckValidLocalizeValue(strKey) == false) return null;

        return _mapLocaleData[p_eCurrentLocalize][strKey].GetRandom();
    }


    public string DoGetLocalizeRandomText_OrNull(string strKey)
	{
		List<string> listLocalizeValue = DoGetLocalizeValueContains(strKey);
		if (listLocalizeValue == null)
			return null;

        return listLocalizeValue[Random.Range(0, listLocalizeValue.Count)];
	}


    /* public - [Event] Function             
       프랜드 객체가 호출                       */

    public void EventAddCallBack_OnFinishParse(System.Action OnFinishParse)
	{
		if (p_bIsFinishParse)
			OnFinishParse();
		else
			p_EVENT_OnFinishParse_LocFile += OnFinishParse;
	}

    //public void EventSet_LocalizeParsing(SystemLanguage eLocale, string strText, byte[] arrTextByte, bool bIsFinish)
    //{
    //    if (CheckIsUTF8(arrTextByte))
    //        strText = Encoding.UTF8.GetString(arrTextByte, 3, arrTextByte.Length - 3);

    //    string[] arrStr = strText.Split('\n');
    //    for (int i = 0; i < arrStr.Length; i++)
    //    {
    //        string strLine = arrStr[i];
    //        if (string.IsNullOrEmpty(strLine) || strLine.StartsWith("//")) continue;

    //        StringPair pStringPair = DoSplitText_OrEmpty(strLine);
    //        if (StringPair.IsEmpty(pStringPair) == false)
    //            Regist_LocalizeData(eLocale, pStringPair.strKey, pStringPair.strValue);
    //    }

    //    if (bIsFinish)
    //        ExcuteFinishEvent();
    //}

    // ========================================================================== //

    /* protected - [abstract & virtual]         */

    /* protected - [Event] Function           
       자식 객체가 호출                         */

    /* protected - Override & Unity API         */

    protected override void OnAwake()
	{
		base.OnAwake();

		GetComponent<CCompoDontDestroyObj>()._bIsSingleton = true;
	}

    // ========================================================================== //

    /* private - [Proc] Function             
       중요 로직을 처리                         */

    private void Regist_LocalizeData(SystemLanguage eLocalize, string strKey, string strValue)
    {
        if (_mapLocaleData.ContainsKey(eLocalize) == false)
        {
            _listLocale.Add(eLocalize);
            _mapLocaleData.Add(eLocalize, new Dictionary<string, List<string>>());
        }

        if (_mapLocaleData[eLocalize].ContainsKey(strKey) == false)
            _mapLocaleData[eLocalize].Add(strKey, new List<string>());

        _mapLocaleData[eLocalize][strKey].Add(strValue);
    }

    private IEnumerator CoProcParse_Locale(SystemLanguage eLocale)
	{
		_pStrBuilder.Length = 0;
//#if UNITY_EDITOR
//		_pStrBuilder.Append("file://");
//#endif
		_pStrBuilder.Append(Application.streamingAssetsPath).Append("/").Append(const_strLocalePath).Append("/")
					.Append(eLocale.ToString()).Append(const_strLocaleFileExtension);

        WWW pReader = null;
        if (System.IO.File.Exists(_pStrBuilder.ToString()))
        {
            pReader = new WWW(_pStrBuilder.ToString());
            yield return pReader;
        }

        if (++_iParsingFinishCount >= (int)SystemLanguage.Unknown + 1)
			p_bIsFinishParse = true;

        if (pReader == null || pReader.error != null || pReader.bytes.Length == 0)
        {
            if (p_bIsFinishParse)
                ExcuteFinishEvent();

            yield break;
        }


        //EventSet_LocalizeParsing(eLocale, pReader.text, pReader.bytes, p_bIsFinishParse);
    }
    
	private bool ProcCheckValidLocalizeValue(string strKey)
	{
		return (_mapLocaleData.ContainsKey(p_eCurrentLocalize) &&
			    _mapLocaleData[p_eCurrentLocalize].ContainsKey(strKey) &&
			    _mapLocaleData[p_eCurrentLocalize][strKey].Count > 0);
	}

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

    private void ExcuteFinishEvent()
    {
        if (p_EVENT_OnFinishParse_LocFile != null)
        {
            p_EVENT_OnFinishParse_LocFile();
            p_EVENT_OnFinishParse_LocFile = null;
        }
    }

    private bool CheckIsUTF8(byte[] arrByte)
    {
        return arrByte.Length >= 3 && arrByte[0] == 239 && arrByte[1] == 187 && arrByte[2] == 191;
    }
}
