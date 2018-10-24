#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public interface IDropDownInitializer
{
	void IDropDownInitializer_Regist_DropDownItem( CUGUIDropdownItem pItem );
}

[RequireComponent(typeof(GraphicRaycaster))]
abstract public class CUGUIPanelHasInputBase<Enum_InputName> : CUGUIPanelBase, IDropDownInitializer
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    #region Field

    /* public - Field declaration            */

    /* protected - Field declaration         */

    protected Dictionary<Enum_InputName, Button> _mapButton;

    /* private - Field declaration           */

    private List<CUGUIScrollItem> _listScrollView = new List<CUGUIScrollItem>();
    private Dictionary<string, Toggle> _mapToggle;

    #endregion Field

    #region Public

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoEnableButtons(bool bEnable)
    {
        List<Button> listButton = _mapButton.Values.ToList();
        for (int i = 0; i < listButton.Count; i++)
            listButton[i].enabled = bEnable;
    }

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    public void IDropDownInitializer_Regist_DropDownItem(CUGUIDropdownItem pItem)
    {
        Dropdown pDropDown = pItem.GetComponentInParent<Dropdown>();
        Enum_InputName eDropDownName;
        if (pDropDown.name.ConvertEnum(out eDropDownName))
            pItem.DoInitItem(eDropDownName.GetHashCode(), EventOnPointerEnter);
    }

    public void EventOnPointerEnter(int iOwnerID, CUGUIDropDown.SDropDownData pData, string strText)
    {
        OnDropDown_HoverItem((Enum_InputName)(object)iOwnerID, pData, strText);
    }

    public void EventOnChangeDropDown(Enum_InputName eDropDownName, CUGUIDropDown pDropDown)
    {
        string strText = pDropDown.options[pDropDown.value].text;
        OnDropDown_SelectItem(eDropDownName, pDropDown.GetData(strText), strText);
    }

    public void EventInitScrollView<Data_ScrollItem>(List<Data_ScrollItem> listDataScrollItem)
        where Data_ScrollItem : IUGUIScrollItemData
    {
        listDataScrollItem.Sort(ComparerScrollItem);
        for (int i = 0; i < _listScrollView.Count && i < listDataScrollItem.Count; i++)
            _listScrollView[i].EventSetScrollData(listDataScrollItem[i]);
    }

    #endregion Public

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    abstract public void OnButtons_Click(Enum_InputName eButtonName);
    protected virtual void OnToggles_Click(Enum_InputName eToggle, bool bIsOn) { }
    protected virtual void OnInputFields_ValueChanged(Enum_InputName eToggle, string strInput) { }
    protected virtual void OnInputFields_Submit(Enum_InputName eToggle, string strInput) { }

    virtual public void OnButtons_Press_And_Hold(Enum_InputName eButtonName, bool bPress) { }
    virtual public void OnScrollView_ClickItem(CUGUIScrollItem pScrollItem, IUGUIScrollItemData pScrollData, Enum_InputName eButtonName) { }
    virtual public void OnDropDown_SelectItem(Enum_InputName eDropDownName, CUGUIDropDown.SDropDownData pData, string strItemText) { }
    virtual public void OnDropDown_HoverItem(Enum_InputName eDropDownName, CUGUIDropDown.SDropDownData pData, string strItemText) { }
    virtual public void OnSlider_SetValue(Enum_InputName eSliderName, float fSliderValue_0_1) { }

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	protected void OnClick_Buttons_Wrapper( Enum_InputName eButtonName, Button pButton ) { OnButtons_Click( eButtonName ); }

	/* protected - Override & Unity API         */

	protected override void OnAwake()
	{
		base.OnAwake();

		Button[] arrButton = GetComponentsInChildren<Button>(true);
		for (int i = 0; i < arrButton.Length; i++)
		{
			Button pButton = arrButton[i];
			string strButtonName = pButton.name;

			Enum_InputName eButtonName;
			if (strButtonName.ConvertEnum( out eButtonName ))
			{
                pButton.onClick.AddListener(() => { OnClick_Buttons_Wrapper(eButtonName, pButton); });
				if (_mapButton == null)
					_mapButton = new Dictionary<Enum_InputName, Button>();

				if(_mapButton.ContainsKey(eButtonName))
				{
					Debug.LogWarning( name + "Already Button Exist A - " + strButtonName, _mapButton[eButtonName] );
					Debug.LogWarning( name + "Already Button Exist B - " + strButtonName, pButton );
					continue;
				}

				_mapButton.Add(eButtonName, pButton);

				CUGUIButton_Press pButtonPress = pButton.GetComponent<CUGUIButton_Press>();
				if (pButtonPress != null)
				{
					pButtonPress.p_Event_OnPress_Down.AddListener( delegate { OnButtons_Press_And_Hold( eButtonName, true ); } );
					pButtonPress.p_Event_OnPress_Up.AddListener( delegate { OnButtons_Press_And_Hold( eButtonName, false ); } );
				}
			}
		}

		Toggle[] arrToggle = GetComponentsInChildren<Toggle>(true);
		int iLen = arrToggle.Length;
		for (int i = 0; i < iLen; i++)
		{
			Toggle pToggle = arrToggle[i];
			Enum_InputName eToggleName;
			string strToggleName = pToggle.name;

			if (strToggleName.ConvertEnum(out eToggleName))
			{
				pToggle.onValueChanged.AddListener((bool bIsOn) => { OnToggles_Click(eToggleName, bIsOn); });

				if (_mapToggle == null)
					_mapToggle = new Dictionary<string, Toggle>();

				if (_mapToggle.ContainsKey(strToggleName) == false)
					_mapToggle.Add(strToggleName, pToggle);
			}
		}

		CUGUIDropDown[] arrDropDown = GetComponentsInChildren<CUGUIDropDown>(true);
		for (int i = 0; i < arrDropDown.Length; i++)
		{
			CUGUIDropDown pDropDown = arrDropDown[i];
			Enum_InputName eDropDownName;
			if (pDropDown.name.ConvertEnum( out eDropDownName ))
				pDropDown.onValueChanged.AddListener( delegate { EventOnChangeDropDown( eDropDownName, pDropDown ); } );
		}

		GetComponentsInChildren( true, _listScrollView );
		for (int i = 0; i < _listScrollView.Count; i++)
			_listScrollView[i].EventInitScrollItem<Enum_InputName>( OnScrollView_ClickItem );

		InputField[] arrInputField = GetComponentsInChildren<InputField>(true);
		for (int i = 0; i < arrInputField.Length; i++)
		{
			InputField pInputField = arrInputField[i];
			Enum_InputName eInputField_Name;
			if (pInputField.name.ConvertEnum( out eInputField_Name ))
			{
				pInputField.onValueChanged.AddListener( ( string strInput ) => { OnInputFields_ValueChanged( eInputField_Name, strInput ); } );
				pInputField.onEndEdit.AddListener( ( string strInput ) => { OnInputFields_Submit( eInputField_Name, strInput ); } );
			}

		}

        Slider[] arrSlider = GetComponentsInChildren<Slider>();
        for(int i = 0; i < arrSlider.Length; i++)
        {
            var pInput = arrSlider[i];
            Enum_InputName eInputField_Name;
            if (pInput.name.ConvertEnum(out eInputField_Name))
            {
                pInput.onValueChanged.AddListener((float fValue) => OnSlider_SetValue(eInputField_Name, pInput.value));
            }
        }
	}

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */


	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	static private int ComparerScrollItem<Data_ScrollItem>( Data_ScrollItem pDataX, Data_ScrollItem pDataY )
	where Data_ScrollItem : IUGUIScrollItemData
	{
		int iSortOrderX = pDataX.IScrollData_GetSortOrder();
		int iSortOrderY = pDataY.IScrollData_GetSortOrder();

		if (iSortOrderX < iSortOrderY)
			return -1;
		else if (iSortOrderX > iSortOrderY)
			return 1;
		else
			return 0;
	}
	#endregion Private
}

