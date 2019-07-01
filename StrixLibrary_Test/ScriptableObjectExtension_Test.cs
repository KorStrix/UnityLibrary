using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class ScriptableObjectExtension_Test
    {
        [System.Serializable]
        public class ScriptableObjectTest : ScriptableObject
        {
            public int iTest;
            public string strTest;
        }

        [Test]
        public void JsonParsing_Test()
        {
            ScriptableObjectTest pObjectTest = ScriptableObjectTest.CreateInstance<ScriptableObjectTest>();
            pObjectTest.iTest = 1;
            pObjectTest.strTest = "테스트";

            Assert.AreEqual(pObjectTest.iTest, 1);
            Assert.AreEqual(pObjectTest.strTest, "테스트");

            string strJsonText = pObjectTest.ConvertJson();

            pObjectTest.iTest = 2;
            pObjectTest.strTest = "Test";

            Assert.AreEqual(pObjectTest.iTest, 2);
            Assert.AreEqual(pObjectTest.strTest, "Test");

            pObjectTest.WriteJson(strJsonText);

            Assert.AreEqual(pObjectTest.iTest, 1);
            Assert.AreEqual(pObjectTest.strTest, "테스트");
        }
    }
}
