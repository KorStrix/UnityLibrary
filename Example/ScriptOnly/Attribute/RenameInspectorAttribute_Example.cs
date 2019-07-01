using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414

public class RenameInspectorAttribute_Example : MonoBehaviour {

    [DisplayName("public iValue")]
    public int iValue = 1;

    [DisplayName("private fValue")]
    [SerializeField]
    private float fValue = 0f;

    [SerializeField]
    [DisplayName("private reaonlyString", false)]
    private string strValue = "It's a Readonly";

    [SerializeField]
    [DisplayName("It's Looooooooooooooooooooooooooooooooooooooooooong", false)]
    private string strValue_Long = "It's Looooooooooooooooooooooooooooooooooooooooooong";
}