#region Test
#if UNITY_EDITOR

[Category("StrixLibrary")]
public class UGUI_인풋패널_테스트 : CUGUIPanelHasInputBase<UGUI_인풋패널_테스트.EInput>
{
    public enum EInput
    {
        None, Button_Test, Button_Test2,
    }
    static EInput eLastInput;

    [UnityTest]
    public IEnumerator 인풋입력테스트()
    {
        EventSystem.current = new GameObject().AddComponent<EventSystem>();
        UGUI_인풋패널_테스트 pTestPanel = new GameObject().AddComponent<UGUI_인풋패널_테스트>();
        Button pButtonTest = new GameObject(EInput.Button_Test.ToString()).AddComponent<Button>();
        Button pButtonTest2 = new GameObject(EInput.Button_Test2.ToString()).AddComponent<Button>();

        pButtonTest.transform.SetParent(pTestPanel.transform);
        pButtonTest2.transform.SetParent(pTestPanel.transform);
        pTestPanel.EventOnAwake_Force();

        eLastInput = EInput.None;
        Assert.AreEqual(eLastInput, EInput.None);

        pButtonTest.OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.AreEqual(eLastInput, EInput.Button_Test);

        pButtonTest2.OnPointerClick(new PointerEventData(EventSystem.current));
        Assert.AreEqual(eLastInput, EInput.Button_Test2);

        yield break;
    }

    public override void OnButtons_Click(EInput eButtonName) { eLastInput = eButtonName; }
}
#endif
#endregion