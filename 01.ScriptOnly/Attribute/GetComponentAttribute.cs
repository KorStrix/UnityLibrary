#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	작성자 : Strix
 *	
 *	기능 : 
 *	오리지널 소스코드의 경우 에디터 - Inspector에서 봐야만 갱신이 되었는데,
 *	현재는 SCManagerGetComponent.DoUpdateGetComponentAttribute 를 호출하면 갱신하도록 변경하였습니다.
 *	Awake에서 호출하시면 됩니다.
 *	
 *	Private 변수는 갱신되지 않습니다.
 *	
 *	오리지널 소스 링크
 *	https://openlevel.postype.com/post/683269
   ============================================ */
#endregion Header

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

static public class Extension_MemberInfo
{
    static public System.Type MemberType(this MemberInfo pMemberInfo)
    {
        FieldInfo pFieldInfo = pMemberInfo as FieldInfo;
        if (pFieldInfo != null)
            return pFieldInfo.FieldType;

        PropertyInfo pProperty = pMemberInfo as PropertyInfo;
        if(pProperty != null)
            return pProperty.PropertyType;

        return null;
    }

    static public bool CheckValueIsNull(this MemberInfo pMemberInfo, object pTarget)
    {
        FieldInfo pFieldInfo = pMemberInfo as FieldInfo;
        if (pFieldInfo == null)
            return true;

        object pObjectValue = pFieldInfo.GetValue(pTarget);
        if(pObjectValue == null)
            return true;

        bool bResult;
        UnityEngine.Object pUnityObject = pObjectValue as UnityEngine.Object;
        if (pObjectValue is UnityEngine.Object)
            bResult = pUnityObject.Equals(null);
        else
            bResult = pObjectValue.Equals(null);
        return bResult;
    }

    static public void SetValue_Extension(this MemberInfo pMemberInfo, object pTarget, object pValue)
    {
        FieldInfo pFieldInfo = pMemberInfo as FieldInfo;
        if (pFieldInfo != null)
            pFieldInfo.SetValue(pTarget, pValue);

        PropertyInfo pProperty = pMemberInfo as PropertyInfo;
        if (pProperty != null)
            pProperty.SetValue(pTarget, pValue, null);
    }

    static public object GetValue_Extension(this MemberInfo pMemberInfo, object pTarget)
    {
        FieldInfo pFieldInfo = pMemberInfo as FieldInfo;
        if (pFieldInfo != null)
            return pFieldInfo.GetValue(pTarget);

        PropertyInfo pProperty = pMemberInfo as PropertyInfo;
        if (pProperty != null)
            return pProperty.GetValue(pTarget);

        return null;
    }
}

public interface IGetComponentAttribute
{
    object GetComponent(MonoBehaviour pTargetMono, Type pElementType);
    bool bIsPrint_OnNotFound_GetComponent { get; }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public abstract class GetComponentAttributeBase : UnityEngine.PropertyAttribute, IGetComponentAttribute
{
    public bool bIsPrint_OnNotFound_GetComponent => bIsPrint_OnNotFound;
    public bool bIsPrint_OnNotFound;

    public abstract object GetComponent(MonoBehaviour pTargetMono, Type pElementType);
}

public class GetComponentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        return SCManagerGetComponent.Event_GetComponent(pTargetMono, pElementType);
    }
}

public class GetComponentInChildrenAttribute : GetComponentAttributeBase
{
    public bool bSearch_By_ComponentName = false;
    public bool bInclude_DeActive = false;
    public string strComponentName;

    public GetComponentInChildrenAttribute(bool bInclude_DeActive = true)
    {
        bSearch_By_ComponentName = false;
        this.bInclude_DeActive = bInclude_DeActive;
    }

    public GetComponentInChildrenAttribute(bool bInclude_DeActive, bool bIsPrint_OnNotFound = true)
    {
        this.bInclude_DeActive = bInclude_DeActive;
        this.bSearch_By_ComponentName = false;
        this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
    }

    public GetComponentInChildrenAttribute(System.Object pComponentName)
    {
        this.strComponentName = pComponentName.ToString();
        this.bSearch_By_ComponentName = true;
    }

    public GetComponentInChildrenAttribute(System.Object pComponentName, bool bInclude_DeActive)
    {
        this.strComponentName = pComponentName.ToString();
        this.bSearch_By_ComponentName = true;
        this.bInclude_DeActive = bInclude_DeActive;
    }

