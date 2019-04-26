#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
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
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

public interface IUIObject_HasButton<Enum_ButtonName>
{
    void IUIObject_HasButton_OnClickButton(Enum_ButtonName eButtonName, Button pButton);
}

public interface IUIObject_HasToggle<Enum_ToggleName>
{
    void IUIObject_HasToggle_OnToggle(Enum_ToggleName eToggleName, Toggle pToggle, bool bToggle);
}

public class CUIObjectBase : CObjectBase, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, /*IDragHandler,*/ IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    #region Field

    /* public - Field declaration            */

    public CObserverSubject<System.Action<string>> p_Event_OnClickButton { get; private set; } = new CObserverSubject<System.Action<string>>();

    /* protected - Field declaration         */

    /* private - Field declaration           */

    #endregion Field

    #region Public

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /* public - [Event] Function             
       프랜드 객체가 호출(For Friend class call)*/

    #endregion Public

    // ========================================================================== //

    #region Protected

    /* protected - [abstract & virtual]         */

    // ========================== [ Division ] ========================== //

    virtual protected void OnUIClick()
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log("OnUIClick");
    }

    virtual protected void OnUIPress( bool bPress )
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log("OnUIPress bPress : " + bPress);
    }

    virtual protected void OnUIHover( bool bHover )
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log("OnUIHover bHover : " + bHover);
    }

    virtual protected void OnUIDrag( bool bIsDrag )
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log("OnUIDrag bIsDrag : " + bIsDrag);
    }

    virtual protected void OnUIDrop( GameObject pObjectOnDrop )
    {
        if (CheckDebugFilter(EDebugFilter.Debug_Level_Core))
            Debug.Log("OnUIDrop pObjectOnDrop : " + pObjectOnDrop.name);
    }

    /* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

    /* protected - Override & Unity API         */

    protected override void OnAwake()
    {
        base.OnAwake();

        Init_HasButton();
        Init_HasToggle();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPress(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPress(false);
    }

    public void OnDrag(GameObject pObject)
    {
        OnUIDrag(true);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnUIDrag(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnUIDrop(eventData.pointerDrag);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnUIHover(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnUIHover(false);
    }


    // For NGUI
    public void OnClick() { OnUIClick(); }
	public void OnPress( bool bPress ) { OnUIPress( bPress ); }
	public void OnDragStart() { OnUIDrag( true ); }
	public void OnDragEnd() { OnUIDrag( false ); }
	public void OnDrop( GameObject pObjectDrop ){ OnUIDrop( pObjectDrop ); }
	public void OnDrag( PointerEventData eventData ) { OnUIDrag( true ); }
    #endregion Protected

    // ========================================================================== //

    #region Private

    /* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

    /* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

    protected void Init_HasButton()
    {
        System.Type pType_InterfaceHasButton = this.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IUIObject_HasButton<>)); ;
        if (pType_InterfaceHasButton != null)
        {
            System.Type pType_EnumButtonName = pType_InterfaceHasButton.GetGenericArguments()[0];
            var pMethod = pType_InterfaceHasButton.GetMethod("IUIObject_HasButton_OnClickButton");

            Button[] arrButton = GetComponentsInChildren<Button>(true);
            for (int i = 0; i < arrButton.Length; i++)
            {
                Button pButton = arrButton[i];

                bool bParseSuccess;
                object pEnum;
                Parsing_NameToEnum(pType_EnumButtonName, pButton, out bParseSuccess, out pEnum);

                if (bParseSuccess)
                {
                    UnityEngine.Events.UnityAction pButtonAction = delegate { pMethod.Invoke(this, new object[2] { pEnum, pButton }); };

                    if (CManagerCommand.instance != null)
                        CManagerCommand.instance.Event_RegistUIInpput_Button(this, pButton, pButtonAction);
                    else
                    {
                        pButton.onClick.RemoveListener(pButtonAction);
                        pButton.onClick.AddListener(pButtonAction);
                    }
                }
            }
        }
    }

    protected void Init_HasToggle()
    {
        System.Type pType_InterfaceHasToggle = this.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IUIObject_HasToggle<>)); ;
        if (pType_InterfaceHasToggle != null)
        {
            System.Type pType_EnumButtonName = pType_InterfaceHasToggle.GetGenericArguments()[0];
            var pMethod = pType_InterfaceHasToggle.GetMethod("IUIObject_HasToggle_OnToggle");

            Toggle[] arrToggle = GetComponentsInChildren<Toggle>(true);
            for (int i = 0; i < arrToggle.Length; i++)
            {
                Toggle pToggle = arrToggle[i];

                bool bParseSuccess = true;
                object pEnum = null;
                Parsing_NameToEnum(pType_EnumButtonName, pToggle, out bParseSuccess, out pEnum);

                if (bParseSuccess)
                {
                    UnityEngine.Events.UnityAction<bool> pButtonAction = delegate (bool bIsOn) {  pMethod.Invoke(this, new object[3] { pEnum, pToggle, bIsOn }); };
                    pToggle.onValueChanged.RemoveListener(pButtonAction);
                    pToggle.onValueChanged.AddListener(pButtonAction);
                }
            }
        }
    }

    private static void Parsing_NameToEnum(System.Type pType_EnumButtonName, Component pButton, out bool bParseSuccess, out object pEnum)
    {
        bParseSuccess = true;
        pEnum = null;
        if (pType_EnumButtonName.IsEnum)
        {
            try
            {
                pEnum = System.Enum.Parse(pType_EnumButtonName, pButton.name);
            }
            catch
            {
                bParseSuccess = false;
            }
        }
        else
            pEnum = pButton.name;
    }

    #endregion Private
}



#region Test
#if UNITY_EDITOR

[Category("StrixLibrary")]
public class UGUIPanel_Test : CUGUIPanelBase, IUIObject_HasButton<UGUIPanel_Test.EInput>
{
    public enum EInput
    {
        None, Button_Test, Button_Test2,
    }
    static EInput eLastInput;

    [UnityTest]
    public IEnumerator UGUIPanel_HasButtonTest()
    {
        EventSystem.current = new GameObject().AddComponent<EventSystem>();
        UGUIPanel_Test pTestPanel = new GameObject().AddComponent<UGUIPanel_Test>();
        Button pButtonTest = new GameObject(EInput.Button_Test.ToString()).AddComponent<Button>();
        Button pButtonTest2 = new GameObject(EInput.Button_Test2.ToString()).AddComponent<Button>();

        pButtonTest.transform.SetParent(pTestPanel.transform);
        pButtonTest2.transform.SetParent(pTestPanel.transform);
        pTestPanel.EventOnAwake_Force();

        eLastInput = EInput.None;
        Assert.AreEqual(eLastInput, EInput.None);

        if(CManagerCommand.instance != null)
        {
            //pButtonTest.OnPointerClick(new PointerEventData(EventSystem.current));
            //CManagerCommand.instance.OnUpdate();  // 통합 테스트를 하면 Command Manager가 생성되는데, Command Manager가 씬에 존재할 경우 UI Object Awake시 커맨드에 등록된다.
            //Assert.AreEqual(eLastInput, EInput.Button_Test);

            //pButtonTest2.OnPointerClick(new PointerEventData(EventSystem.current));
            //CManagerCommand.instance.OnUpdate();
            //Assert.AreEqual(eLastInput, EInput.Button_Test2);
        }
        else
        {
            pButtonTest.OnPointerClick(new PointerEventData(EventSystem.current));
            Assert.AreEqual(eLastInput, EInput.Button_Test);

            pButtonTest2.OnPointerClick(new PointerEventData(EventSystem.current));
            Assert.AreEqual(eLastInput, EInput.Button_Test2);
        }

        yield break;
    }

    public void IUIObject_HasButton_OnClickButton(EInput eButtonName, Button pButton)
    {
        eLastInput = eButtonName;
    }
}
#endif
#endregion