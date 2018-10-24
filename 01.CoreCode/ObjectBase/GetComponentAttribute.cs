#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
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
            return false;

        return pFieldInfo.GetValue(pTarget) == null;
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

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public abstract class GetComponentAttributeBase : UnityEngine.PropertyAttribute
{
    public bool bIsPrint_OnNotFound;

    public abstract object GetComponent(MonoBehaviour pTargetMono, Type pElementType);
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
        bSearch_By_ComponentName = false;
        this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
    }

    public GetComponentInChildrenAttribute(System.Object pObject)
    {
        bSearch_By_ComponentName = true;
        this.strComponentName = pObject.ToString();
        this.bSearch_By_ComponentName = true;
    }
    public GetComponentInChildrenAttribute(System.Object pObject, bool bInclude_DeActive)
    {
        bSearch_By_ComponentName = true;
        this.strComponentName = pObject.ToString();
        this.bSearch_By_ComponentName = true;
        this.bInclude_DeActive = bInclude_DeActive;
    }

    public GetComponentInChildrenAttribute(System.Object pObject, bool bInclude_DeActive, bool bIsPrint_OnNotFound = true)
    {
        bSearch_By_ComponentName = true;
        this.bInclude_DeActive = bInclude_DeActive;
        this.strComponentName = pObject.ToString();
        this.bSearch_By_ComponentName = true;
        this.bIsPrint_OnNotFound = bIsPrint_OnNotFound;
    }

    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(pElementType);

        if (pElementType == typeof(GameObject))
        {
            getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInChildren", new Type[] { typeof(bool) })
            .MakeGenericMethod(typeof(Transform));

            object[] arrObject = getter.Invoke(pTargetMono,
                    new object[] { this.bInclude_DeActive }) as object[];

            GameObject[] arrObjectReturn = new GameObject[arrObject.Length];
            for (int i = 0; i < arrObject.Length; i++)
                arrObjectReturn[i] = (arrObject[i] as Transform).gameObject;

            return arrObjectReturn;

        }
        else
        {
            return getter.Invoke(pTargetMono,
                    new object[] { this.bInclude_DeActive });
        }
    }
}

public class GetComponentInParentAttribute : GetComponentAttributeBase
{
    public override object GetComponent(MonoBehaviour pTargetMono, Type pElementType)
    {
        MethodInfo getter = typeof(MonoBehaviour)
          .GetMethod("GetComponentsInParent", new Type[] { typeof(bool) })
          .MakeGenericMethod(pElementType);

        if (pElementType == typeof(GameObject))
        {
            getter = typeof(MonoBehaviour)
            .GetMethod("GetComponentsInParent", new Type[] { typeof(bool) })
            .MakeGenericMethod(typeof(Transform));

            Transform pTransform = getter.Invoke(pTargetMono,
                new object[] { }) as Transform;

            return pTransform.gameObject;
        }
        else
        {
            return getter.Invoke(pTargetMono,
                new object[] { });
        }
    }
}


static public class SCManagerGetComponent
{
    static public Component GetComponentInChildren(this Component pTarget, string strObjectName, System.Type pComponentType, bool bInclude_DeActive)
    {
        Component[] arrComponentFind = null;
        if (pComponentType == typeof(GameObject))
            arrComponentFind = pTarget.transform.GetComponentsInChildren(typeof(Transform), true);
        else
            arrComponentFind = pTarget.transform.GetComponentsInChildren(pComponentType, bInclude_DeActive);


        for (int i = 0; i < arrComponentFind.Length; i++)
        {
            if (arrComponentFind[i].name.Equals(strObjectName))
                return arrComponentFind[i];
        }

        return null;
    }

    static public void DoUpdateGetComponentAttribute(MonoBehaviour pTarget)
    {
        // BindingFloags를 일일이 써야 잘 동작한다..

        System.Type pType = pTarget.GetType();
        MemberInfo[] arrMembers = pType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetFields(BindingFlags.Public | BindingFlags.Static);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetFields(BindingFlags.NonPublic | BindingFlags.Static);
        UpdateComponentAttribute(pTarget, arrMembers);

        // Property 구간
        arrMembers = pType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetProperties(BindingFlags.Public | BindingFlags.Static);
        UpdateComponentAttribute(pTarget, arrMembers);

        arrMembers = pType.GetProperties(BindingFlags.NonPublic | BindingFlags.Static);
        UpdateComponentAttribute(pTarget, arrMembers);
    }

