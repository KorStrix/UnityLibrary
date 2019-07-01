using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StrixSO/Example/" + nameof(CommandList_Example))]
public class CommandList_Example : CommandListBase
{
    protected override Type _pGetInheritedClassType => typeof(CommandList_Example);

    public class Command_PrintLog_1 : CommandBase
    {
        public override void DoExcute(ref SInputValue sInputValue, ref bool bIsExcuted_DefaultIsTrue)
        {
            Debug.Log("1");
        }
    }

    public class Command_PrintLog_2 : CommandBase
    {
        public override void DoExcute(ref SInputValue sInputValue, ref bool bIsExcuted_DefaultIsTrue)
        {
            Debug.Log("2");
        }
    }

    public class Command_PrintLog_3 : CommandBase
    {
        public override void DoExcute(ref SInputValue sInputValue, ref bool bIsExcuted_DefaultIsTrue)
        {
            Debug.Log("3");
        }
    }
}
