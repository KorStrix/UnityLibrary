#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/strix13/UnityLibrary
 *	============================================
 *	작성자 : Strix
 *	작성일 : 2018-03-18 오전 9:14:00
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CExtension_Mono
{
    static public void SetLayer_Recursive(this Component pObj, int iLayer)
    {
        pObj.gameObject.layer = iLayer;
        Transform pTransformTarget = pObj.transform;
        int iChildCount = pTransformTarget.childCount;
        for (int i = 0; i < iChildCount; i++)
            pTransformTarget.GetChild(i).SetLayer_Recursive(iLayer);
    }

    static public void SetActive(this Component pObj, bool bEnable)
    {
        if (pObj == null)
        {
            Debug.Log(" SetActive - Component == null");
            return;
        }

        pObj.gameObject.SetActive(bEnable);
    }

    static public GameObject GetGameObject<TObjectName>(this Component pTarget, TObjectName tGameObjName, bool bPrintError = true)
    {
        if (tGameObjName == null)
            return null;

        string strGameObjName = tGameObjName.ToString();
        GameObject pFindGameObj = null;

        Transform[] arrCompo = pTarget.GetComponentsInChildrenOnly<Transform>();
        for (int i = 0; i < arrCompo.Length; i++)
        {
            if (arrCompo[i].name.Equals(strGameObjName))
            {
                pFindGameObj = arrCompo[i].gameObject;
                break;
            }
        }

        if (pFindGameObj == null && bPrintError)
            Debug.LogWarning(string.Format("{0}에 {1}이 없다", pTarget.name, strGameObjName), pTarget);

        return pFindGameObj;
    }

    static public bool GetComponent<COMPONENT>(this Component pTarget, out COMPONENT pComponent)
    where COMPONENT : UnityEngine.Component
    {
        pComponent = pTarget.GetComponent<COMPONENT>();

        return pComponent != null;
    }

    static public bool GetComponentInChildren<COMPONENT>(this Component pTarget, string strObjectName, out COMPONENT pComponent)
    where COMPONENT : UnityEngine.Component
    {
        GameObject pObjectFind = pTarget.GetGameObject(strObjectName, false);
        if (pObjectFind != null)
            pComponent = pObjectFind.GetComponent<COMPONENT>();
        else
            pComponent = null;

        return pComponent != null;
    }

    static public bool GetComponentInChildren<COMPONENT>(this Component pTarget, out COMPONENT pComponent)
        where COMPONENT : UnityEngine.Component
    {
        pComponent = pTarget.GetComponentInChildren<COMPONENT>(true);

        return pComponent != null;
    }

    private enum EKeyType
    {
        None,
        String,
        Enum,
        CustomKey,
    }


    static public Dictionary<KEY, COMPONENT> GetComponentInChildren<KEY, COMPONENT>(this Component pTarget)
        where COMPONENT : UnityEngine.Component
    {
        Dictionary<KEY, COMPONENT> mapInitTarget = new Dictionary<KEY, COMPONENT>();
        System.Type pType = typeof(KEY);
        EKeyType eKeyType = EKeyType.None;
        if (pType.Equals(typeof(string)))
            eKeyType = EKeyType.String;
        else if (pType.IsEnum)
            eKeyType = EKeyType.Enum;

        if (eKeyType == EKeyType.None)
        {
            Debug.LogWarning(pTarget.name + " GetComponentInChildren_InitEnumerator eKeyType == EKeyType.None", pTarget);
            return null;
        }

        COMPONENT[] arrComponent = pTarget.GetComponentsInChildren<COMPONENT>(true);
        for (int i = 0; i < arrComponent.Length; i++)
        {
            KEY Key = default(KEY);
            switch (eKeyType)
            {
                case EKeyType.CustomKey: break;
                case EKeyType.String: Key = (KEY)(object)arrComponent[i].name; break;
                case EKeyType.Enum:
                    try
                    {
                        Key = (KEY)System.Enum.Parse(typeof(KEY), arrComponent[i].name);
                    }
                    catch
                    {
                        Debug.LogWarning(pTarget.name + " GetComponentInChildren_InitEnumerator Enum Parsing Error - " + arrComponent[i].name, pTarget);
                        continue;
                    }
                    break;
            }
            mapInitTarget.Add(Key, arrComponent[i]);
        }

        return mapInitTarget;
    }

    static public COMPONENT GetComponentInChildrenOnly<COMPONENT>(this Component pTarget)
        where COMPONENT : UnityEngine.Component
    {
        COMPONENT pFindCompo = null;
        COMPONENT[] arrChildrenCompo = pTarget.GetComponentsInChildren<COMPONENT>();
        for (int i = 0; i < arrChildrenCompo.Length; i++)
        {
            if (arrChildrenCompo[i].transform != pTarget.transform)
            {
                pFindCompo = arrChildrenCompo[i];
                break;
            }
        }

        return pFindCompo;
    }

    static public COMPONENT[] GetComponentsInChildrenOnly<COMPONENT>(this Component pTarget)
        where COMPONENT : UnityEngine.Component
    {
        List<COMPONENT> listComponentChildrenOnly = new List<COMPONENT>();
        COMPONENT[] arrChildrenCompo = pTarget.GetComponentsInChildren<COMPONENT>(true);
        for (int i = 0; i < arrChildrenCompo.Length; i++)
        {
            if (arrChildrenCompo[i].transform != pTarget.transform)
                listComponentChildrenOnly.Add(arrChildrenCompo[i]);
        }

        return listComponentChildrenOnly.ToArray();
    }
    
    static public COMPONENT GetComponentInChildren_Cashed<COMPONENT>(this Component pTarget, Dictionary<string, COMPONENT> mapUIElements, string strUIElement, bool bIsPrintError = true)
        where COMPONENT : Component
    {
        if (mapUIElements == null)
            mapUIElements = new Dictionary<string, COMPONENT>();

        if (mapUIElements.ContainsKey(strUIElement))
            return mapUIElements[strUIElement];

        COMPONENT[] arrUIElements = pTarget.GetComponentsInChildren<COMPONENT>(true);
        for (int i = 0; i < arrUIElements.Length; i++)
        {
            COMPONENT pUIElement = arrUIElements[i];
            string strElementName = pUIElement.name;

            if (strElementName.Equals(strUIElement))
            {
                if (mapUIElements.ContainsKey(strElementName) == false)
                    mapUIElements.Add(strUIElement, pUIElement);
                else
                    Debug.LogError(strElementName + " 키 값이 중복되었습니다.", pTarget);

                return pUIElement;
            }
        }

        if (bIsPrintError)
            Debug.LogError(pTarget.name + " - Find UI Element : " + strUIElement + " 를 찾을 수 없습니다... ", pTarget);

        return null;
    }
}