    static private void UpdateComponentAttribute(MonoBehaviour pTargetMono, MemberInfo[] arrMember)
    {
        for (int i = 0; i < arrMember.Length; i++)
        {
            MemberInfo pMemberInfo = arrMember[i];
            object[] arrCustomAttributes = pMemberInfo.GetCustomAttributes(true);

            for (int j = 0; j < arrCustomAttributes.Length; j++)
            {
                GetComponentAttributeBase pGetcomponentAttribute = arrCustomAttributes[j] as GetComponentAttributeBase;
                if (pGetcomponentAttribute == null)
                    continue;

                System.Type pTypeField = pMemberInfo.MemberType();
                object pComponent = null;

                if (pTypeField.IsArray)
                {
                    pComponent = pGetcomponentAttribute.GetComponent(pTargetMono, pTypeField.GetElementType());
                }
                else if (pTypeField.IsGenericType)
                {
                    pComponent = ProcUpdateComponent_Generic(pTargetMono, pMemberInfo, (GetComponentInChildrenAttribute)pGetcomponentAttribute, pTypeField);
                }
                else
                {
                    if (pGetcomponentAttribute is GetComponentAttribute)
                        pComponent = pTargetMono.GetComponent(pTypeField);
                    else if (pGetcomponentAttribute is GetComponentInChildrenAttribute)
                    {
                        GetComponentInChildrenAttribute pAttributeInChildren = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
                        if (pAttributeInChildren.bSearch_By_ComponentName)
                            pComponent = pTargetMono.GetComponentInChildren(pAttributeInChildren.strComponentName, pTypeField, pAttributeInChildren.bInclude_DeActive);
                        else
                            pComponent = pTargetMono.GetComponentInChildren(pTypeField, pAttributeInChildren.bInclude_DeActive);
                    }
                    else if (pGetcomponentAttribute is GetComponentInParentAttribute)
                        pComponent = pTargetMono.GetComponentInParent(pTypeField);
                }

                if (pComponent == null && pGetcomponentAttribute.bIsPrint_OnNotFound)
                {
                    GetComponentInChildrenAttribute pAttribute = (GetComponentInChildrenAttribute)pGetcomponentAttribute;
                    if (pAttribute != null && pAttribute.bSearch_By_ComponentName)
                        Debug.LogWarning(pTargetMono.name + string.Format(".{0}<{1}>({2}) Result == null", pGetcomponentAttribute.GetType().Name, pTypeField, pAttribute.strComponentName), pTargetMono);
                    else
                        Debug.LogWarning(pTargetMono.name + string.Format(".{0}<{1}> Result == null", pGetcomponentAttribute.GetType().Name, pTypeField), pTargetMono);
                    continue;
                }

                if (pTypeField.IsGenericType == false)
                {
                    if(pTypeField == typeof(GameObject))
                        pMemberInfo.SetValue_Extension(pTargetMono, ((Component)pComponent).gameObject);
                    else
                        pMemberInfo.SetValue_Extension(pTargetMono, pComponent);
                }
            }
        }
    }

    static private object ProcUpdateComponent_Generic(MonoBehaviour pTargetMono, MemberInfo pMember, GetComponentInChildrenAttribute pGetComponentAttribute, System.Type pTypeField)
    {
        object pComponent = null;
        System.Type pTypeField_Generic = pTypeField.GetGenericTypeDefinition();
        Type[] arrArgumentsType = pTypeField.GetGenericArguments();

        if (pTypeField_Generic == typeof(List<>))
        {
            // List의 경우 GetComponentsInChildren 을 통해 Array를 얻은 뒤
            pComponent = pGetComponentAttribute.GetComponent(pTargetMono, arrArgumentsType[0]);
            // List 생성자에 Array를 집어넣는다.
            pMember.SetValue_Extension(pTargetMono, System.Activator.CreateInstance(pTypeField, pComponent));
        }
        else if (pTypeField_Generic == typeof(Dictionary<,>))
        {
            pComponent = ProcUpdateComponent_Dictionary(pTargetMono, pMember, pTypeField, pGetComponentAttribute, arrArgumentsType[0], arrArgumentsType[1]);
        }

