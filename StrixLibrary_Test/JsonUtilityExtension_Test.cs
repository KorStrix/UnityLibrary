using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class JsonUtilityExtension_Test
    {
        private const string const_strFolderPath = "Test_SCParserJson";

        [System.Serializable]
        public class STestJson
        {
            public int _int = -1;
            public string _string = "string";
            public Vector3 _vector3 = Vector3.one;

            public void DoSetDataRandom()
            {
                _int = Random.Range(1, 1000);
                _string = _int.ToString() + " 한글 테스트";
                _vector3 = Vector3.one * _int;
            }
        }

        [System.Serializable]
        public class STestJson_ScriptableObject : ScriptableObject
        {
            public int _int = -1;
            public string _string = "string";
            public Vector3 _vector3 = Vector3.one;

            public void DoSetDataRandom()
            {
                _int = Random.Range(1, 1000);
                _string = _int.ToString() + " 한글 테스트";
                _vector3 = Vector3.one * _int;
            }
        }



        [Test]
        public void Write_And_Read_SingleData()
        {
            STestJson pTestJson = new STestJson();
            STestJson pTestJson2 = new STestJson();
            pTestJson.DoSetDataRandom();

            Assert.IsFalse(pTestJson._int == pTestJson2._int);
            Assert.IsFalse(pTestJson._string == pTestJson2._string);
            Assert.IsFalse(pTestJson._vector3 == pTestJson2._vector3);

            JsonUtilityExtension.DoWriteJson(const_strFolderPath, nameof(Write_And_Read_SingleData), pTestJson);
            JsonUtilityExtension.DoReadJson(const_strFolderPath, nameof(Write_And_Read_SingleData), out pTestJson2);

            Assert.IsTrue(pTestJson._int == pTestJson2._int);
            Assert.IsTrue(pTestJson._string == pTestJson2._string);
            Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
        }

        [Test]
        public void Write_And_Read_MultipleData()
        {
            List<STestJson> listTestJson = new List<STestJson>();
            List<STestJson> listTestJson2 = new List<STestJson>();

            for (int i = 0; i < 3; i++)
            {
                STestJson pTestJson = new STestJson();
                pTestJson.DoSetDataRandom();

                listTestJson.Add(pTestJson);
            }

            JsonUtilityExtension.DoWriteJsonArray(const_strFolderPath, nameof(Write_And_Read_MultipleData), listTestJson.ToArray());
            JsonUtilityExtension.DoReadJsonArray(const_strFolderPath, nameof(Write_And_Read_MultipleData), ref listTestJson2);

            for (int i = 0; i < 3; i++)
            {
                STestJson pTestJson = listTestJson[i];
                STestJson pTestJson2 = listTestJson2[i];

                Assert.IsTrue(pTestJson._int == pTestJson2._int);
                Assert.IsTrue(pTestJson._string == pTestJson2._string);
                Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
            }
        }

        [Test]
        public void Write_And_Read_SingleScriptableData()
        {
            STestJson_ScriptableObject pTestJson = ScriptableObject.CreateInstance<STestJson_ScriptableObject>();
            STestJson_ScriptableObject pTestJson2 = ScriptableObject.CreateInstance<STestJson_ScriptableObject>();
            pTestJson.DoSetDataRandom();

            Assert.IsFalse(pTestJson._int == pTestJson2._int);
            Assert.IsFalse(pTestJson._string == pTestJson2._string);
            Assert.IsFalse(pTestJson._vector3 == pTestJson2._vector3);

            JsonUtilityExtension.DoWriteJson(const_strFolderPath, nameof(Write_And_Read_SingleScriptableData), pTestJson);
            JsonUtilityExtension.DoReadJson_ScriptableObject(const_strFolderPath, nameof(Write_And_Read_SingleScriptableData), out pTestJson2);

            Assert.IsTrue(pTestJson._int == pTestJson2._int);
            Assert.IsTrue(pTestJson._string == pTestJson2._string);
            Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
        }

        [Test]
        public void Write_And_Read_MultipleScriptableData()
        {
            List<STestJson_ScriptableObject> listTestJson = new List<STestJson_ScriptableObject>();
            List<STestJson_ScriptableObject> listTestJson2 = new List<STestJson_ScriptableObject>();

            for (int i = 0; i < 3; i++)
            {
                STestJson_ScriptableObject pTestJson = ScriptableObject.CreateInstance<STestJson_ScriptableObject>();
                pTestJson.DoSetDataRandom();

                listTestJson.Add(pTestJson);
            }

            JsonUtilityExtension.DoWriteJsonArray(const_strFolderPath, nameof(Write_And_Read_MultipleScriptableData), listTestJson.ToArray());
            JsonUtilityExtension.DoReadJsonArray_ScriptableObject(const_strFolderPath, nameof(Write_And_Read_MultipleScriptableData), ref listTestJson2);

            for (int i = 0; i < 3; i++)
            {
                STestJson_ScriptableObject pTestJson = listTestJson[i];
                STestJson_ScriptableObject pTestJson2 = listTestJson2[i];

                Assert.IsTrue(pTestJson._int == pTestJson2._int);
                Assert.IsTrue(pTestJson._string == pTestJson2._string);
                Assert.IsTrue(pTestJson._vector3 == pTestJson2._vector3);
            }
        }
    }

}
