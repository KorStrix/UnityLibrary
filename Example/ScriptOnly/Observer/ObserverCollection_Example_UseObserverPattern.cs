using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrixLibrary_Example
{
    public class ObserverCollection_Example_UseObserverPattern : MonoBehaviour
    {
        public struct OnChangeHP_Arg
        {
            public ObserverCollection_Example_UseObserverPattern pMessenger {get; private set;}
            public float fCurrentHP_0_1 { get; private set; }
            public bool bIsDamaged { get; private set; }
            public bool bIsDead { get; private set; }

            public OnChangeHP_Arg(ObserverCollection_Example_UseObserverPattern pMessenger, bool bIsDamaged, float fCurrentHP_0_1)
            {
                this.pMessenger = pMessenger;
                this.fCurrentHP_0_1 = fCurrentHP_0_1;
                this.bIsDamaged = bIsDamaged;
                this.bIsDead = fCurrentHP_0_1 == 0f;
            }
        }

        public ObservableCollection<OnChangeHP_Arg> p_Event_OnChangeHP { get; private set; } = new ObservableCollection<OnChangeHP_Arg>();

        public int p_iHP_Init = 10;

        [SerializeField]
        private int _iHP_Max;
        [SerializeField]
        private int _iHP_Current;

        public void Damaged(int iDamageAmount)
        {
            _iHP_Current -= iDamageAmount;
            if (_iHP_Current <= 0)
                _iHP_Current = 0;

            p_Event_OnChangeHP.DoNotify(new OnChangeHP_Arg(this, true, _iHP_Current / (float)_iHP_Max));
        }

        private void Awake()
        {
            _iHP_Max = p_iHP_Init;
        }

        private void OnEnable()
        {
            _iHP_Current = _iHP_Max;
            p_Event_OnChangeHP.DoNotify(new OnChangeHP_Arg(this, false, _iHP_Current / (float)_iHP_Max));
        }
    }
}