    public GetComponentInChildrenAttribute(System.Object pComponentName, bool bInclude_DeActive, bool bIsPrint_OnNotFound = true)
    {
        this.bInclude_DeActive = bInclude_DeActive;
        this.strComponentName = pComponentName.ToString();
        this.bSearch_By_ComponentName = true;
        this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
    }

    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        return SCManagerGetComponent.Event_GetComponentInChildren(pTargetMono, pElementType, bInclude_DeActive, bSearch_By_ComponentName, strComponentName);
    }
}

public class GetComponentInParentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        return SCManagerGetComponent.Event_GetComponentInParents(pTargetMono, pElementType);
    }
}


static public class SCManagerGetComponent
{
    static public UnityEngine.Object GetComponentInChildren_SameName(this Component pTarget, string strObjectName, System.Type pComponentType, bool bInclude_DeActive)
    {
        List<UnityEngine.Object> listComponent = GetComponentsInChildrenList_SameName(pTarget, strObjectName, pComponentType, bInclude_DeActive);
        if (listComponent.Count > 0)
            return listComponent[0];
        else
            return null;
    }

    static public UnityEngine.Object[] GetComponentsInChildren_SameName(this Component pTarget, string strObjectName, System.Type pComponentType, bool bInclude_DeActive)
    {
        return GetComponentsInChildrenList_SameName(pTarget, strObjectName, pComponentType, bInclude_DeActive).ToArray();
    }

    static public List<UnityEngine.Object> GetComponentsInChildrenList_SameName(this Component pTarget, string strObjectName, System.Type pComponentType, bool bInclude_DeActive)
    {
        Component[] arrComponentFind = null;
        if (pComponentType == typeof(GameObject))
            arrComponentFind = pTarget.transform.GetComponentsInChildren(typeof(Transform), true);
        else
            arrComponentFind = pTarget.transform.GetComponentsInChildren(pComponentType, bInclude_DeActive);

        return ExtractSameNameList(strObjectName, arrComponentFind);
    }

    static public List<UnityEngine.Object> ExtractSameNameList(string strObjectName, UnityEngine.Object[] arrComponentFind)
    {
        List<UnityEngine.Object> listReturn = new List<UnityEngine.Object>();
        if (arrComponentFind != null)
        {
            for (int i = 0; i < arrComponentFind.Length; i++)
            {
                if (arrComponentFind[i].name.Equals(strObjectName))
                    listReturn.Add(arrComponentFind[i]);
            }
        }

        return listReturn;
    }

    static public void DoUpdateGetComponentAttribute(MonoBehaviour pTarget)
    {
        DoUpdateGetComponentAttribute(pTarget, pTarget);
    }

