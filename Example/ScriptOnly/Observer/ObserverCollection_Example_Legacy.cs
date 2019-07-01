using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrixLibrary_Example
{
    public class ObserverCollection_Example_Legacy : MonoBehaviour
    {
        public Image p_Image_HP;

        public int p_iHP_Init = 10;

        [SerializeField]
        private int _iHP_Max;
        [SerializeField]
        private int _iHP_Current;

        public void Damaged(int iDamageAmount)
        {
            _iHP_Current -= iDamageAmount;
            if(_iHP_Current <= 0)
            {
                _iHP_Current = 0;
                Debug.Log(name + " Play On Dead Effect - Legacy");
            }
            else
            {
                Debug.Log(name + " Play On Damage Effect - Legacy");
            }

            OnChange_HP(_iHP_Current / (float)_iHP_Max);
        }

        private void Awake()
        {
            _iHP_Max = p_iHP_Init;
        }

        private void OnEnable()
        {
            _iHP_Current = _iHP_Max;
            OnChange_HP(_iHP_Current / (float)_iHP_Max);
        }

        private void OnChange_HP(float fHeath_0_1)
        {
            p_Image_HP.fillAmount = fHeath_0_1;
        }
    }
}
