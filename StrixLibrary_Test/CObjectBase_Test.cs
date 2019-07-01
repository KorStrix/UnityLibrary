using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CObjectBase_Test : CObjectBase
    {
        [GetComponent]
        [HideInInspector]
        public CObjectBase pGetComponent;

        [GetComponentInParent]
        [HideInInspector]
        public CObjectBase pGetComponentParents;

        [UnityTest]
        public IEnumerator Test_ObjectBase_GetComponent_Attribute()
        {
            GameObject pObjectNew = new GameObject();
            CObjectBase_Test pTarget = pObjectNew.AddComponent<CObjectBase_Test>();
            pTarget.EventOnAwake();

            yield return null;

            Assert.IsNotNull(pTarget.pGetComponent);
        }

        [UnityTest]
        public IEnumerator Test_ObjectBase_GetComponentInChildren_Attribute()
        {
            GameObject pObjectNew = new GameObject();
            CObjectBase_Test pTargetParents = pObjectNew.AddComponent<CObjectBase_Test>();

            CObjectBase_Test pTarget = pObjectNew.AddComponent<CObjectBase_Test>();
            pTarget.transform.SetParent(pTargetParents.transform);
            pTarget.EventOnAwake();

            yield return null;

            Assert.IsNotNull(pTarget.pGetComponentParents);
        }
    }
}
