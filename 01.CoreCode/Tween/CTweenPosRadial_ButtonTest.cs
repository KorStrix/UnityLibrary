using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTweenPosRadial_ButtonTest : MonoBehaviour, CTweenPosition_Radial.ITweenPosRadial_Listener
{
    public void ITweenPosRadial_Listener_OnStartTween(int iChildIndex)
    {
        Debug.Log(name + iChildIndex, this);
    }
}
