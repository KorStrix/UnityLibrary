using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class CList_Enter_Stay_Exit_Test
    {
        [Test]
        public void Working_Test()
        {
            SCManagerProfiler.DoStartTestCase(nameof(Working_Test));

            CList_Enter_Stay_Exit<int> list = new CList_Enter_Stay_Exit<int>();

            int[] arrEmpty = new int[] { };
            int[] arrValue = new int[] { 0, 1, 3, 5 };
            list.AddEnter(arrValue);

            Assert.AreEqual(list.p_list_Enter.ToArray(), arrValue);
            Assert.AreEqual(list.p_list_Stay.ToArray(), arrValue);
            Assert.AreEqual(list.p_list_Exit.ToArray(), arrEmpty);

            arrValue = new int[] { 0 };
            int[] arrExit = new int[] { 1, 3, 5 };
            list.AddEnter(arrValue);

            Assert.AreEqual(list.p_list_Enter.ToArray(), arrEmpty);
            Assert.AreEqual(list.p_list_Stay.ToArray(), arrValue);
            Assert.AreEqual(list.p_list_Exit.ToArray(), arrExit);

            SCManagerProfiler.DoFinishTestCase(nameof(Working_Test));
            SCManagerProfiler.DoPrintResult(true);
        }
    }
}
