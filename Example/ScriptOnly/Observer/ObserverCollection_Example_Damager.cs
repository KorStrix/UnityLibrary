using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrixLibrary_Example
{
    public class ObserverCollection_Example_Damager : MonoBehaviour
    {
        ObserverCollection_Example_Legacy p_Monster_A;
        ObserverCollection_Example_UseObserverPattern p_Monster_B;

        private void Awake()
        {
            p_Monster_A = GetComponentInChildren<ObserverCollection_Example_Legacy>();
            p_Monster_B = GetComponentInChildren<ObserverCollection_Example_UseObserverPattern>();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                p_Monster_A.Damaged(1);
                p_Monster_B.Damaged(1);
            }
        }
    }
}