        return pComponent;
    }

    static private object ProcUpdateComponent_Dictionary(MonoBehaviour pTargetMono, MemberInfo pMember, System.Type pTypeField, GetComponentInChildrenAttribute pAttributeInChildren, Type pType_DictionaryKey, Type pType_DictionaryValue)
    {
        object pComponent = pAttributeInChildren.GetComponent(pTargetMono, pType_DictionaryValue);
        Array arrComponent = pComponent as Array;

        if (arrComponent.Length == 0)
        {
            return null;
        }

        var Method_Add = pTypeField.GetMethod("Add", new[] {
                                pType_DictionaryKey, pType_DictionaryValue });

        // Reflection의 메소드는 Instance에서만 호출할수 있다.
        var pInstanceDictionary = System.Activator.CreateInstance(pTypeField);
        bool bIsDerived_DictionaryItem = false;
        Type[] arrInterfaces = pType_DictionaryValue.GetInterfaces();
        string strTypeName = typeof(IDictionaryItem<>).Name;
        for (int i = 0; i < arrInterfaces.Length; i++)
        {
            if (arrInterfaces[i].Name.Equals(strTypeName))
            {
                bIsDerived_DictionaryItem = true;
                break;
            }
        }

        if (pType_DictionaryKey == typeof(string))
        {
            for (int i = 0; i < arrComponent.Length; i++)
            {
                UnityEngine.Object pComponentChild = arrComponent.GetValue(i) as UnityEngine.Object;
                Method_Add.Invoke(pInstanceDictionary, new object[] {
                                pComponentChild.name,
                                pComponentChild });
            }

        }
        else
        {
            if (bIsDerived_DictionaryItem)
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
                        UnityEngine.Object pComponentChild = arrComponent.GetValue(i) as UnityEngine.Object;
                        try
                        {
                            var pEnum = System.Enum.Parse(pType_DictionaryKey, pComponentChild.name, true);
                            Method_Add.Invoke(pInstanceDictionary, new object[] {
                                    pEnum,
                                    pComponentChild });
                        }
                        catch { }
                    }
                }
            }

            // Debug.LogError("GetComponentAttribute Error Dictionary Key 타입은 string, enum만 됩니다. Key Type : " + pType_DictionaryKey.Name);
            // return null;
        }

        pMember.SetValue_Extension(pTargetMono, pInstanceDictionary);
        return pComponent;
    }
}

