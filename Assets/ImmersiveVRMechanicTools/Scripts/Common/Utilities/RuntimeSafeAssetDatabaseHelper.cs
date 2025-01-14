using System.Collections.Generic;
using System.Linq;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class RuntimeSafeAssetDatabaseHelper
    {

        public static List<T> GetAllScriptableObjects<T>() where T : UnityEngine.ScriptableObject
        {
#if UNITY_EDITOR
            var guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name); //FindAssets uses tags check documentation for more info
            return guids.Select(UnityEditor.AssetDatabase.GUIDToAssetPath).Select(UnityEditor.AssetDatabase.LoadAssetAtPath<T>).ToList();
#else
            return null;
#endif
        }
        
        public static T GetAssetOrSubAsset<T>(string name) where T : UnityEngine.Object  
        {
#if UNITY_EDITOR
            if (TryGetAssetPath<T>(name, out var assetPath))
            {
                var assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(assetPath);
                if (assets.Length == 0) return null;
                if (assets.Length == 1) return (T) assets[0];
                else
                {
                    return (T)assets.FirstOrDefault(a => a.name == name);
                }
            }

            return null;
#else
            return null;
#endif
        }

        public static bool TryGetAssetPath<T>(string name, out string assetPath) where T : class
        {
#if UNITY_EDITOR
            var found = UnityEditor.AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}");
            if (found.Length == 0)
            {
                UnityEngine.Debug.LogWarning($"Unable to find new input system action for {name} please update input profile.");
                assetPath = string.Empty;
                return false;
            }

            if (found.Length > 1)
            {
                UnityEngine.Debug.LogWarning($"Multiple new input system actions found for {name} using first.");
            }

            assetPath = UnityEditor.AssetDatabase.GUIDToAssetPath(found.First());
            return true;
#else
            assetPath = string.Empty;
            return false;
#endif
        }
    }
}