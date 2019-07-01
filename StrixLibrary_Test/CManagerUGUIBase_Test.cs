using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
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

}
