#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-05-16 오후 7:09:19
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StrixLibrary_Example
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(menuName = "StrixSO/" + nameof(CommandList_Example))]
    public class CommandList_Example : CommandListBase
    {
        protected override Type _pGetInheritedClassType => typeof(CommandList_Example);

        public class MovePlayerCharacter : CommandBase
        {
            SimpleMoveAble_Example _pPlayerMove;

            public override void OnInitCommand(out bool bIsInit)
            {
                base.OnInitCommand(out bIsInit);

                _pPlayerMove = GameManager_InputExample.instance.p_pMoveAble_Player;
                bIsInit = _pPlayerMove != null;
            }

            public override void DoExcute(ref SInputValue sInputValue, ref bool bIsExcuted_DefaultIsTrue)
            {
                _pPlayerMove.DoMove(new Vector2(sInputValue.fAxisValue_Minus1_1, 0f));
                Debug.LogError("MovePlayer_Character Axis : " + sInputValue.fAxisValue_Minus1_1);
            }
        }

        public class Print_TestLog : CommandBase
        {
            public override void DoExcute(ref SInputValue sInputValue, ref bool bIsExcuted_DefaultIsTrue)
            {
                Debug.LogError("Test Log");
            }
        }
    }
}