#region Test

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


    [GetComponentInChildren]
    public List<Test_ComponentChild> p_listTest = new List<Test_ComponentChild>();

    [GetComponentInChildren]
    public Dictionary<string, Test_ComponentChild> p_mapTest_KeyIsString = new Dictionary<string, Test_ComponentChild>();
    [GetComponentInChildren]
    private Dictionary<ETestChildObject, Test_ComponentChild> p_mapTest_KeyIsEnum = new Dictionary<ETestChildObject, Test_ComponentChild>();

    [GetComponentInChildren("TestObject_Other_FindString")]
    private Test_ComponentChild p_pChildComponent_FindString = null;
    [GetComponentInChildren(ETestChildObject.TestObject_Other_FindEnum)]
    private Test_ComponentChild p_pChildComponent_FindEnum = null;

    [GetComponentInChildren]
    public Test_ComponentChild p_pChildComponent_FindEnumProperty { get; private set; }

    [GetComponent]
    Test_ComponentOnly[] arrComponent = null;

    public void Awake()
    {
        SCManagerGetComponent.DoUpdateGetComponentAttribute(this);
    }

    [UnityTest]
    static public IEnumerator GetComponent_자식_필드_테스트()
    {
        GameObject pObjectParents = new GameObject("GetComponent_Test_Field");

        // GetComponent 대상인 자식 추가
        for (int i = 0; i < (int)GetComponentAttribute_Test.ETestChildObject.MAX; i++)
        {
            GameObject pObjectChild = new GameObject(((GetComponentAttribute_Test.ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild>();
        }

        // 자식을 전부 추가한 뒤에 페런츠에 추가한다.
        // 추가하자마자 Awake로 자식을 찾기 때문
        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();
        pParents.Awake();

        yield return null;

        // Getcomponent Attribute가 잘 작동했는지 체크 시작!!
        Assert.NotNull(pParents.p_pChildComponent_FindEnum);
        Assert.NotNull(pParents.p_pChildComponent_FindString);

        Assert.IsTrue(pParents.p_pChildComponent_FindString.name == GetComponentAttribute_Test.ETestChildObject.TestObject_Other_FindString.ToString());
        Assert.IsTrue(pParents.p_pChildComponent_FindEnum.name == GetComponentAttribute_Test.ETestChildObject.TestObject_Other_FindEnum.ToString());

        Assert.IsTrue(pParents.p_listTest.Count == (int)GetComponentAttribute_Test.ETestChildObject.MAX);

        Assert.IsTrue(pParents.p_mapTest_KeyIsEnum.Count == 5);
        Assert.IsTrue(pParents.p_mapTest_KeyIsString.Count == 5);

        var pIterString = pParents.p_mapTest_KeyIsString.GetEnumerator();
        while (pIterString.MoveNext())
        {
            Assert.IsTrue(pIterString.Current.Key == pIterString.Current.Value.name.ToString());
        }

        var pIterEnum = pParents.p_mapTest_KeyIsEnum.GetEnumerator();
        while (pIterEnum.MoveNext())
        {
            Assert.IsTrue(pIterEnum.Current.Key.ToString() == pIterEnum.Current.Value.name.ToString());
        }
    }

    [UnityTest]
    static public IEnumerator GetComponent_자식_Enum_테스트()
    {
        GameObject pObjectParents = new GameObject("GetComponent_Test_Enum");

        // GetComponent 대상인 자식 추가
        for (int i = 0; i < (int)GetComponentAttribute_Test.ETestChildObject.MAX; i++)
        {
            GameObject pObjectChild = new GameObject(((GetComponentAttribute_Test.ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild_DerivedDictionaryItem>();
        }

        // 자식을 전부 추가한 뒤에 페런츠에 추가한다.
        // 추가하자마자 Awake로 자식을 찾기 때문
        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();
        pParents.Awake();

        yield return null;

        var pIterEnum = pParents.p_mapTest_KeyIsEnum.GetEnumerator();
        while (pIterEnum.MoveNext())
        {
            Assert.IsTrue(pIterEnum.Current.Key.ToString() == pIterEnum.Current.Value.name.ToString());
        }
    }

    [UnityTest]
    static public IEnumerator GetComponent_자식_Property_테스트()
    {
        GameObject pObjectParents = new GameObject("GetComponent_Test_Property");

        // GetComponent 대상인 자식 추가
        for (int i = 0; i < (int)GetComponentAttribute_Test.ETestChildObject.MAX; i++)
        {
            GameObject pObjectChild = new GameObject(((GetComponentAttribute_Test.ETestChildObject)i).ToString());
            pObjectChild.transform.SetParent(pObjectParents.transform);
            pObjectChild.AddComponent<Test_ComponentChild>();
        }

        // 자식을 전부 추가한 뒤에 페런츠에 추가한다.
        // 추가하자마자 Awake로 자식을 찾기 때문
        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();
        pParents.Awake();

        yield return null;

        Assert.IsNotNull(pParents.p_pChildComponent_FindEnumProperty);
    }

    [UnityTest]
    static public IEnumerator GetComponent_Array_테스트()
    {
        GameObject pObjectParents = new GameObject("GetComponent_Test_GetComponentArray");

        // GetComponent 대상인 자식 추가
        int iAddComponentCount = 3;
        for (int i = 0; i < iAddComponentCount; i++)
            pObjectParents.AddComponent<Test_ComponentOnly>();

        // 자식을 전부 추가한 뒤에 페런츠에 추가한다.
        // 추가하자마자 Awake로 자식을 찾기 때문
        GetComponentAttribute_Test pParents = pObjectParents.AddComponent<GetComponentAttribute_Test>();
        pParents.Awake();

        yield return null;

        Assert.AreEqual(pParents.arrComponent.Length, iAddComponentCount);
    }
}

#endregion