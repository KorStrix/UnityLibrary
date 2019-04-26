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

using NUnit.Framework;
using UnityEngine.TestTools;

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

    protected GameObject[] Convert_TransformArray_To_GameObjectArray(MonoBehaviour pTargetMono, object pObject)
    {
        object[] arrObject = pObject as object[];
        GameObject[] arrObjectReturn = new GameObject[arrObject.Length];
        for (int i = 0; i < arrObject.Length; i++)
            arrObjectReturn[i] = (arrObject[i] as Transform).gameObject;
        return arrObjectReturn;
    }
}

public class GetComponentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
                 .GetMethod("GetComponents", new Type[0])
                 .MakeGenericMethod(pElementType);

        return getter.Invoke(pTargetMono, null);
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
        object pObjectReturn;

        MethodInfo getter = typeof(MonoBehaviour)
                    .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
                    .MakeGenericMethod(pElementType);

        if (pElementType == typeof(GameObject))
        {
            getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(typeof(Transform));

            pObjectReturn = Convert_TransformArray_To_GameObjectArray(pTargetMono, getter.Invoke(pTargetMono, new object[] { this.bInclude_DeActive }));
        }
        else
            pObjectReturn = getter.Invoke(pTargetMono, new object[] { this.bInclude_DeActive });

        if (bSearch_By_ComponentName)
            return  SCManagerGetComponent.ExtractSameNameList(strComponentName, pObjectReturn as UnityEngine.Object[]).ToArray();
        else
            return pObjectReturn;
    }
}

public class GetComponentInParentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
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
        // BindingFloags를 일일이 써야 잘 동작한다..
        System.Type pType = pTarget.GetType();
        MemberInfo[] arrMembers = pType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        //arrMembers = pType.GetFields(BindingFlags.Public | BindingFlags.Static);
        //UpdateComponentAttribute(pTarget, arrMembers);

        //arrMembers = pType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
        //UpdateComponentAttribute(pTarget, arrMembers);

        // Property 구간
        arrMembers = pType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        //arrMembers = pType.GetProperties(BindingFlags.Public | BindingFlags.Static);
        //UpdateComponentAttribute(pTarget, arrMembers);

        //arrMembers = pType.GetProperties(BindingFlags.NonPublic | BindingFlags.Static);
        //UpdateComponentAttribute(pTarget, arrMembers);
    }

    static private void UpdateComponentAttribute(MonoBehaviour pTargetMono, MemberInfo[] arrMember)
    {
        for (int i = 0; i < arrMember.Length; i++)
        {
            MemberInfo pMemberInfo = arrMember[i];
            object[] arrCustomAttributes = pMemberInfo.GetCustomAttributes(true);

            for (int j = 0; j < arrCustomAttributes.Length; j++)
            {
                IGetComponentAttribute pGetcomponentAttribute = arrCustomAttributes[j] as IGetComponentAttribute;
                if (pGetcomponentAttribute == null)
                    continue;

                System.Type pTypeField = pMemberInfo.MemberType();
                object pComponent = null;

                if (pTypeField.IsGenericType)
                    pComponent = SetMember_OnGeneric(pGetcomponentAttribute, pTargetMono, pMemberInfo, pTypeField);
                else if (pTypeField.HasElementType)
                    pComponent = pGetcomponentAttribute.GetComponent(pTargetMono, pTypeField.GetElementType());
                else
                    pComponent = pGetcomponentAttribute.GetComponent(pTargetMono, pTypeField);
                
                if (pComponent == null)
                {
                    if(pGetcomponentAttribute.bIsPrint_OnNotFound_GetComponent)
                    {
                        GetComponentInChildrenAttribute pAttribute = pGetcomponentAttribute as GetComponentInChildrenAttribute;
                        if (pAttribute != null && pAttribute.bSearch_By_ComponentName)
                            Debug.LogError(pTargetMono.name + string.Format(".{0}<{1}>({2}) Result == null", pGetcomponentAttribute.GetType().Name, pTypeField, pAttribute.strComponentName), pTargetMono);
                        else
                            Debug.LogError(pTargetMono.name + string.Format(".{0}<{1}> Result == null", pGetcomponentAttribute.GetType().Name, pTypeField), pTargetMono);
                    }

                    continue;
                }

                if (pTypeField.IsGenericType == false)
                {
                    if(pTypeField.HasElementType == false)
                    {
                        Array arrComponent = pComponent as Array;
                        if (arrComponent != null && arrComponent.Length != 0)
                            pMemberInfo.SetValue_Extension(pTargetMono, arrComponent.GetValue(0));
                    }
                    else
                    {
                        if (pTypeField == typeof(GameObject))
                            pMemberInfo.SetValue_Extension(pTargetMono, ((Component)pComponent).gameObject);
                        else
                            pMemberInfo.SetValue_Extension(pTargetMono, pComponent);
                    }
                }
            }
        }
    }

    static private object SetMember_OnGeneric(IGetComponentAttribute pGetComponentAttribute, MonoBehaviour pTargetMono, MemberInfo pMember, System.Type pTypeField)
    {
        object pComponent = null;
        System.Type pTypeField_Generic = pTypeField.GetGenericTypeDefinition();
        Type[] arrArgumentsType = pTypeField.GetGenericArguments();

        if (pTypeField_Generic == typeof(List<>))
            pComponent = SetMember_OnList(pGetComponentAttribute, pTargetMono, pMember, pTypeField, arrArgumentsType);
        else if (pTypeField_Generic == typeof(Dictionary<,>))
            pComponent = SetMember_OnDictionary(pGetComponentAttribute, pTargetMono, pMember, pTypeField, arrArgumentsType[0], arrArgumentsType[1]);

        return pComponent;
    }

    static private object SetMember_OnList(IGetComponentAttribute pGetComponentAttribute, MonoBehaviour pTargetMono, MemberInfo pMember, Type pTypeField, Type[] arrArgumentsType)
    {
        object pComponent = pGetComponentAttribute.GetComponent(pTargetMono, arrArgumentsType[0]);
        Array arrComponent = pComponent as Array;
        var Method_Add = pTypeField.GetMethod("Add");
        var pInstanceList = System.Activator.CreateInstance(pTypeField);

        for (int i = 0; i < arrComponent.Length; i++)
            Method_Add.Invoke(pInstanceList, new object[] { arrComponent.GetValue(i) });

        pMember.SetValue_Extension(pTargetMono, pInstanceList);
        return pComponent;
    }

    static private object SetMember_OnDictionary(IGetComponentAttribute pAttributeInChildren, MonoBehaviour pTargetMono, MemberInfo pMember, System.Type pTypeField, Type pType_DictionaryKey, Type pType_DictionaryValue)
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
        bool bIsDrived_DictionaryItem = CheckIsDriven_DictionaryItem(pType_DictionaryValue.GetInterfaces(), typeof(IDictionaryItem<>).Name);

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
        else
        {
            if (bIsDrived_DictionaryItem)
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
            else
            {
                if (pType_DictionaryKey.IsEnum)
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
            }
        }

        pMember.SetValue_Extension(pTargetMono, pInstanceDictionary);
        return pComponent;
    }

    private static bool CheckIsDriven_DictionaryItem(Type[] arrInterfaces, string strTypeName)
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
}

