#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-04-26 오후 7:18:53
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// https://stackoverflow.com/questions/4185521/c-sharp-get-generic-type-name/26429045
public static class TypeExtension
{
    public static string GetFriendlyName(this System.Type type)
    {
        if (type == null)
            return "";

        string friendlyName = type.Name;
        if (type.IsGenericType)
        {
            int iBacktick = friendlyName.IndexOf('`');
            if (iBacktick > 0)
            {
                friendlyName = friendlyName.Remove(iBacktick);
            }
            friendlyName += "<";
            System.Type[] typeParameters = type.GetGenericArguments();
            for (int i = 0; i < typeParameters.Length; ++i)
            {
                string typeParamName = GetFriendlyName(typeParameters[i]);
                friendlyName += (i == 0 ? typeParamName : "," + typeParamName);
            }
            friendlyName += ">";
        }

        return friendlyName;
    }
}