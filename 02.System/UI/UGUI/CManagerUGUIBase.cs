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
using UnityEngine.UI;
using UnityEngine.EventSystems;

using NUnit.Framework;
using UnityEngine.TestTools;

[RequireComponent( typeof( CCompoEventSystemChecker ) )]
[RequireComponent( typeof( Canvas ) )]
[RequireComponent( typeof( CanvasScaler ) )]
[RequireComponent( typeof( GraphicRaycaster ) )]
abstract public class CManagerUGUIBase<Class_Instance, Enum_Panel_Name> : CManagerUIBase<Class_Instance, Enum_Panel_Name, CUGUIPanelBase, Button>
	where Class_Instance : CManagerUGUIBase<Class_Instance, Enum_Panel_Name>
	where Enum_Panel_Name : System.IFormattable, System.IConvertible, System.IComparable
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */
	
	#region Field

	/* public - Field declaration            */

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

	/* protected - [Event] Function           
       자식 객체가 호출(For Child class call)		*/

	/* protected - Override & Unity API         */

	#endregion Protected

	// ========================================================================== //

	#region Private

	/* private - [Proc] Function             
       로직을 처리(Process Local logic)           */

	/* private - Other[Find, Calculate] Func 
       찾기, 계산등 단순 로직(Simpe logic)         */

	#endregion Private
}

#region Test

[Category("StrixLibrary")]
public class CManagerUGUIBase_Test : CManagerUGUIBase<CManagerUGUIBase_Test, CManagerUGUIBase_Test.EUIPanel>
{
    const float const_fShowAnimation_DurationSec = 0.1f;

    public enum EUIPanel { None, 패널테스트_1, 패널테스트_2, }

    public class 패널테스트_1 : CUGUIPanelBase
    {
        public bool p_bIsPlaying_Show_Animation { get; private set; }
        protected override IEnumerator OnShowPanel_PlayingAnimation(int iSortOrder)
        {
            p_bIsPlaying_Show_Animation = true;
            yield return YieldManager.GetWaitForSecond(const_fShowAnimation_DurationSec);
            p_bIsPlaying_Show_Animation = false;
        }
    }
    public class 패널테스트_2 : CUGUIPanelBase { }

    [UnityTest]
    public IEnumerator ManagerPanel_Test()
    {
        GameObject pObjectManager = new GameObject();
        패널테스트_1 pPanelTest = new GameObject(typeof(패널테스트_1).ToString()).AddComponent<패널테스트_1>();
        패널테스트_2 pPanelTest2 = new GameObject(typeof(패널테스트_2).ToString()).AddComponent<패널테스트_2>();

        pPanelTest.transform.SetParent(pObjectManager.transform);
        pPanelTest2.transform.SetParent(pObjectManager.transform);

        CManagerUGUIBase_Test pManager = pObjectManager.AddComponent<CManagerUGUIBase_Test>();
        yield return null;

        Assert.AreEqual(pPanelTest.gameObject.activeSelf, true);
        Assert.AreEqual(pPanelTest2.gameObject.activeSelf, false);
        Assert.AreEqual(pPanelTest.transform.GetSiblingIndex(), pManager.transform.childCount - 1);

        Assert.AreEqual(pPanelTest.p_bIsPlaying_Show_Animation, true);
        yield return YieldManager.GetWaitForSecond(const_fShowAnimation_DurationSec); yield return null;
        Assert.AreEqual(pPanelTest.p_bIsPlaying_Show_Animation, false);

        pManager.DoShowHide_Panel(EUIPanel.패널테스트_1, false); yield return null;
        Assert.AreEqual(pPanelTest.gameObject.activeSelf, false);

        pManager.DoShowHide_Panel(EUIPanel.패널테스트_2, true); yield return null;
        Assert.AreEqual(pPanelTest2.gameObject.activeSelf, true);
        Assert.AreEqual(pPanelTest2.transform.GetSiblingIndex(), pManager.transform.childCount - 1);
    }

    protected override void OnDefaultPanelShow() { DoShowHide_Panel(EUIPanel.패널테스트_1, true); }
}

#endregion