using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CObserverSubject_Chain_Test
    {
        int _iTestValue;

        [Test]
        public void IsNotOverlap_Observer()
        {
            ObservableCollection_ChainData<int> pObserverSubject = new ObservableCollection_ChainData<int>();
            _iTestValue = 0;

            pObserverSubject.DoRegist_Listener(Plus_To_TestValue, 0);
            pObserverSubject.DoRegist_Listener(Plus_To_TestValue, 1); // Not Regist

            Assert.IsTrue(_iTestValue == 0);

            pObserverSubject.DoNotify(123);
            Assert.IsTrue(_iTestValue == 123);
        }

        private void Plus_To_TestValue(int pValue_Origin, ref int pValue_Current)
        {
            _iTestValue += pValue_Origin;
        }

        // ===========================================================

        [Test]
        public void Regist_And_Remove()
        {
            ObservableCollection_ChainData<int> pObserverSubject = new ObservableCollection_ChainData<int>();
            _iTestValue = 0;

            pObserverSubject.DoRegist_Listener(Plus_To_TestValue, 0);
            pObserverSubject.DoRegist_Listener(Minus_To_TestValue, 1);

            pObserverSubject.DoRemove_Listener(Plus_To_TestValue); // Remove Plus & Minus Only

            Assert.IsTrue(_iTestValue == 0);

            pObserverSubject.DoNotify(10); // Minus Only
            Assert.IsTrue(_iTestValue == -10);
        }

        private void Minus_To_TestValue(int pValue_Origin, ref int pValue_Current)
        {
            _iTestValue -= pValue_Origin;
        }

        // ===========================================================

        [Test]
        public void HasMultipleObserver_And_MustCall_In_Order()
        {
            ObservableCollection_ChainData<int> pObserverSubject = new ObservableCollection_ChainData<int>();
            _iTestValue = 0;

            pObserverSubject.DoRegist_Listener(Plus_Current_To_TestValue, 2);
            pObserverSubject.DoRegist_Listener(Plus_10_Current, 0);
            pObserverSubject.DoRegist_Listener(Minus_20_Current, 1);

            Assert.IsTrue(_iTestValue == 0);

            pObserverSubject.DoNotify_ForDebug(100); // Result : 0 + (100 + 10 - 20)
            Assert.IsTrue(_iTestValue == (100 + 10 - 20));
        }

        private void Plus_10_Current(int pValue_Origin, ref int pValue_Current)
        {
            pValue_Current += 10;
        }

        private void Minus_20_Current(int pValue_Origin, ref int pValue_Current)
        {
            pValue_Current -= 20;
        }

        private void Plus_Current_To_TestValue(int pValue_Origin, ref int pValue_Current)
        {
            _iTestValue += pValue_Current;
        }

        // ===========================================================

        [Test]
        public void HasMultipleObserver_And_GenericParams()
        {
            ObservableCollection_ChainData<string, int> pObserverSubject = new ObservableCollection_ChainData<string, int>();
            _iTestValue = 0;

            pObserverSubject.DoRegist_Listener(AddField_HasParam, 1);
            Assert.IsTrue(_iTestValue == 0);

            pObserverSubject.DoNotify("테스트_A", 5);
            Assert.IsTrue(_iTestValue == 5);

            pObserverSubject.DoRegist_Listener(PrintLog_HasParam, 2);
            pObserverSubject.DoNotify("테스트_B", 8);
            Assert.IsTrue(_iTestValue == 5 + 8);

            pObserverSubject.DoRegist_Listener(Decrease_20_CurrentValue_HasParam, 0);

            pObserverSubject.DoNotify("테스트_C", 10);
            Assert.IsTrue(_iTestValue == 5 + 8 + (10 - 20));
        }

        private void AddField_HasParam(string arg1, int pValue_Origin, ref int pValue_Current)
        {
            _iTestValue += pValue_Current;
        }

        private void Decrease_20_CurrentValue_HasParam(string arg1, int pValue_Origin, ref int pValue_Current)
        {
            pValue_Current -= 20;
        }

        private void PrintLog_HasParam(string arg1, int pValue_Origin, ref int pValue_Current)
        {
            Debug.Log("PrintLog_HasParam - arg1 : " + arg1 + " pValue_Origin : " + pValue_Origin + " pValue_Current : " + pValue_Current);
        }
    }
}
