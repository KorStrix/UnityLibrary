using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strix
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class CustomLogLevelAttribute : Attribute
    {
        public ELogLevel p_eLogLevel { get; private set; }
        public int p_iLogLevelGroup_HashCode { get; private set; }

        public CustomLogLevelAttribute(ELogLevel eLogLevel)
        {
            p_eLogLevel = eLogLevel;
            p_iLogLevelGroup_HashCode = -1;
        }

        public CustomLogLevelAttribute(ELogLevel eLogLevel, int iLogLevelGroupHashCode)
        {
            p_eLogLevel = eLogLevel;
            p_iLogLevelGroup_HashCode = iLogLevelGroupHashCode;
        }

        public CustomLogLevelAttribute(ELogLevel eLogLevel, System.Type eEnumValue)
        {
            p_eLogLevel = eLogLevel;
            p_iLogLevelGroup_HashCode = eEnumValue.GetHashCode();
        }
    }

    // Log, Warning, Error는 string 체크 시 유니티 기본 Level과 겹치므로 CustomLine_을 붙인다.
    public enum ELogLevel
    {
        CustomLine_Log,
        CustomLine_Warning,
        CustomLine_Error,
    }

    static class SCDebugEnumHelper
    {
        static public ELogLevel ConvertToCustomLogLevel( this UnityEngine.LogType eLogType )
		{
			switch (eLogType)
			{
				case UnityEngine.LogType.Warning:
					return ELogLevel.CustomLine_Warning;

				case UnityEngine.LogType.Exception:
				case UnityEngine.LogType.Error:
				case UnityEngine.LogType.Assert:
					return ELogLevel.CustomLine_Error;
			}

			return ELogLevel.CustomLine_Log;
		}

		static public string ToStringCustom( this ELogLevel eLogType )
		{
			switch (eLogType)
			{
				case ELogLevel.CustomLine_Warning: return "WNG";
				case ELogLevel.CustomLine_Error: return "ERR";
			}

			return "LOG";
		}
    }

	public class Debug<Enum_LogWriter, Enum_LogLevel>
    {
        public enum EFileNameType
        {
            OnSecond,
            OnMinute,
            OnHour,
            OnDay,
            OnMonth,
        }

        [Flags]
        public enum EFlagFileExportType
        {
            NotGenerateFile = 1,
            TXT = 2,
            CSV = 4,
        }

        static readonly Dictionary<EFlagFileExportType, string> const_mapExtensionName = new Dictionary<EFlagFileExportType, string>()
        {
            {EFlagFileExportType.TXT, ".txt" }, {EFlagFileExportType.CSV, ".csv" }
        };

        static Dictionary<string, ELogLevel> _mapLogLevelConvert = new Dictionary<string, ELogLevel>();
        static Dictionary<int, List<ELogLevel>> _mapLogLevel_GroupList = new Dictionary<int, List<ELogLevel>>();
        static Dictionary<string, int> _mapLogLevelGroup = new Dictionary<string, int>();

        static HashSet<int> _setIgnoreLogLevelGroup = new HashSet<int>();
        static HashSet<string> _setIgnoreLogLevel = new HashSet<string>();
        static HashSet<string> _setIgnoreLogWriter = new HashSet<string>();

        static StringBuilder _pStrBuilder = new StringBuilder();
        static EFileNameType _eFileNameType = EFileNameType.OnSecond;


        static string _strLogFilePathAll;
        static string _strLogFilePath = "Log~";	// 접미어로 ~를 붙이면 Unity에서 무시한다.
        static string _strLogFileName = null;
        static bool _bIsInitPath = false;

        static int _eFlagFileExportType = (int)EFlagFileExportType.NotGenerateFile;

        static string _strWriter = "System";

        static public void ClearCustomLogSetting()
        {
            _mapLogLevelConvert.Clear();
            _mapLogLevel_GroupList.Clear();
            _mapLogLevelGroup.Clear();

            _setIgnoreLogLevelGroup.Clear();
            _setIgnoreLogLevel.Clear();
            _setIgnoreLogWriter.Clear();

            _eFlagFileExportType = (int)EFlagFileExportType.NotGenerateFile;
        }

        static public void SetLogWriter(Enum_LogWriter eLogWriter)
        {
            _strWriter = eLogWriter.ToString();
        }

        static public void SetIgnoreLogWriter(params Enum_LogWriter[] arrLogWriter)
        {
			_setIgnoreLogWriter.Clear();
            for (int i = 0; i < arrLogWriter.Length; i++)
				_setIgnoreLogWriter.Add( arrLogWriter[i].ToString());
        }

        static public void SetIgnoreLogLevel(params Enum_LogLevel[] arrLogLevel)
        {
            _setIgnoreLogLevel.Clear();
            for (int i = 0; i < arrLogLevel.Length; i++)
				_setIgnoreLogLevel.Add(arrLogLevel[i].ToString());
        }

        static public void SetIgnoreLogLevelGroup<Enum_LogGroup>(Enum_LogGroup eLogGroup)
        {
            _setIgnoreLogLevelGroup.Add(eLogGroup.GetHashCode());
        }

        static public void SetIgnoreLogLevelGroup(int iLogGroup)
        {
            _setIgnoreLogLevelGroup.Add(iLogGroup);
        }


        static public void SetFileExportType(params EFlagFileExportType[] arrFlagFileExportType)
        {
            _eFlagFileExportType = 0;
            for (int i = 0; i < arrFlagFileExportType.Length; i++)
                _eFlagFileExportType |= arrFlagFileExportType[i].GetHashCode();
        }
        
        static public void SetFileNameType(EFileNameType eFileNameType)
        {
            _eFileNameType = eFileNameType;
        }

        static public void SetLogFilePath(string strLogFilePath)
        {
            _strLogFilePath = strLogFilePath;
        }

        /// <summary>
        /// 유니티 로그 콜백 등록용
        /// </summary>
        static public void OnUnityDebugLogCallBack(string strLog, string strStackTrace, UnityEngine.LogType eLogType)
        {
            ProcWriteLog(strLog, eLogType.ConvertToCustomLogLevel());
        }

        //static public void Log(Enum_LogWriter eLogWriter, string strLog, UnityEngine.Object pObject = null)
        //{
        //    if (CheckIsIgnore(ELogLevel.CustomLine_Log, eLogWriter)) return;

        //    UnityEngine.Debug.Log(ProcLogLine(strLog, ELogLevel.CustomLine_Log, eLogWriter, true), pObject);
        //    ProcWriteLog(strLog, ELogLevel.CustomLine_Log, eLogWriter);
        //}

        static public void LogWarning(Enum_LogWriter eLogWriter, string strLog, UnityEngine.Object pObject = null)
        {
            if (CheckIsIgnore(ELogLevel.CustomLine_Warning, eLogWriter)) return;

            UnityEngine.Debug.LogWarning(ProcLogLine(strLog, ELogLevel.CustomLine_Warning, eLogWriter, true), pObject);
            ProcWriteLog(strLog, ELogLevel.CustomLine_Warning, eLogWriter);
        }

        static public void LogError(Enum_LogWriter eLogWriter, string strLog, UnityEngine.Object pObject = null)
        {
            if (CheckIsIgnore(ELogLevel.CustomLine_Error, eLogWriter)) return;

            UnityEngine.Debug.LogError(ProcLogLine(strLog, ELogLevel.CustomLine_Error, eLogWriter, true), pObject);
            ProcWriteLog(strLog, ELogLevel.CustomLine_Error, eLogWriter);
        }


        static public void Log(Enum_LogWriter eLogWriter, Enum_LogLevel eLogLevel, string strLog, UnityEngine.Object pObject = null)
        {
            if (CheckIsIgnore(eLogLevel, eLogWriter)) return;

            ELogLevel eLogLevelConvert = _mapLogLevelConvert[eLogLevel.ToString()];
            if (eLogLevelConvert < ELogLevel.CustomLine_Warning)
                UnityEngine.Debug.Log(ProcLogLine(strLog, eLogLevel, eLogWriter, true), pObject);
            else if (eLogLevelConvert < ELogLevel.CustomLine_Error)
                UnityEngine.Debug.LogWarning( ProcLogLine( strLog, eLogLevel, eLogWriter, true ), pObject);
            else
                UnityEngine.Debug.LogError( ProcLogLine( strLog, eLogLevel, eLogWriter, true ), pObject);

            ProcWriteLog(strLog, eLogLevel, eLogWriter);
        }

        #region  // ================= 기본적인 유니티 로그 래핑 ================= //

        //static public void Log(string strLog, UnityEngine.Object pObject = null)
        //{
        //    if (CheckIsIgnore(ELogLevel.CustomLine_Log)) return;

        //    UnityEngine.Debug.Log(strLog, pObject);
        //    ProcWriteLog(strLog, ELogLevel.CustomLine_Log);
        //}

        //static public void LogWarning(string strLog, UnityEngine.Object pObject = null)
        //{
        //    if (CheckIsIgnore(ELogLevel.CustomLine_Warning)) return;

        //    UnityEngine.Debug.LogWarning(strLog, pObject);
        //    ProcWriteLog(strLog, ELogLevel.CustomLine_Warning);
        //}

        //static public void LogError(string strLog, UnityEngine.Object pObject = null)
        //{
        //    if (CheckIsIgnore(ELogLevel.CustomLine_Error)) return;

        //    UnityEngine.Debug.LogError(strLog, pObject);
        //    ProcWriteLog(strLog, ELogLevel.CustomLine_Error);
        //}

        #endregion

        #region  // ================= 출력파일 타입별 로직 전개 ================= //

        static private void ProcInitFilePath()
        {
            _bIsInitPath = true;
            string strDirectory = UnityEngine.Application.dataPath;
            _strLogFilePathAll = Path.Combine(strDirectory, _strLogFilePath);

            if ((_eFlagFileExportType & (int)EFlagFileExportType.TXT) == (int)EFlagFileExportType.TXT)
                ProcWriteTxt_StartLog(File.Exists(_strLogFilePathAll));

            if ((_eFlagFileExportType & (int)EFlagFileExportType.CSV) == (int)EFlagFileExportType.CSV)
            {
                _pStrBuilder.Length = 0;
                _pStrBuilder.Append("Time,LogLevel,LogWriter,Log");
                _pStrBuilder.Append(Environment.NewLine);
                ProcWriteTo_File(_pStrBuilder.ToString(), EFlagFileExportType.CSV);
            }


            UnityEngine.Debug.Log("Record LogFile!! - Check Path:" + _strLogFilePathAll);
        }

        static private void ProcWriteLog<Enum_LogLevel_Local>(string strLog, Enum_LogLevel_Local eLogLevel, Enum_LogWriter eLogWriter = default(Enum_LogWriter))
        {
            if ((_eFlagFileExportType & (int)EFlagFileExportType.NotGenerateFile) == (int)EFlagFileExportType.NotGenerateFile) return;

            if (_bIsInitPath == false)
                ProcInitFilePath();

            if (CheckIsIgnore(eLogLevel)) return;

            if ((_eFlagFileExportType & (int)EFlagFileExportType.TXT) == (int)EFlagFileExportType.TXT)
                ProcWriteLog_TXT(strLog, eLogLevel, eLogWriter);

            if ((_eFlagFileExportType & (int)EFlagFileExportType.CSV) == (int)EFlagFileExportType.CSV)
                ProcWriteLog_CSV(strLog, eLogLevel, eLogWriter);
        }

        static private void ProcWriteLog_TXT<Enum_LogLevel_Local>(string strLog, Enum_LogLevel_Local eLogLevel, Enum_LogWriter eLogWriter)
        {
            string strLogResult = ProcLogLine(strLog, eLogLevel, eLogWriter, false);
            ProcWriteTo_File(strLogResult, EFlagFileExportType.TXT);
        }

        static private void ProcWriteLog_CSV<Enum_LogLevel_Local>(string strLog, Enum_LogLevel_Local eLogLevel, Enum_LogWriter eLogWriter)
        {
            System.DateTime sDateTime = System.DateTime.Now;
            string strCurrentTime = sDateTime.ToString("HH:mm:ss.fff");

            // CSV의 경우 다음줄이 있으면 포멧이 망가져서 수정
            strLog = strLog.Replace(Environment.NewLine, " ");
            strLog = strLog.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");

            // CSV의 경우 ,이 있으면 포멧이 망가져서 수정
            strLog = strLog.Replace(",", ".");

            // Time, LogLevel, LogWriter, Log
            _pStrBuilder.Length = 0;
            _pStrBuilder.Append(strCurrentTime);
            _pStrBuilder.Append(",");
            _pStrBuilder.Append(GetLogLevelString(eLogLevel));
            _pStrBuilder.Append(",");
            if (eLogWriter == null)
                _pStrBuilder.Append(_strWriter.ToString());
            else
                _pStrBuilder.Append(eLogWriter.ToString());
            _pStrBuilder.Append(",");
            _pStrBuilder.Append(strLog);
            _pStrBuilder.Append(Environment.NewLine);

            ProcWriteTo_File(_pStrBuilder.ToString(), EFlagFileExportType.CSV);
        }

        #endregion

        #region  // ================= 공용 로직 전개 ================= //

        static private void ProcWriteTo_File(string strText, EFlagFileExportType eFileType)
        {
            string strFilePath = Path.Combine(_strLogFilePathAll, GetFileName(eFileType));
            try
            {
                if (File.Exists(strFilePath) == false)
                {
                    string strDirectory = Path.GetDirectoryName(strFilePath);
                    if (Directory.Exists(strDirectory) == false)
                        Directory.CreateDirectory(strDirectory);

                    File.WriteAllText(strFilePath, strText, Encoding.UTF8);
                }
                else
                    File.AppendAllText(strFilePath, strText, Encoding.UTF8);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("File Write Error - " + strFilePath + " Exception : " + e);
            }
        }

        static private string ProcLogLine<Enum_LogLevel_Local>(string strLog, Enum_LogLevel_Local eLogLevel, Enum_LogWriter eLogWriter, bool bIsForUnityConsole)
        {
            System.DateTime sDateTime = System.DateTime.Now;
            string strCurrentTime = sDateTime.ToString("HH:mm:ss.fff");

            _pStrBuilder.Length = 0;
            _pStrBuilder.Append("[");
            _pStrBuilder.Append(strCurrentTime.ToString());
            _pStrBuilder.Append("]");
            _pStrBuilder.Append("[");

			// 콘솔의 경우 로그 레벨의 문자열 길이가 상관없어 그대로 출력하지만, 파일 입출력시에는 보기 좋지 않으므로 따로 처리
			if(bIsForUnityConsole)
				_pStrBuilder.Append( eLogLevel.ToString() );
			else
				_pStrBuilder.Append( GetLogLevelString( eLogLevel ) );

			_pStrBuilder.Append("]");
            if (eLogWriter == null)
                _pStrBuilder.Append(_strWriter.ToString());
            else
                _pStrBuilder.Append(eLogWriter.ToString());

            if (bIsForUnityConsole)
            {
                _pStrBuilder.Append(Environment.NewLine);
                _pStrBuilder.Append(strLog);
            }
            else
            {
                _pStrBuilder.Append(" : ");
                _pStrBuilder.Append(strLog);
                _pStrBuilder.Append(Environment.NewLine);
            }

            return _pStrBuilder.ToString();
        }

        static private bool CheckIsIgnore<Enum_LogLevel_Local>(Enum_LogLevel_Local eLogLevel, Enum_LogWriter eLogWriter = default(Enum_LogWriter))
        {
            string strLogLevel = CheckIs_InitLogLevel(eLogLevel);
            if (_setIgnoreLogLevel.Contains(strLogLevel))
                return true;

            int iGroupID = _mapLogLevelGroup[strLogLevel];
            if (_setIgnoreLogLevelGroup.Contains(iGroupID))
                return true;

            if(eLogWriter != null)
            {
                if (_setIgnoreLogWriter.Contains(eLogWriter.ToString()))
                    return true;
            }

            return false;
        }

        static private string GetFileName(EFlagFileExportType eFloagFileExportType)
        {
            if (string.IsNullOrEmpty(_strLogFileName))
            {
                _strLogFileName = "Log";
                System.DateTime sDateTime = System.DateTime.Now;
                switch (_eFileNameType)
                {
                    case EFileNameType.OnSecond: _strLogFileName += sDateTime.ToString("_MMdd_HH_mm_ss"); break;
                    case EFileNameType.OnMinute: _strLogFileName += sDateTime.ToString("_MMdd_HH_mm"); break;
                    case EFileNameType.OnHour: _strLogFileName += sDateTime.ToString("_MMd_HH"); break;
                    case EFileNameType.OnDay: _strLogFileName += sDateTime.ToString("_MMdd"); break;
                    case EFileNameType.OnMonth: _strLogFileName += sDateTime.ToString("_MM"); break;
                }
            }

            string strLogFileName = _strLogFileName;
            strLogFileName += const_mapExtensionName[eFloagFileExportType];
            return strLogFileName;
        }

        static private void ProcWriteTxt_StartLog(bool bIsAppend)
        {
            System.DateTime sDateTime = System.DateTime.Now;
            string strCurrentTime = sDateTime.ToString("HH:mm:ss.");
            strCurrentTime += sDateTime.Millisecond;

            _pStrBuilder.Length = 0;
            if(bIsAppend)
            {
                _pStrBuilder.Append(Environment.NewLine);
                _pStrBuilder.Append(Environment.NewLine);
            }

            _pStrBuilder.Append("============================");
            _pStrBuilder.Append(Environment.NewLine);
            _pStrBuilder.Append("[");
            _pStrBuilder.Append(strCurrentTime.ToString());
            _pStrBuilder.Append("] ");
            _pStrBuilder.Append("Start Log!!");
            _pStrBuilder.Append(Environment.NewLine);
            _pStrBuilder.Append("============================");
            _pStrBuilder.Append(Environment.NewLine);
            _pStrBuilder.Append(Environment.NewLine);

            ProcWriteTo_File(_pStrBuilder.ToString(), EFlagFileExportType.TXT);
        }

        static private string GetLogLevelString<Enum_LogLevel_Local>(Enum_LogLevel_Local eLogLevel)
        {
            string strLogLevelString = null;
            string strLogLevel = CheckIs_InitLogLevel(eLogLevel);

            ELogLevel eLogLevelConvert = _mapLogLevelConvert[strLogLevel];
            if (eLogLevelConvert < ELogLevel.CustomLine_Warning)
                strLogLevelString = ELogLevel.CustomLine_Log.ToStringCustom();
            else if (eLogLevelConvert < ELogLevel.CustomLine_Error)
                strLogLevelString = ELogLevel.CustomLine_Warning.ToStringCustom();
            else
                strLogLevelString = ELogLevel.CustomLine_Error.ToStringCustom();

            return strLogLevelString;
        }

        private static string CheckIs_InitLogLevel<Enum_LogLevel_Local>(Enum_LogLevel_Local eLogLevel)
        {
            string strLogLevel = eLogLevel.ToString();
            if (_mapLogLevelConvert.ContainsKey(strLogLevel) == false)
            {
                CustomLogLevelAttribute pAttribute = GetAttributeOfType<CustomLogLevelAttribute, Enum_LogLevel_Local>(eLogLevel);
                if (pAttribute != null)
                {
                    if (_mapLogLevel_GroupList.ContainsKey(pAttribute.p_iLogLevelGroup_HashCode) == false)
                        _mapLogLevel_GroupList.Add(pAttribute.p_iLogLevelGroup_HashCode, new List<ELogLevel>());

                    _mapLogLevel_GroupList[pAttribute.p_iLogLevelGroup_HashCode].Add(pAttribute.p_eLogLevel);
                    _mapLogLevelGroup.Add(strLogLevel, pAttribute.p_iLogLevelGroup_HashCode);
                    _mapLogLevelConvert.Add(strLogLevel, pAttribute.p_eLogLevel);
                }
                else
                {
                    _mapLogLevelConvert.Add(strLogLevel, ELogLevel.CustomLine_Log);
                    _mapLogLevelGroup.Add(strLogLevel, -1);
                }
            }

            return strLogLevel;
        }


        // https://stackoverflow.com/questions/1799370/getting-attributes-of-enums-value
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static T GetAttributeOfType<T, EnumType>(EnumType enumVal) where T : System.Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        #endregion
    }
}
