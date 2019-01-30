using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckVisible_IVisibleListener_Test : MonoBehaviour, IVisibleListener
{
    public void IVisibleListener_OnVisible(bool bIsVisible)
    {
        GetComponent<Renderer>().material.color = bIsVisible ? Color.cyan : Color.magenta;

        // Debug.Log(name + " IVisibleListener_OnVisible : " + bIsVisible, this);
    }
}
