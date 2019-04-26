#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-22 오전 11:16:10
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

/// <summary>
/// 
/// </summary>
public class CExecuteContainer_Example : CObjectBase
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public abstract class ExecuterBase : IExecuter
    {
        public int p_iExecuterOrder => 0;

        public void IExecuter_Check_IsInvalid_OnEditor(CObjectBase pScriptOwner, ref bool bIsInvalid_Default_IsFalse, ref string strErrorMessage_Default_Is_Error)
        {
        }

        public void IExecuter_OnAwake(IExecuteContainer pContainer, CObjectBase pScriptOwner)
        {
        }

        public void IExecuter_OnEnable(IExecuteContainer pContainer, CObjectBase pScriptOwner)
        {
        }

        public string IHasName_GetName()
        {
            return this.ToStringSub();
        }
    }

    [RegistSubString("Executer1")]
    public class Executer1 : ExecuterBase
    {
    }

    [RegistSubString("Executer2")]
    public class Executer2 : ExecuterBase
    {
    }

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    [SerializeField]
    [ShowDrawerChain]
    CExecuterContainer<ExecuterBase> _pExecuteContainer = new CExecuterContainer<ExecuterBase>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}