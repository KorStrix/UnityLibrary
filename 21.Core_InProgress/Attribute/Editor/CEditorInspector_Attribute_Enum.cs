#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class CEditorEnumToStringHelper
{
    const string strFileName = "EnumToStringHelper";

    [System.Serializable]
    public class SEnumToStringData
    {
        public int iInstanceID;
        public string strEnumGroup;
        public string strEnumName;
        public string strEnumValue;
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptsReloaded()
    {
        // Debug.Log("OnScriptsReloaded" + System.DateTime.Now);

        //JsonUtility.FromJson<SCManagerParserJson.Wrapper_ForArray<SEnumToStringData>>
    }

    static public void DoInitListEnum_ContainAttribute<Attribute>(List<KeyValuePair<System.Type, Attribute>> listEnumType)
        where Attribute : System.Attribute
    {
        // 모든 클래스 중 애트리뷰트가 맞는 것을 찾는다.
        System.Reflection.Assembly[] arrAssembly = System.AppDomain.CurrentDomain.GetAssemblies();
        System.Type pTypeFind = typeof(Attribute);

        for (int i = 0; i < arrAssembly.Length; i++)
        {
            if (arrAssembly[i].FullName.Contains("CSharp") == false)
            {
                //Debug.LogWarning( arrAssembly[i].FullName + " Skipped " );
                continue;
            }

            System.Type[] arrAssemblyType = arrAssembly[i].GetTypes();
            for (int j = 0; j < arrAssemblyType.Length; j++)
            {
                System.Type pCurrentType = arrAssemblyType[j];
                System.Attribute[] arrAttribute = System.Attribute.GetCustomAttributes(arrAssemblyType[j]);
                for (int k = 0; k < arrAttribute.Length; k++)
                {
                    if (arrAttribute[k].GetType() == pTypeFind)
                        listEnumType.Add(new KeyValuePair<System.Type, Attribute>(pCurrentType, arrAttribute[k] as Attribute));
                }
            }
        }
    }
}

[CustomPropertyDrawer(typeof(EnumToStringAttribute))]
public class CEditorInspector_Attribute_EnumToString : PropertyDrawer
{
	const float const_fPropertyHeight_Default = 16f;

	List<KeyValuePair<System.Type, RegistEnumAttribute>> listEnumType = new List<KeyValuePair<System.Type, RegistEnumAttribute>>();

	public override void OnGUI( Rect position,
				   SerializedProperty property, GUIContent label )
	{
		EnumToStringAttribute pAttributeTarget = (EnumToStringAttribute)attribute;
		if (property.propertyType == SerializedPropertyType.String)
		{
			if (listEnumType.Count == 0)
                CEditorEnumToStringHelper.DoInitListEnum_ContainAttribute( listEnumType );

			if (listEnumType.Count == 0)
			{
				EditorGUI.LabelField( position, "ERROR:", "RegistEnum listEnum == 0" );
				return;
			}

			Dictionary<string, List<string>> mapEnumName = new Dictionary<string, List<string>>();
			Dictionary<string, List<System.Type>> mapEnumType = new Dictionary<string, List<System.Type>>();
			for (int i = 0; i < listEnumType.Count; i++)
			{
				System.Type pType = listEnumType[i].Key;
				RegistEnumAttribute pAttribute = listEnumType[i].Value;

				if(mapEnumName.ContainsKey( pAttribute.strGroupName ) == false)
					mapEnumName.Add( pAttribute.strGroupName, new List<string>());
				mapEnumName[pAttribute.strGroupName].Add( pType.FullName );

				if(mapEnumType.ContainsKey( pAttribute.strGroupName ) == false)
					mapEnumType.Add( pAttribute.strGroupName, new List<System.Type>() );
				mapEnumType[pAttribute.strGroupName].Add( pType );

				if (pAttributeTarget.pTypeEnum == pType)
					pAttributeTarget.iSelectIndex_EnumType = i;
			}

			bool bIsFind_SameEnumGroup = false;
			string[] arrEnumGroup = mapEnumName.Keys.ToArray();
			for(int i = 0; i < arrEnumGroup.Length; i++)
			{
				if (string.Equals( arrEnumGroup[i], pAttributeTarget.strSelectEnumGroup ))
				{
					pAttributeTarget.iSelectIndex_EnumGroup = i;
					bIsFind_SameEnumGroup = true;
					break;
				}
			}
			
			if(bIsFind_SameEnumGroup == false)
			{
				pAttributeTarget.strSelectEnumGroup = arrEnumGroup[pAttributeTarget.iSelectIndex_EnumGroup];
				pAttributeTarget.iSelectIndex_EnumType = 0;
				pAttributeTarget.iSelectIndex_EnumValue = 0;
				pAttributeTarget.pTypeEnum = mapEnumType[pAttributeTarget.strSelectEnumGroup][pAttributeTarget.iSelectIndex_EnumType];
			}

			float fPosOffset_Y = position.y;
			position = new Rect( position.x, fPosOffset_Y, position.width, const_fPropertyHeight_Default );
			fPosOffset_Y += const_fPropertyHeight_Default;

			bool bChangeEnumGroup = false;
			EditorGUI.BeginChangeCheck();
			pAttributeTarget.iSelectIndex_EnumGroup = EditorGUI.Popup( position, "Select Enum Group", pAttributeTarget.iSelectIndex_EnumGroup, arrEnumGroup );
			if (EditorGUI.EndChangeCheck())
			{
				bChangeEnumGroup = true;
				pAttributeTarget.strSelectEnumGroup = arrEnumGroup[pAttributeTarget.iSelectIndex_EnumGroup];
				pAttributeTarget.iSelectIndex_EnumType = 0;
			}

			position = new Rect( position.x, fPosOffset_Y, position.width, const_fPropertyHeight_Default );
			fPosOffset_Y += const_fPropertyHeight_Default;

			string[] arrEnumName = mapEnumName[pAttributeTarget.strSelectEnumGroup].ToArray();

			EditorGUI.BeginChangeCheck();
			pAttributeTarget.iSelectIndex_EnumType = EditorGUI.Popup( position, "Select Enum", pAttributeTarget.iSelectIndex_EnumType, arrEnumName );
			if (bChangeEnumGroup || EditorGUI.EndChangeCheck())
			{
				pAttributeTarget.iSelectIndex_EnumValue = 0;
				pAttributeTarget.pTypeEnum = mapEnumType[pAttributeTarget.strSelectEnumGroup][pAttributeTarget.iSelectIndex_EnumType];
			}

			position = new Rect( position.x, fPosOffset_Y, position.width, const_fPropertyHeight_Default );
			fPosOffset_Y += const_fPropertyHeight_Default;

			string[] arrEnumValue = System.Enum.GetNames( pAttributeTarget.pTypeEnum );
			pAttributeTarget.iSelectIndex_EnumValue = EditorGUI.Popup( position, property.displayName, pAttributeTarget.iSelectIndex_EnumValue, arrEnumValue );
			property.stringValue = arrEnumValue[pAttributeTarget.iSelectIndex_EnumValue];

		}
		else
			EditorGUI.LabelField( position, "ERROR:", "May only apply to type string or Enum" );
	}

	public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
	{
		return base.GetPropertyHeight( property, label ) * 3f;
	}
}
