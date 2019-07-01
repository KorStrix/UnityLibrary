#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-11-13 오후 5:21:44
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IResourceEventListener
{
    void IResourceEventListener_Excute(string strEventName);
}