    static public void DoUpdateGetComponentAttribute(MonoBehaviour pMonobehaviourOwner, object pClass_Anything)
    {
        // BindingFloags를 일일이 써야 잘 동작한다..
        System.Type pType = pClass_Anything.GetType();
        MemberInfo[] arrMembers = pType.GetMembers(BindingFlags.Public | BindingFlags.Instance);
        UpdateComponentAttribute(pMonobehaviourOwner, pClass_Anything, arrMembers);

        arrMembers = pType.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance);
        UpdateComponentAttribute(pMonobehaviourOwner, pClass_Anything, arrMembers);
    }

    static private void UpdateComponentAttribute(MonoBehaviour pTargetMono, object pFieldOwner, MemberInfo[] arrMember)
    {
        for (int i = 0; i < arrMember.Length; i++)
            DoUpdateGetComponentAttribute(pTargetMono, pFieldOwner, arrMember[i]);
    }

    static public void DoUpdateGetComponentAttribute(MonoBehaviour pTargetMono, object pMemberOwner, MemberInfo pMemberInfo)
    {
        object[] arrCustomAttributes = pMemberInfo.GetCustomAttributes(true);
        for (int i = 0; i < arrCustomAttributes.Length; i++)
        {
            IGetComponentAttribute pGetcomponentAttribute = arrCustomAttributes[i] as IGetComponentAttribute;
            if (pGetcomponentAttribute == null)
                continue;

            System.Type pTypeMember = pMemberInfo.MemberType();
            object pComponent = null;

            if (pTypeMember.IsGenericType)
                pComponent = SetMember_OnGeneric(pGetcomponentAttribute, pTargetMono, pMemberOwner, pMemberInfo, pTypeMember);
            else if (pTypeMember.HasElementType)
                pComponent = pGetcomponentAttribute.GetComponent(pTargetMono, pTypeMember.GetElementType());
            else
                pComponent = pGetcomponentAttribute.GetComponent(pTargetMono, pTypeMember);

            if (pComponent == null)
            {
                if (pGetcomponentAttribute.bIsPrint_OnNotFound_GetComponent)
                {
                    GetComponentInChildrenAttribute pAttribute = pGetcomponentAttribute as GetComponentInChildrenAttribute;
                    if (pAttribute != null && pAttribute.bSearch_By_ComponentName)
                        Debug.LogError(pTargetMono.name + string.Format(".{0}<{1}>({2}) Result == null", pGetcomponentAttribute.GetType().Name, pTypeMember, pAttribute.strComponentName), pTargetMono);
                    else
                        Debug.LogError(pTargetMono.name + string.Format(".{0}<{1}> Result == null", pGetcomponentAttribute.GetType().Name, pTypeMember), pTargetMono);
                }

                continue;
            }

            if (pTypeMember.IsGenericType == false)
            {
                if (pTypeMember.HasElementType == false)
                {
                    Array arrComponent = pComponent as Array;
                    if (arrComponent != null && arrComponent.Length != 0)
                        pMemberInfo.SetValue_Extension(pMemberOwner, arrComponent.GetValue(0));
                }
                else
                {
                    if (pTypeMember == typeof(GameObject))
                        pMemberInfo.SetValue_Extension(pMemberOwner, ((Component)pComponent).gameObject);
                    else
                        pMemberInfo.SetValue_Extension(pMemberOwner, pComponent);
                }
            }
        }
    }

    // ====================================================================================================================

    static public object Event_GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
                 .GetMethod("GetComponents", new Type[0])
                 .MakeGenericMethod(pElementType);

        return getter.Invoke(pTargetMono, null);
    }

    static public object Event_GetComponentInChildren(MonoBehaviour pTargetMono, Type pElementType, bool bInclude_DeActive, bool bSearch_By_ComponentName, string strComponentName)
    {
        object pObjectReturn;

        MethodInfo getter = typeof(MonoBehaviour)
                    .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
                    .MakeGenericMethod(pElementType);

        if (pElementType == typeof(GameObject))
        {
            getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(typeof(Transform));

            pObjectReturn = Convert_TransformArray_To_GameObjectArray(pTargetMono, getter.Invoke(pTargetMono, new object[] { bInclude_DeActive }));
        }
        else
            pObjectReturn = getter.Invoke(pTargetMono, new object[] { bInclude_DeActive });

        if (bSearch_By_ComponentName)
            return ExtractSameNameList(strComponentName, pObjectReturn as UnityEngine.Object[]).ToArray();
        else
            return pObjectReturn;
    }

    static public object Event_GetComponentInParents(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
          .GetMethod("GetComponentsInParent", new Type[] { })
          .MakeGenericMethod(pElementType);

        if (pElementType == typeof(GameObject))
        {
            getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInParent", new Type[] { })
            .MakeGenericMethod(typeof(Transform));

            return Convert_TransformArray_To_GameObjectArray(pTargetMono,
                getter.Invoke(pTargetMono, new object[] { }));
        }

        return getter.Invoke(pTargetMono, new object[] { });
    }

    // ====================================================================================================================

    static private object SetMember_OnGeneric(IGetComponentAttribute pGetComponentAttribute, MonoBehaviour pTargetMono, object pMemberOwner, MemberInfo pMember, System.Type pTypeField)
    {
        object pComponent = null;
        System.Type pTypeField_Generic = pTypeField.GetGenericTypeDefinition();
        Type[] arrArgumentsType = pTypeField.GetGenericArguments();

        if (pTypeField_Generic == typeof(List<>))
            pComponent = SetMember_OnList(pGetComponentAttribute, pTargetMono, pMemberOwner, pMember, pTypeField, arrArgumentsType);
        else if (pTypeField_Generic == typeof(Dictionary<,>))
            pComponent = SetMember_OnDictionary(pGetComponentAttribute, pTargetMono, pMemberOwner, pMember, pTypeField, arrArgumentsType[0], arrArgumentsType[1]);

        return pComponent;
    }

    static private object SetMember_OnList(IGetComponentAttribute pGetComponentAttribute, MonoBehaviour pTargetMono, object pMemberOwner, MemberInfo pMember, Type pTypeField, Type[] arrArgumentsType)
    {
        object pComponent = pGetComponentAttribute.GetComponent(pTargetMono, arrArgumentsType[0]);
        Array arrComponent = pComponent as Array;
        var Method_Add = pTypeField.GetMethod("Add");
        var pInstanceList = System.Activator.CreateInstance(pTypeField);

        for (int i = 0; i < arrComponent.Length; i++)
            Method_Add.Invoke(pInstanceList, new object[] { arrComponent.GetValue(i) });

        pMember.SetValue_Extension(pMemberOwner, pInstanceList);
        return pComponent;
    }

    static private object SetMember_OnDictionary(IGetComponentAttribute pAttributeInChildren, MonoBehaviour pTargetMono, object pMemberOwner, MemberInfo pMember, System.Type pTypeField, Type pType_DictionaryKey, Type pType_DictionaryValue)
    {
        object pComponent = pAttributeInChildren.GetComponent(pTargetMono, pType_DictionaryValue);
        Array arrComponent = pComponent as Array;

        if (arrComponent == null || arrComponent.Length == 0)
        {
            return null;
        }

        var Method_Add = pTypeField.GetMethod("Add", new[] {
                                pType_DictionaryKey, pType_DictionaryValue });

        var pInstanceDictionary = System.Activator.CreateInstance(pTypeField);
        bool bIsDrived_DictionaryItem = CheckIsDERIVED_DictionaryItem(pType_DictionaryValue.GetInterfaces(), typeof(IDictionaryItem<>).Name);

        if (pType_DictionaryKey == typeof(string))
        {
            for (int i = 0; i < arrComponent.Length; i++)
            {
                UnityEngine.Object pComponentChild = arrComponent.GetValue(i) as UnityEngine.Object;

                try
                {
                    Method_Add.Invoke(pInstanceDictionary, new object[] {
                                pComponentChild.name,
                                pComponentChild });
                }
                catch
                {
                    Debug.LogError(pComponentChild.name + " Get Compeont - Dictionary Add - Overlap Key MonoType : " + pTargetMono.GetType() + "/Member : " + pMember.Name, pTargetMono);
                }
            }

        }
        else if (bIsDrived_DictionaryItem)
        {
            var pMethod_GetKey = pType_DictionaryValue.GetMethod("IDictionaryItem_GetKey");
            for (int i = 0; i < arrComponent.Length; i++)
            {
                UnityEngine.Object pComponentChild = arrComponent.GetValue(i) as UnityEngine.Object;
                Method_Add.Invoke(pInstanceDictionary, new object[] {
                                    pMethod_GetKey.Invoke(pComponentChild, null),
                                    pComponentChild });
            }
        }
        else if (pType_DictionaryKey.IsEnum)
        {
            for (int i = 0; i < arrComponent.Length; i++)
            {
                try
                {
                    UnityEngine.Object pComponentChild = arrComponent.GetValue(i) as UnityEngine.Object;
                    var pEnum = System.Enum.Parse(pType_DictionaryKey, pComponentChild.name, true);
                    Method_Add.Invoke(pInstanceDictionary, new object[] {
                                    pEnum,
                                    pComponentChild });
                }
                catch { }
            }
        }

        pMember.SetValue_Extension(pMemberOwner, pInstanceDictionary);
        return pComponent;
    }

    static private bool CheckIsDERIVED_DictionaryItem(Type[] arrInterfaces, string strTypeName)
    {
        bool bIsDerived_DictionaryItem = false;
        for (int i = 0; i < arrInterfaces.Length; i++)
        {
            if (arrInterfaces[i].Name.Equals(strTypeName))
            {
                bIsDerived_DictionaryItem = true;
                break;
            }
        }

        return bIsDerived_DictionaryItem;
    }

    static private GameObject[] Convert_TransformArray_To_GameObjectArray(MonoBehaviour pTargetMono, object pObject)
    {
        object[] arrObject = pObject as object[];
        GameObject[] arrObjectReturn = new GameObject[arrObject.Length];
        for (int i = 0; i < arrObject.Length; i++)
            arrObjectReturn[i] = (arrObject[i] as Transform).gameObject;
        return arrObjectReturn;
    }
}