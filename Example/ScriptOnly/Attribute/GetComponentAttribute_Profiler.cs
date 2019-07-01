#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-18 오후 9:50:53
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace StrixLibrary_Example
{
    /// <summary>
    /// 
    /// </summary>
    public class GetComponentAttribute_Profiler : MonoBehaviour
    {
        public enum ETestCase
        {
            GetComponent_DefulatProperty,
            GetComponet_Function,
            GetComponetsInChildren_Function,

            GetComponet_Attribute_Individual,
            GetComponet_Attribute_All,
        }

        Transform GetComponent_Property;

        Transform GetComponent_Function;

        [GetComponent]
        Transform GetComponent_Attribute;

        Transform[] GetComponentsChildren_Children_Function;

        [GetComponentInChildren]
        Transform[] GetComponentsChildren_Children_Attribute;

        public int iTestCase = 10000;

        private void OnEnable()
        {
            MemberInfo pMemberInfo = null;
            #region Initialize
            MemberInfo[] arrMembers = GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < arrMembers.Length; i++)
            {
                if (arrMembers[i].Name.Equals(nameof(GetComponent_Attribute)))
                {
                    pMemberInfo = arrMembers[i];
                    break;
                }
            }

            GameObject pObjectChild = new GameObject("Child");
            pObjectChild.transform.parent = transform;
            #endregion Initialize


            string strTestCase = ETestCase.GetComponent_DefulatProperty.ToString();
            for(int i = 0; i < iTestCase; i++)
            {
                SCManagerProfiler.DoStartTestCase(strTestCase);
                GetComponent_Property = null;
                GetComponent_Property = transform;
                GetComponent_Property.GetType();
                SCManagerProfiler.DoFinishTestCase(strTestCase);
            }

            strTestCase = ETestCase.GetComponet_Function.ToString();
            for (int i = 0; i < iTestCase; i++)
            {
                SCManagerProfiler.DoStartTestCase(strTestCase);
                GetComponent_Function = null;
                GetComponent_Function = GetComponent<Transform>();
                GetComponent_Function.GetType();
                SCManagerProfiler.DoFinishTestCase(strTestCase);
            }

            strTestCase = ETestCase.GetComponetsInChildren_Function.ToString();
            for (int i = 0; i < iTestCase; i++)
            {
                SCManagerProfiler.DoStartTestCase(strTestCase);
                GetComponentsChildren_Children_Function = null;
                GetComponentsChildren_Children_Function = GetComponentsInChildren<Transform>();
                GetComponentsChildren_Children_Function[0].GetType();
                SCManagerProfiler.DoFinishTestCase(strTestCase);
            }

            strTestCase = ETestCase.GetComponet_Attribute_Individual.ToString();
            for (int i = 0; i < iTestCase; i++)
            {
                SCManagerProfiler.DoStartTestCase(strTestCase);
                GetComponent_Attribute = null;
                SCManagerGetComponent.DoUpdateGetComponentAttribute(this, this, pMemberInfo);
                GetComponent_Attribute.GetType();
                SCManagerProfiler.DoFinishTestCase(strTestCase);
            }

            strTestCase = ETestCase.GetComponet_Attribute_All.ToString();
            for (int i = 0; i < iTestCase; i++)
            {
                SCManagerProfiler.DoStartTestCase(strTestCase);
                GetComponentsChildren_Children_Attribute = null;
                SCManagerGetComponent.DoUpdateGetComponentAttribute(this);
                GetComponentsChildren_Children_Attribute[0].GetType();
                SCManagerProfiler.DoFinishTestCase(strTestCase);
            }

            SCManagerProfiler.DoPrintResult_PrintLog_IsError(true);
        }
    }
}

