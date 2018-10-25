using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : KJH
 *	
 *	기능 : 
   ============================================ */
#endregion Header

[System.Serializable]
public class SDataTable_Dialogue : IDictionaryItem<string>, System.IComparable<SDataTable_Dialogue>
{
	public struct SDialogueEvent
	{
		public string str배우;
		public string str대사;
		public string str표정;

		public SDialogueEvent( SDataTable_Dialogue pDialogueData)
		{
			this.str배우 = pDialogueData.str배우;
			this.str대사 = pDialogueData.str대사;
			this.str표정 = pDialogueData.str표정;
		}
	}

	public string str대사이벤트;

	public int i대화그룹;
	public int i대사순서;

	public string str배우;
	public string str대사;
	public string str표정;

	public string str이벤트함수이름;

	public string IDictionaryItem_GetKey()
	{
		return str대사이벤트;
	}

	public int CompareTo( SDataTable_Dialogue other )
	{
		return i대사순서.CompareTo( other.i대사순서 );
	}
}

public interface IDialogueListner 
{
	void IDialogueListner_OnPlayDialogue( SDataTable_Dialogue pDialogueData, bool bIsLast );
    void IDialogueListner_OnStartDialogue(SDataTable_Dialogue pDialogueData);
    void IDialogueListner_OnFinishDialogue();
}

public class CManagerDialogue : CSingletonNotMonoBase<CManagerDialogue>
{
	/* const & readonly declaration             */

	public const string const_strResourcesJsonDataPath = "JsonData";
	public const string const_strResourcesFileName = "캐릭터대사";

	/* enum & struct declaration                */

	/* public - Variable declaration            */

	/* protected - Variable declaration         */

	/* private - Variable declaration           */

	private Dictionary<string, List<SDataTable_Dialogue>> _mapDataTable_Dialogue = new Dictionary<string, List<SDataTable_Dialogue>>();
	private Dictionary<string, List<IDialogueListner>> _mapDialogueListner = new Dictionary<string, List<IDialogueListner>>();

    private List<SDataTable_Dialogue> _listEmpty = new List<SDataTable_Dialogue>();
	private List<SDataTable_Dialogue> _listDialogueCurrent
    {
        get
        {
            if (_strCurrentEvent != null && _mapDataTable_Dialogue.ContainsKey(_strCurrentEvent))
                return _mapDataTable_Dialogue[_strCurrentEvent];
            else
                return _listEmpty;
        }
    }

	private string _strCurrentEvent;
	private int _iCurDialogueIndex;
	private bool _bEndOfDialogue;

	// ========================================================================== //

	/* public - [Do] Function
     * 외부 객체가 호출                         */

	public void DoAddListner( string strListenEvent, IDialogueListner pListner )
	{
		if (_mapDialogueListner.ContainsKey(strListenEvent) == false)
			_mapDialogueListner.Add(strListenEvent, new List<IDialogueListner>());
		else
			_mapDialogueListner[strListenEvent].Clear(); // 한번 더 들어올때는 이미 파괴된 리스너라서 클리어 해줘야함

		_mapDialogueListner[strListenEvent].Add( pListner );
	}

	public void DoStart_DialogueScene(string strDialogueEvent) { _strCurrentEvent = strDialogueEvent; EventStart_DialogueScene(); }
	public void DoPrev_DialogueScene() { EventPrev_DialogueScene(); }
	public void DoNext_DialogueScene() { EventNext_DialogueScene(); }
    
	/* private - [Do] Function
	 * 내부 객체가 호출                         */

	/* public - [Event] Function             
       프랜드 객체가 호출                       */

	// ========================================================================== //

	/* protected - [abstract & virtual]         */

	/* protected - [Event] Function           
       자식 객체가 호출                         */

	private void EventStart_DialogueScene() { _bEndOfDialogue = false;  EventSet_DialogueScene( 0); }
	private void EventNext_DialogueScene() { EventSet_DialogueScene( ++_iCurDialogueIndex ); }
	private void EventPrev_DialogueScene() { EventSet_DialogueScene( --_iCurDialogueIndex ); }

	private void EventSet_DialogueScene(int iDialogueScene)
	{
		if (_bEndOfDialogue)
		{
			List<IDialogueListner> listListner = _mapDialogueListner[_strCurrentEvent];
			for (int i = 0; i < listListner.Count; i++)
				listListner[i].IDialogueListner_OnFinishDialogue();
			return;
		}

		_iCurDialogueIndex = Mathf.Clamp(iDialogueScene, 0, _listDialogueCurrent.Count - 1);
        _bEndOfDialogue = iDialogueScene == _listDialogueCurrent.Count - 1;

        if (_mapDialogueListner.ContainsKey_PrintOnError( _strCurrentEvent ) && _listDialogueCurrent.Count != 0)
        {
            List<IDialogueListner> listListner = _mapDialogueListner[_strCurrentEvent];
            for (int i = 0; i < listListner.Count; i++)
            {
                SDataTable_Dialogue pDataDialogue = _listDialogueCurrent[_iCurDialogueIndex];
                if (_iCurDialogueIndex == 0)
				{
					IDialogueListner pListener = listListner[i];
					if (pListener != null)
						pListener.IDialogueListner_OnStartDialogue(pDataDialogue);
					else
						Debug.LogError("왜 리스너가 없니...");
				}


                listListner[i].IDialogueListner_OnPlayDialogue(_listDialogueCurrent[_iCurDialogueIndex], _bEndOfDialogue );
            }
        }
    }

	/* protected - Override & Unity API         */

	protected override void OnMakeSingleton()
	{
		base.OnMakeSingleton();

		SCManagerParserJson pManagerParser = SCManagerParserJson.DoMakeInstance( null, const_strResourcesJsonDataPath, EResourcePath.Resources );
		pManagerParser.DoReadJson_And_InitEnumerator( const_strResourcesFileName, ref _mapDataTable_Dialogue );
	}

	// ========================================================================== //

	/* private - [Proc] Function             
       중요 로직을 처리                         */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산 등의 비교적 단순 로직         */

}