#region Test

public class Test_ComponentParents : MonoBehaviour { }
public class Test_ComponentChild : MonoBehaviour { }
public class Test_ComponentOnly : MonoBehaviour { }
public class Test_ComponentChild_DerivedDictionaryItem : MonoBehaviour, IDictionaryItem<GetComponentAttribute_Test.ETestChildObject>
{
    public GetComponentAttribute_Test.ETestChildObject IDictionaryItem_GetKey()
    {
        return name.ConvertEnum<GetComponentAttribute_Test.ETestChildObject>();
    }
}

[Category("StrixLibrary")]
public class GetComponentAttribute_Test : MonoBehaviour
{
    public enum ETestChildObject
    {
        TestObject_1,
        TestObject_2,
        TestObject_3,

        TestObject_Other_FindString,
        TestObject_Other_FindEnum,

        MAX,
    }


    [GetComponentInParent]
    public Test_ComponentParents p_pParents = null;

    [GetComponentInChildren]
    public List<Test_ComponentChild> p_listTest = new List<Test_ComponentChild>();

    [GetComponentInChildren]
    public Dictionary<string, Test_ComponentChild> p_mapTest_KeyIsString = new Dictionary<string, Test_ComponentChild>();
    [GetComponentInChildren]
    private Dictionary<ETestChildObject, Test_ComponentChild> p_mapTest_KeyIsEnum = new Dictionary<ETestChildObject, Test_ComponentChild>();

    [GetComponentInChildren(nameof(ETestChildObject.TestObject_Other_FindString))]
    private Test_ComponentChild p_pChildComponent_FindString = null;
    [GetComponentInChildren(ETestChildObject.TestObject_Other_FindEnum)]
    private Test_ComponentChild p_pChildComponent_FindEnum = null;

    [GetComponentInChildren(nameof(ETestChildObject.TestObject_Other_FindString))]
    private GameObject p_pObject_FindString = null;
    [GetComponentInChildren(ETestChildObject.TestObject_Other_FindEnum)]
    private GameObject p_pObject_FindEnum = null;


    [GetComponentInChildren]
    public Test_ComponentChild p_pChildComponent_FindEnumProperty { get; private set; }

