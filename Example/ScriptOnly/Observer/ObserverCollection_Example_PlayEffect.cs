using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrixLibrary_Example
{
    public class ObserverCollection_Example_PlayEffect : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<ObserverCollection_Example_UseObserverPattern>().p_Event_OnChangeHP.Subscribe += OnChange_HP;
        }

        private void OnChange_HP(ObserverCollection_Example_UseObserverPattern.OnChangeHP_Arg pArg)
        {
            if (pArg.bIsDamaged == false)
                return;

            if (pArg.bIsDead)
                Debug.Log(pArg.pMessenger.name + " Play On Dead Effect - Use Observer");
            else
                Debug.Log(pArg.pMessenger.name + " Play On Damage Effect - Use Observer");
        }
    }
}
