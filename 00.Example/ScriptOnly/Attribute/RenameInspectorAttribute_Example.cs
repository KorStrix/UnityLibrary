using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414

public class RenameInspectorAttribute_Example : MonoBehaviour {

    [Rename_Inspector("public iValue")]
    public int iValue = 1;

    [Rename_Inspector("private fValue")]
    [SerializeField]
    private float fValue = 0f;

    [SerializeField]
    [Rename_Inspector("private reaonlyString", false)]
    private string strValue = "It's a Readonly";

    [SerializeField]
    [Rename_Inspector("It's Looooooooooooooooooooooooooooooooooooooooooong", false)]
    private string strValue_Long = "It's Looooooooooooooooooooooooooooooooooooooooooong";
}
