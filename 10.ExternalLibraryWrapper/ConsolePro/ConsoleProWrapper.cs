#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2018-12-05 오전 10:56:19
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

static public class ConsoleProWrapper
{
    static public string ConvertLog_ToCore(string strLog)
    {
        return ConvertLog(strLog, "Core");
    }

    static public string ConvertLog(string strLog, string strFilter)
    {
#if ConsolePro
        return strLog + "\nCPAPI:{\"cmd\":\"Filter\" \"name\":\"" + strFilter + "\"}";
#else
        return string.Format("{0}", strLog);
#endif
    }
}
