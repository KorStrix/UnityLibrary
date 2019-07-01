using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace StrixLibrary_Test
{
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

            pButtonTest.OnPointerClick(new PointerEventData(EventSystem.current));
            Assert.AreEqual(eLastInput, EInput.Button_Test);

            pButtonTest2.OnPointerClick(new PointerEventData(EventSystem.current));
            Assert.AreEqual(eLastInput, EInput.Button_Test2);

            yield break;
        }

        public void IUIObject_HasButton_OnClickButton(EInput eButtonName, Button pButton)
        {
            eLastInput = eButtonName;
        }
    }
}
