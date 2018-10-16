using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private void OnDisable()
    {
        CManagerUpdateObject.instance.DoRemoveObject(this);
    }

    public void OnUpdate(ref bool bCheckUpdateCount)
    {
        bCheckUpdateCount = true;
        pManagerObject.TestCase(this);
    }
}

public class Example_CManagerUpdateObject : MonoBehaviour
{
    public bool bPrintLog = true;
    public int iTestObjectCount = 1000;

    public void TestCase(MonoBehaviour pTestObject)
    {
        if(bPrintLog)
            Debug.Log(pTestObject.name + " Working..", pTestObject);

        int j = 0;
        for(int i = 0; i < 100; i++)
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


            GameObject pObject_IUpdateAble = new GameObject(strObjectName_IUpdateAble + "_" + (i + 1));
            pObject_IUpdateAble.AddComponent<Test_UpdateAbleObject>().pManagerObject = this;
            pObject_IUpdateAble.transform.SetParent(transform);
        }
    }
}