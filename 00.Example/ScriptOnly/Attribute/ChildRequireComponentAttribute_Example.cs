using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414

public class ChildRequireComponentAttribute_Example : CObjectBase {

    public enum ETestChildObject
    {
        TestObject_1,
        TestObject_2,
        TestObject_3,

        TestObject_Other_FindString,
        TestObject_Other_FindEnum,
    }

    [SerializeField]
    [ChildRequireComponent(nameof(ETestChildObject.TestObject_Other_FindString))]
    private Transform p_pChildComponent_FindString = null;

    [SerializeField]
    [Rename_Inspector("FindEnum")]
    [ChildRequireComponent(ETestChildObject.TestObject_Other_FindEnum)]
    private Transform p_pChildComponent_FindEnum = null;
}
