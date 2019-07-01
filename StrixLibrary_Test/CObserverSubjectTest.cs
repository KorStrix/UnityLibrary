using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CObserverSubject_Test
    {
        int _TestValue;

        /// <summary>
        /// 테스트 목표 :  Listener는 중복이 되지 않습니다.
        ///</summary>
        [Test]
        public void IsNotOverlap_Observer()
        {
            ObservableCollection pObserverSubject = new ObservableCollection();
            _TestValue = 0;

            pObserverSubject.DoRegist_Listener(AddField_1); // _TestValue  = 1
            pObserverSubject.DoRegist_Listener(AddField_1); // 등록 되있으면 _TestValue = 2, 등록이 안되었기 때문에 _TestValue = 1

            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoNotify();
            Assert.IsTrue(_TestValue == 1);
        }

        /// <summary>
        /// 테스트 목표 : 옵저버에는 Listener가 여러개 들어갑니다.
        ///</summary>
        [Test]
        public void HasMultipleObserver()
        {
            ObservableCollection pObserverSubject = new ObservableCollection();
            _TestValue = 0;

            pObserverSubject.DoRegist_Listener(AddField_1); // _TestValue = 1
            pObserverSubject.Subscribe += AddField_2;       // _TestValue = 1 + 2 = 3

            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoNotify();
            Assert.IsTrue(_TestValue == 3);
        }

        /// <summary>
        /// 테스트 목표 : 옵저버 인자로 제네릭을 사용할 수 있습니다.
        ///</summary>
        [Test]
        public void HasGenericParams()
        {
            ObservableCollection<int> pObserverSubject = new ObservableCollection<int>();
            _TestValue = 0;

            pObserverSubject.DoRegist_Listener(AddField_HasParam);
            pObserverSubject.DoRegist_Listener(AddField_HasParam); // Listener는 중복되지 않습니다.

            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoNotify(5);
            Assert.IsTrue(_TestValue == 5);
        }

        /// <summary>
        /// 테스트 목표 : 제네릭 옵저버도 Listener가 여러개 들어갑니다.
        ///</summary>
        [Test]
        public void HasMultipleObserver_And_GenericParams()
        {
            ObservableCollection<int> pObserverSubject = new ObservableCollection<int>();
            _TestValue = 0;

            pObserverSubject.DoRegist_Listener(AddField_HasParam);
            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoNotify(5);
            Assert.IsTrue(_TestValue == 5);

            pObserverSubject.DoRegist_Listener(MinusField_HasParam);
            pObserverSubject.DoNotify(6);
            Assert.IsTrue(_TestValue == 5); // Add 6, Minus 6을 했기 때문에 원본값 그대로입니다.

            pObserverSubject.DoRemove_Listener(AddField_HasParam);

            pObserverSubject.DoNotify(5);
            Assert.IsTrue(_TestValue == 0);
        }

        /// <summary>
        /// 테스트 목표 : 리스너에 등록하자마자 마지막에 갱신된 데이터를 받을 수 있습니다.
        ///</summary>
        [Test]
        public void Regist_And_InstantNotify()
        {
            ObservableCollection<int> pObserverSubject = new ObservableCollection<int>();

            _TestValue = 0;
            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoNotify(5);
            Assert.IsTrue(_TestValue == 0); // 등록되있는 Listener가 없기 때문에 변동이 없습니다.

            pObserverSubject.DoRegist_Listener(AddField_HasParam, true); // 뒤늦게 요청했을 때, 2번째 인자가 true면 최신값을 받을 수 있습니다.
            Assert.IsTrue(_TestValue == 5); // 최신값을 받았기 때문에 값이 변했습니다.

            pObserverSubject.DoRemove_Listener(AddField_HasParam); // 이때 Add를 지웠기 때문에 이제 리스너가 없어 TestValue는 변하지 않습니다.
            pObserverSubject.DoNotify(6);
            Assert.IsTrue(_TestValue == 5);

            pObserverSubject.DoRegist_Listener(AddField_HasParam); // 뒤늦게 요청했을 때 2번째 인자를 안넣으면 값은 갱신되지 않습니다.
            Assert.IsTrue(_TestValue == 5);

            pObserverSubject.DoNotify(-5); // AddField Listener가 하나 존재하기 때문에, 값은 0이 됩니다.
            Assert.IsTrue(_TestValue == 0);
        }

        public struct Test_Args
        {
            public int iValue;
            public bool bValue;

            public Test_Args(int iValue, bool bValue)
            {
                this.iValue = iValue;
                this.bValue = bValue;
            }
        }


        /// <summary>
        /// 테스트 목표 : 값이 변했을 때만 알리는 기능이 있습니다.
        ///</summary>
        [Test]
        public void Choice_OnChangeOnly_Or_Normal__OnNotify()
        {
            ObservableCollection<Test_Args> pObserverSubject = new ObservableCollection<Test_Args>();

            _TestValue = 0;
            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoRegist_Listener(AddField_HasParam_WhenApplyOnly, true); // 두번째 인자가 True일때만 계산되는 이벤트를 리스너로 등록합니다.
            pObserverSubject.DoNotify(new Test_Args(5, false)); // 두번째 인자가 False이기 때문에 적용되지 않습니다.

            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoNotify(new Test_Args(5, true)); // 두번째 인자가 True이기 때문에 적용됩니다.
            Assert.IsTrue(_TestValue == 5);
        }

        /// <summary>
        /// 파라미터 정의를 위한 상속 클래스.
        /// </summary>
        public class ObserverSubject_DefineParameter : ObservableCollection<int>
        {
            public delegate void OnTest(int iValueDefine);

            public void DoRegist_Listener_Define(OnTest OnTest)
            {
                DoRegist_Listener(new System.Action<int>(OnTest));
            }

            public void DoNotify_Define(int iValueDefine)
            {
                DoNotify(iValueDefine);
            }
        }


        /// <summary>
        /// 테스트 목표 : 옵저버를 상속받아서 사용할 수 있습니다.
        ///</summary>
        [Test]
        public void DefineDelegateParameter()
        {
            ObserverSubject_DefineParameter pObserverSubject = new ObserverSubject_DefineParameter();
            _TestValue = 0;

            pObserverSubject.DoRegist_Listener_Define(AddField_HasParam);
            Assert.IsTrue(_TestValue == 0);

            pObserverSubject.DoNotify_Define(5);
            Assert.IsTrue(_TestValue == 5);
        }

        // =========================================================================================

        private void AddField_1()
        {
            _TestValue += 1;
        }

        private void AddField_2()
        {
            _TestValue += 2;
        }


        /// <summary>
        /// Apply 일때만 필드에 파라미터만큼 더합니다.
        /// </summary>
        /// <param name="iAddValue"></param>
        private void AddField_HasParam_WhenApplyOnly(Test_Args pArgs)
        {
            if (pArgs.bValue)
                _TestValue += pArgs.iValue;
        }

        /// <summary>
        /// 필드에 파라미터만큼 더합니다.
        /// </summary>
        /// <param name="iAddValue"></param>
        private void AddField_HasParam(int iAddValue)
        {
            _TestValue += iAddValue;
        }

        /// <summary>
        /// 필드에 파라미터만큼 뺍니다.
        /// </summary>
        /// <param name="iMinusValue"></param>
        private void MinusField_HasParam(int iMinusValue)
        {
            _TestValue -= iMinusValue;
        }
    }
}
