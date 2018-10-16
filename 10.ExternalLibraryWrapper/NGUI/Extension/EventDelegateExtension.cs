using UnityEngine;
using System.Collections;
#if NGUI
public partial class EventDelegate
{
    public EventDelegate(MonoBehaviour target, string methodName, params object[] pParams)
    {
        Set(target, methodName);

        mParameters = new Parameter[pParams.Length];
        for (int i = 0; i < pParams.Length; i++)
            mParameters[i] = new Parameter(pParams[i]);
    }

}
#endif
