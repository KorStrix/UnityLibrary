using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrixLibrary_Example
{
    public class SimpleMoveAble_Example : MonoBehaviour
    {
        public float p_fMoveSpeed = 10f;

        Transform _pTransform;

        public void DoMove(Vector2 vecMoveWorldDireciton)
        {
            _pTransform.forward = Quaternion.Lerp(Quaternion.Euler(_pTransform.forward), Quaternion.Euler(vecMoveWorldDireciton), 0.5f).eulerAngles;
            _pTransform.Translate(vecMoveWorldDireciton * p_fMoveSpeed);
        }


        private void Awake()
        {
            _pTransform = GetComponent<Transform>();
        }
    }

}