    [GetComponent]
    Test_ComponentOnly[] arrComponent = null;

    [GetComponentInChildren]
    GameObject[] arrObject_Children = null;

    public void Awake()
    {
        SCManagerGetComponent.DoUpdateGetComponentAttribute(this);
    }

    [Test]
    static public void GetComponentIn_Parent()
    {
        GameObject pObjectRoot = new GameObject("Root");
        pObjectRoot.AddComponent<Test_ComponentParents>();

        GameObject pObjectParents = new GameObject(nameof(GetComponentIn_Parent));
        pObjectParents.transform.SetParent(pObjectRoot.transform);

        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();
        Assert.NotNull(pParents.p_pParents);
    }

    [Test]
    static public void GetComponentChildren_Field_Test()
    {
        GameObject pObjectParents = new GameObject(nameof(GetComponentChildren_Field_Test));

        // GetComponent 대상인 자식 추가
        int iChildCount = (int)ETestChildObject.MAX;
        for (int i = 0; i < iChildCount; i++)
        {
            GameObject pObjectChild = new GameObject(((ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild>();
        }

        // 자식을 전부 추가한 뒤에 페런츠에 추가한다.
        // 추가하자마자 Awake로 자식을 찾기 때문
        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();

        // Getcomponent Attribute가 잘 작동했는지 체크 시작!!
        Assert.NotNull(pParents.p_pChildComponent_FindEnum);
        Assert.NotNull(pParents.p_pChildComponent_FindString);

        Assert.NotNull(pParents.p_pObject_FindString);
        Assert.NotNull(pParents.p_pObject_FindEnum);

        Assert.AreEqual(pParents.p_pChildComponent_FindString.name, ETestChildObject.TestObject_Other_FindString.ToString());
        Assert.AreEqual(pParents.p_pChildComponent_FindEnum.name, ETestChildObject.TestObject_Other_FindEnum.ToString());

        Assert.AreEqual(pParents.p_pObject_FindString.name, ETestChildObject.TestObject_Other_FindString.ToString());
        Assert.AreEqual(pParents.p_pObject_FindEnum.name, ETestChildObject.TestObject_Other_FindEnum.ToString());

        Assert.AreEqual(pParents.p_listTest.Count, iChildCount);

        Assert.AreEqual(pParents.p_mapTest_KeyIsEnum.Count, iChildCount);
        Assert.AreEqual(pParents.p_mapTest_KeyIsString.Count, iChildCount);
        Assert.AreEqual(pParents.arrObject_Children.Length, pObjectParents.transform.childCount + 1); // 자기 자신까지 추가하기떄문에 마지막에 + 1을 한다.

        var pIterString = pParents.p_mapTest_KeyIsString.GetEnumerator();
        while (pIterString.MoveNext())
            Assert.IsTrue(pIterString.Current.Key == pIterString.Current.Value.name.ToString());

        var pIterEnum = pParents.p_mapTest_KeyIsEnum.GetEnumerator();
        while (pIterEnum.MoveNext())
            Assert.IsTrue(pIterEnum.Current.Key.ToString() == pIterEnum.Current.Value.name.ToString());
    }

    [Test]
    static public void GetComponent_Child_Enum_Test()
    {
        GameObject pObjectParents = new GameObject(nameof(GetComponent_Child_Enum_Test));

        // GetComponent 대상인 자식 추가
        for (int i = 0; i < (int)ETestChildObject.MAX; i++)
        {
            GameObject pObjectChild = new GameObject(((ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild_DerivedDictionaryItem>();
        }

        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();

        var pIterEnum = pParents.p_mapTest_KeyIsEnum.GetEnumerator();
        while (pIterEnum.MoveNext())
            Assert.IsTrue(pIterEnum.Current.Key.ToString() == pIterEnum.Current.Value.name.ToString());
    }

    [Test]
    static public void GetComponentChildren_Property_Test()
    {
        GameObject pObjectParents = new GameObject(nameof(GetComponentChildren_Property_Test));

        // GetComponent 대상인 자식 추가
        for (int i = 0; i < (int)ETestChildObject.MAX; i++)
        {
            GameObject pObjectChild = new GameObject(((ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild>();
        }

        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();
        Assert.IsNotNull(pParents.p_pChildComponent_FindEnumProperty);
    }

    [Test]
    static public void GetComponent_Array_Test()
    {
        GameObject pObjectParents = new GameObject(nameof(GetComponent_Array_Test));

        // GetComponent 대상인 자식 추가
        int iAddComponentCount = 3;
        for (int i = 0; i < iAddComponentCount; i++)
            pObjectParents.AddComponent<Test_ComponentOnly>();

        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();
        Assert.AreEqual(pParents.arrComponent.Length, iAddComponentCount);
    }
}

#endregion