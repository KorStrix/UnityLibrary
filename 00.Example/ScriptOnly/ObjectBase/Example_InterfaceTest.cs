#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-01-17 오후 4:07:23
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine.TestTools;
#endif

namespace Strix
{
    public interface IInterfaceTest_A
    {
    }
}

public class Example_InterfaceTest : MonoBehaviour, Strix.IInterfaceTest_A
{
}
