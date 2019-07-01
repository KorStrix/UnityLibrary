using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrixLibrary_Example
{
    public class Test_Mono_Update : MonoBehaviour
    {
        public Example_CManagerUpdateObject pManagerObject;

        void Update()
        {
            pManagerObject.TestCase(this);
        }
    }

    public class Test_UpdateAbleObject : MonoBehaviour, IUpdateAble
    {
        public Example_CManagerUpdateObject pManagerObject;

        private void OnEnable()
        {
            CManagerUpdateObject.instance.DoAddObject(this);
        }

        private void OnDestroy()
        {
            CManagerUpdateObject.instance.DoRemoveObject(this);
        }

        public void OnUpdate(float fTimeScale_Individual)
        {
            pManagerObject.TestCase(this);
        }

        public void IUpdateAble_GetUpdateInfo(ref bool bIsUpdate_Default_IsFalse, ref float fTimeScale_Invidiaul_Default_IsOne)
        {
            bIsUpdate_Default_IsFalse = gameObject.activeSelf;
        }
    }

    public class Example_CManagerUpdateObject : MonoBehaviour
    {
        List<GameObject> _listMonobehaviour = new List<GameObject>();
        List<GameObject> _listUpdateAble = new List<GameObject>();

        public bool bPrintLog = true;
        public int iTestObjectCount = 1000;

        public void TestCase(MonoBehaviour pTestObject)
        {
            if (bPrintLog)
                Debug.Log(pTestObject.name + " Working..", pTestObject);

            int j = 0;
            for (int i = 0; i < 100; i++)
                j += i;
        }

        private void Awake()
        {
            string strObjectName_MonoUpdate = typeof(Test_Mono_Update).Name;
            string strObjectName_IUpdateAble = typeof(Test_UpdateAbleObject).Name;
            for (int i = 0; i < iTestObjectCount; i++)
            {
                GameObject pObject_Mono = new GameObject(strObjectName_MonoUpdate + "_" + (i + 1));
                pObject_Mono.AddComponent<Test_Mono_Update>().pManagerObject = this;
                pObject_Mono.transform.SetParent(transform);
                _listMonobehaviour.Add(pObject_Mono);

                GameObject pObject_IUpdateAble = new GameObject(strObjectName_IUpdateAble + "_" + (i + 1));
                pObject_IUpdateAble.AddComponent<Test_UpdateAbleObject>().pManagerObject = this;
                pObject_IUpdateAble.transform.SetParent(transform);
                _listUpdateAble.Add(pObject_IUpdateAble);
            }

            StartCoroutine(CoUpdate_Enable_DisableTest());
        }

        IEnumerator CoUpdate_Enable_DisableTest()
        {
            bool bEnable = false;
            while (true)
            {
                yield return YieldManager.GetWaitForSecond(1f);

                for (int i = 0; i < _listMonobehaviour.Count; i++)
                    _listMonobehaviour[i].SetActive(bEnable);

                yield return YieldManager.GetWaitForSecond(1f);

                for (int i = 0; i < _listUpdateAble.Count; i++)
                    _listUpdateAble[i].SetActive(bEnable);

                bEnable = !bEnable;
            }
        }
    }
}