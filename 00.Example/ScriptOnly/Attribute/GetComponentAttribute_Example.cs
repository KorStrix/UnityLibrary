using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

#pragma warning disable 0414

public class GetComponentAttribute_Example : CObjectBase
{
    public enum ETestChildObject
    {
        TestObject_1,
        TestObject_2,
        TestObject_3,

        TestObject_Other_FindString,
        TestObject_Other_FindEnum,
    }


    [GetComponentInChildren]
    public List<Transform> p_listTest = new List<Transform>();

#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    [GetComponentInChildren]
    public Dictionary<string, Transform> p_mapTest_KeyIsString = new Dictionary<string, Transform>();

#if ODIN_INSPECTOR
    [ShowInInspector]
#endif
    [GetComponentInChildren]
    private Dictionary<ETestChildObject, Transform> p_mapTest_KeyIsEnum = new Dictionary<ETestChildObject, Transform>();

    [SerializeField]
    [GetComponentInChildren(nameof(ETestChildObject.TestObject_Other_FindString))]
    private Transform p_pChildComponent_FindString = null;

    [SerializeField]
    [GetComponentInChildren(ETestChildObject.TestObject_Other_FindEnum)]
    private Transform p_pChildComponent_FindEnum = null;

    [GetComponentInChildren]
    public Transform p_pChildComponent_FindEnumProperty { get; private set; }

    [SerializeField]
    [GetComponentInChildren]
    Transform[] arrComponent = null;
}
