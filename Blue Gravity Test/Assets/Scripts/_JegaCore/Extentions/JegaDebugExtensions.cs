using System.Diagnostics;

namespace JegaCore
{
    public static class JegaDebugExtensions
    {
        //These are shortcut methods to IzyDebug log calls; they immediatly send the 'this' reference as context as well.
        [Conditional(JegaDebug.ConditionString)]
        public static void LogVerbose(this UnityEngine.Object obj, object message) => JegaDebug.LogVerbose(message, obj);
        [Conditional(JegaDebug.ConditionString)]
        public static void Log(this UnityEngine.Object obj, object message) => JegaDebug.Log(message, obj);
        [Conditional(JegaDebug.ConditionString)]
        public static void LogWarning(this UnityEngine.Object obj, object message) => JegaDebug.LogWarning(message, obj);
        [Conditional(JegaDebug.ConditionString)]
        public static void LogError(this UnityEngine.Object obj, object message) => JegaDebug.LogError(message, obj);
    }
    
    public static class IzyLoggingLevelExtensions
    {
        public static bool HasFlagFast(this JegaLoggingLevel value, JegaLoggingLevel flag) => (value & flag) != 0;
    }
}