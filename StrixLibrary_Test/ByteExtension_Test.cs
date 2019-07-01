using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class ByteExtension_Test
    {
        public enum ETest
        {
            ETest1
        }

        [Test]
        static public void Byte_To_BitArray()
        {
            Assert.IsTrue(ByteExtension.ConvertByte_To_Int(1) == 1);
            Assert.IsTrue(ByteExtension.ConvertByte_To_Int(1, 8, 1) == 0);
            Assert.IsTrue(ByteExtension.ConvertByte_To_Int(2, 8, 0) == 2);
            Assert.IsTrue(ByteExtension.ConvertByte_To_Int(127, 8, 2) == (127 - 3));
        }
    }
}
