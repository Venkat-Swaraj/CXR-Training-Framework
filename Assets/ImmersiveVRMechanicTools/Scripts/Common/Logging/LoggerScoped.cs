using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ImmersiveVrToolsCommon.Runtime.Logging
{
    public static class LoggerScoped
    {
        private const string DebugColor = "#969595";

        public static string LogPrefix = "<NeedsSetting>: ";
        /*
     *  Init like so
     *
     *  #if UNITY_EDITOR
     *      [UnityEditor.InitializeOnLoad]
     *  #endif
     *      public static class LoggerScopedInitializer
     *      {
     *          static LoggerScopedInitializer()
     *          {
     *              Init();
     *          }
     *          
     *          [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
     *          static void Init()
     *          {
     *              LoggerScoped.LogPrefix = "FSR: ";
     *          }
     *      }
     */

        [Conditional("ImmersiveVrTools_DebugEnabled")]
        public static void LogDebug(string message)
        {
            Debug.Log($"<color={DebugColor}>{LogPrefix}{message}</color>");
        }
    
        [Conditional("ImmersiveVrTools_DebugEnabled")]
        public static void LogDebug(string message, Object context)
        {
            Debug.Log($"<color={DebugColor}>{LogPrefix}{message}</color>");
        }

        private static void LogInternal(LogType logType, object message, Object context)
        {
            Debug.unityLogger.Log(logType, string.Empty, LogPrefix + message, context);
        }
    
        private static void LogInternal(LogType logType, object message)
        {
            Debug.unityLogger.Log(logType, LogPrefix + message);
        }

        public static void Log(object message) => LogInternal(LogType.Log, message);
        public static void Log(object message, Object context) => LogInternal(LogType.Log, message, context);
        public static void LogError(object message) => LogInternal(LogType.Error, message);
        public static void LogError(object message, Object context) => LogInternal(LogType.Error, message, context);
        public static void LogWarning(object message) => LogInternal(LogType.Warning, message);
        public static void LogWarning(object message, Object context) => LogInternal(LogType.Warning, message, context);
    }
}