using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JegaCore
{
    public static class JegaEditorExtensions
    {
        #region Platforms and Build Targets
#if UNITY_EDITOR
        public static BuildTargetGroup CurrentTargetGroup()
        {
            return BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
        }
#endif

#if UNITY_EDITOR
        public static IEnumerable<BuildTargetGroup> AllAvailableTargetGroups()
        {
            Array values = Enum.GetValues(typeof(BuildTarget));

            for (int i = 0; i < values.Length; i++)
            {
                BuildTarget value = (BuildTarget)values.GetValue(i);
                if (value == BuildTarget.NoTarget) continue;

                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(value);
                if (BuildPipeline.IsBuildTargetSupported(targetGroup, value))
                    yield return targetGroup;
            }
        }
#endif
        #endregion
        
        #region Directives
        [Conditional("UNITY_EDITOR")]
        public static void AddDirectiveToCurrentPlatform(string directive)
        {
#if UNITY_EDITOR
            List<string> activeDirectives = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            if (activeDirectives.Contains(directive)) return;
            
            activeDirectives.Add(directive);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentTargetGroup(), activeDirectives.ToArray());
#endif
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void RemoveDirectiveFromCurrentPlatform(string directive)
        {
#if UNITY_EDITOR
            List<string> activeDirectives = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            if (activeDirectives.Remove(directive) == false) return;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(CurrentTargetGroup(), activeDirectives.ToArray());
#endif
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void AddDirectiveToAllPlatforms(string directive)
        {
#if UNITY_EDITOR
            List<string> activeDirectives = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            if (activeDirectives.Contains(directive)) return;
            
            activeDirectives.Add(directive);
            foreach (BuildTargetGroup targetGroup in AllAvailableTargetGroups())
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, activeDirectives.ToArray());
#endif
        }
        
        [Conditional("UNITY_EDITOR")]
        public static void RemoveDirectiveFromAllPlatforms(string directive)
        {
#if UNITY_EDITOR
            List<string> activeDirectives = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(';').ToList();
            if (activeDirectives.Remove(directive) == false) return;

            foreach (BuildTargetGroup targetGroup in AllAvailableTargetGroups())
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, activeDirectives.ToArray());
#endif
        }
        #endregion
    }
}