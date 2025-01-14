using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace ImmersiveVRTools.Editor.Common.Utilities
{
    public static class BuildDefineSymbolManager
    {
        private const string ProductSymbolPrefix = "PRODUCT_";
        
        public static void SetBuildDefineSymbolState(string buildSymbol, bool isEnabled) =>
            SetBuildDefineSymbolState(buildSymbol, isEnabled, EditorUserBuildSettings.selectedBuildTargetGroup);
        public static void SetBuildDefineSymbolState(string buildSymbol, bool isEnabled, BuildTargetGroup buildTargetGroup)
        {
            var allBuildSymbols = GetAllBuildSymbols(buildTargetGroup);
            var isBuildSymbolDefined = allBuildSymbols.Any(s => s == buildSymbol);

            if (isEnabled && !isBuildSymbolDefined)
            {
                allBuildSymbols.Add(buildSymbol);
                SetBuildSymbols(allBuildSymbols, buildTargetGroup);
                UnityEngine.Debug.Log($"Build Symbol Added: {buildSymbol}");
            }

            if (!isEnabled && isBuildSymbolDefined)
            {
                allBuildSymbols.Remove(buildSymbol);
                SetBuildSymbols(allBuildSymbols, buildTargetGroup);
                UnityEngine.Debug.Log($"Build Symbol Removed: {buildSymbol}");
            }

            EditorUtility.ClearProgressBar();
        }
        
        public static void SetProductSymbol(string productNameSymbol) =>
            SetProductSymbol(productNameSymbol, EditorUserBuildSettings.selectedBuildTargetGroup);
        
        public static void SetProductSymbol(string productNameSymbol, BuildTargetGroup buildTargetGroup)
        {
            if (!productNameSymbol.StartsWith(ProductSymbolPrefix))
            {
                throw new Exception($"Product name symbol needs to start with: '{ProductSymbolPrefix}'");
            }
            
            var allBuildSymbols = GetAllBuildSymbols(buildTargetGroup);
            var allProductSymbols = allBuildSymbols.Where(s => s.StartsWith(ProductSymbolPrefix)).ToList();
            
            var productSymbolsToRemove = allProductSymbols.Where(s => s != productNameSymbol);
            foreach (var productSymbolToRemove in productSymbolsToRemove)
            {
                SetBuildDefineSymbolState(productSymbolToRemove, false, buildTargetGroup);
            }

            if (!allBuildSymbols.Contains(productNameSymbol))
            {
                SetBuildDefineSymbolState(productNameSymbol, true, buildTargetGroup);
            }
        }


        private static List<string> GetAllBuildSymbols(BuildTargetGroup buildTargetGroup)
        {
            return PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)
                .Split(';').ToList();
        }

        private static void SetBuildSymbols(List<string> allBuildSymbols, BuildTargetGroup buildTargetGroup)
        {
            EditorUtility.DisplayProgressBar("Please wait", "Modifying build symbols", 0.1f);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                buildTargetGroup,
                string.Join(";", allBuildSymbols.ToArray())
            );
        }
    }

}
