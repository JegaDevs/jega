using System.Diagnostics;
using UnityEngine;
#if UNITY_2021_1_OR_NEWER
using Application = UnityEngine.Device.Application;
#endif
using Object = UnityEngine.Object;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JegaCore
{
    public static class JegaDebug
    {
        public const string ConditionString = "JEGA_DEBUGLOG";
        private const JegaLoggingLevel DefaultLogLevel = (JegaLoggingLevel.Verbose | JegaLoggingLevel.Logs | JegaLoggingLevel.Warnings | JegaLoggingLevel.Errors);
        private const string DebugMessagePrefix = "[JEGA_DEBUG] ";
        
        private static JegaLoggingLevel loggingFlags = DefaultLogLevel;
        private static SavedBuildLogFlags savedBuildLogFlags;
#if UNITY_EDITOR
        private const string EditorPrefsKey = "JEGADEBUG_EDITOR_FLAGS";
#endif
        
        public static JegaLoggingLevel ActiveFlags => Application.isEditor ? LoggingFlags : BuildOnlyFlags;
        private static JegaLoggingLevel LoggingFlags
        {
            get
            {
#if UNITY_EDITOR
                loggingFlags = (JegaLoggingLevel)EditorPrefs.GetInt(EditorPrefsKey, (int)DefaultLogLevel);
#endif
                return loggingFlags;
            }
            set
            {
                loggingFlags = value;
#if UNITY_EDITOR
                EditorPrefs.SetInt(EditorPrefsKey, (int)loggingFlags);
#endif
            }
        }
        private static JegaLoggingLevel BuildOnlyFlags
        {
            get => SavedBuildLogFlags.savedLogLevel;
            set => SavedBuildLogFlags.savedLogLevel = value;
        }

        private static SavedBuildLogFlags SavedBuildLogFlags
        {
            get
            {
                if (savedBuildLogFlags == null)
                    savedBuildLogFlags = StaticPaths.LoadAssetFromCoreFolder<SavedBuildLogFlags>(nameof(SavedBuildLogFlags));

                if (savedBuildLogFlags == null)
                {
                    savedBuildLogFlags = ScriptableObject.CreateInstance<SavedBuildLogFlags>();
                    savedBuildLogFlags.savedLogLevel = DefaultLogLevel;
                    savedBuildLogFlags.name = nameof(SavedBuildLogFlags);
                    StaticPaths.CreateAssetInCoreFolderThenSave(savedBuildLogFlags);
                    #if UNITY_EDITOR && !JEGA_DEBUGLOG
                    EnableDirective(); //enable the directive when creating the asset, to help with first time project setup
                    #endif
                }
                return savedBuildLogFlags;
            }
        }
        
        private static void LogInternal(JegaLoggingLevel level, object message, Object context = null)
        {
            if (ActiveFlags.HasFlagFast(level) == false) return;
            
            string debugMessage = DebugMessagePrefix + message.ToString();

            switch (level)
            {
                case JegaLoggingLevel.Verbose:
                case JegaLoggingLevel.Logs:
                    Debug.Log(debugMessage, context);
                    break;
                case JegaLoggingLevel.Warnings:
                    Debug.LogWarning(debugMessage, context);
                    break;
                case JegaLoggingLevel.Errors:
                    Debug.LogError(debugMessage, context);
                    break;
                default:
                case JegaLoggingLevel.None:
                    break;
            }
        }
        
        [Conditional(ConditionString)]
        public static void LogVerbose(object message, Object context = null) => LogInternal(JegaLoggingLevel.Verbose, message, context);
        [Conditional(ConditionString)]
        public static void Log(object message, Object context = null) => LogInternal(JegaLoggingLevel.Logs, message, context);
        [Conditional(ConditionString)]
        public static void LogWarning(object message, Object context = null) => LogInternal(JegaLoggingLevel.Warnings, message, context);
        [Conditional(ConditionString)]
        public static void LogError(object message, Object context = null) => LogInternal(JegaLoggingLevel.Errors, message, context);

        //Unity Editor Context Menu Methods
#if UNITY_EDITOR
        
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            #if !JEGA_DEBUGLOG
            SavedBuildLogFlags loadFlags = SavedBuildLogFlags;
            #endif
        }
        
    #if !JEGA_DEBUGLOG
        private const string MenuPrefix = "JEGA/Logging Level/";
        private const string BuildMenuPrefix = "JEGA/Build Logging Level/";
        private const string VerboseMenu = nameof(JegaLoggingLevel.Verbose);
        private const string LogsMenu = nameof(JegaLoggingLevel.Logs);
        private const string WarningsMenu = nameof(JegaLoggingLevel.Warnings);
        private const string ErrorsMenu = nameof(JegaLoggingLevel.Errors);
        
        #region Logging Level Menus
        private static bool ToggleLogLevelValidate(JegaLoggingLevel level, string menuName)
        {
            bool isChecked = LoggingFlags.HasFlagFast(level);
            Menu.SetChecked(MenuPrefix + menuName, isChecked);
            return true;
        }
        
        [MenuItem(MenuPrefix + VerboseMenu)]
        private static void ToggleLogLevelVerbose() => LoggingFlags ^= JegaLoggingLevel.Verbose;
        [MenuItem(MenuPrefix + VerboseMenu, true)]
        private static bool ToggleLogLevelVerboseValidate() => ToggleLogLevelValidate(JegaLoggingLevel.Verbose, VerboseMenu);
        
        [MenuItem(MenuPrefix + LogsMenu)]
        public static void ToggleLogLevelLogs() => LoggingFlags ^= JegaLoggingLevel.Logs;
        [MenuItem(MenuPrefix + LogsMenu, true)]
        public static bool ToggleLogLevelLogsValidate() => ToggleLogLevelValidate(JegaLoggingLevel.Logs, LogsMenu);
        
        [MenuItem(MenuPrefix + WarningsMenu)]
        public static void ToggleLogLevelWarnings() => LoggingFlags ^= JegaLoggingLevel.Warnings;
        [MenuItem(MenuPrefix + WarningsMenu, true)]
        public static bool ToggleLogLevelWarningsValidate() => ToggleLogLevelValidate(JegaLoggingLevel.Warnings, WarningsMenu);
        
        [MenuItem(MenuPrefix + ErrorsMenu)]
        public static void ToggleLogLevelErrors() => LoggingFlags ^= JegaLoggingLevel.Errors;
        [MenuItem(MenuPrefix + ErrorsMenu, true)]
        public static bool ToggleLogLevelErrorsValidate() => ToggleLogLevelValidate(JegaLoggingLevel.Errors, ErrorsMenu);
        #endregion
        
        #region Build Log Level Menus
        private static bool ToggleBuildLogLevelValidate(JegaLoggingLevel level, string menuName)
        {
            bool isChecked = BuildOnlyFlags.HasFlagFast(level);
            Menu.SetChecked(BuildMenuPrefix + menuName, isChecked);
            return true;
        }
        
        [MenuItem(BuildMenuPrefix + VerboseMenu)]
        private static void ToggleBuildLogLevelVerbose() => BuildOnlyFlags ^= JegaLoggingLevel.Verbose;
        [MenuItem(BuildMenuPrefix + VerboseMenu, true)]
        private static bool ToggleBuildLogLevelVerboseValidate() => ToggleBuildLogLevelValidate(JegaLoggingLevel.Verbose, VerboseMenu);
        
        [MenuItem(BuildMenuPrefix + LogsMenu)]
        public static void ToggleBuildLogLevelLogs() => BuildOnlyFlags ^= JegaLoggingLevel.Logs;
        [MenuItem(BuildMenuPrefix + LogsMenu, true)]
        public static bool ToggleBuildLogLevelLogsValidate() => ToggleBuildLogLevelValidate(JegaLoggingLevel.Logs, LogsMenu);
        
        [MenuItem(BuildMenuPrefix + WarningsMenu)]
        public static void ToggleBuildLogLevelWarnings() => BuildOnlyFlags ^= JegaLoggingLevel.Warnings;
        [MenuItem(BuildMenuPrefix + WarningsMenu, true)]
        public static bool ToggleBuildLogLevelWarningsValidate() => ToggleBuildLogLevelValidate(JegaLoggingLevel.Warnings, WarningsMenu);
        
        [MenuItem(BuildMenuPrefix + ErrorsMenu)]
        public static void ToggleBuildLogLevelErrors() => BuildOnlyFlags ^= JegaLoggingLevel.Errors;
        [MenuItem(BuildMenuPrefix + ErrorsMenu, true)]
        public static bool ToggleBuildLogLevelErrorsValidate() => ToggleBuildLogLevelValidate(JegaLoggingLevel.Errors, ErrorsMenu);
        #endregion
    #endif

        #if !JEGA_DEBUGLOG
        [MenuItem("JEGA/Enable JegaDebug Directive", false, 1500)]
        public static void EnableDirective()
        {
            Debug.Log("Enabling JegaDebug directive.");
            JegaEditorExtensions.AddDirectiveToAllPlatforms(ConditionString);
        }
        #else
        [MenuItem("JEGA/Disable JegaDebug Directive", false, 1500)]
        public static void DisableDirective()
        {
            Debug.Log("Disabling JegaDebug directive.");
            JegaEditorExtensions.RemoveDirectiveFromAllPlatforms(ConditionString);
        }
        #endif
#endif
    }
}