using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    [Category("Attribute")]
    public class ToSubStringAttribute_Test
    {
        public enum ETest
        {
            [RegistSubString("Test11")]
            Test1,
            Test2,
        }

        public class ClassToSubStringBase { }
        [RegistSubString("Test11")]
        public class ClassToSubString : ClassToSubStringBase { }
        public class ClassToSubString2 : ClassToSubStringBase { }

        [Test]
        public void 이넘_투_서브스트링_테스트()
        {
            Assert.AreEqual(ETest.Test1.ToStringSub(), "Test11");
            Assert.AreEqual(ETest.Test2.ToStringSub(), "Test2");
        }

        [Test]
        public void 오브젝트_투_서브스트링_테스트()
        {
            Assert.AreEqual(new ClassToSubStringBase().ToStringSub(), new ClassToSubStringBase().ToString());
            Assert.AreEqual(new ClassToSubString().ToStringSub(), "Test11");
            Assert.AreEqual(new ClassToSubString2().ToStringSub(), new ClassToSubString2().ToString());
        }
    }
}
