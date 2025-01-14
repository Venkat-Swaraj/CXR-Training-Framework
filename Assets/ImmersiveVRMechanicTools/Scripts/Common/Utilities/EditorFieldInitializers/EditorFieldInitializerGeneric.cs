using System.Linq;

namespace ImmersiveVRTools.Runtime.Common.Utilities {
    public class EditorFieldInitializerGeneric<T> where T: UnityEngine.Object
    {
        public static bool TrySetIfNotAssigned(object obj, System.Action<T> setObject, string name, string typeFilter)
        {
#if UNITY_EDITOR
            if (obj == null)
            {
                var objectsFound = UnityEditor.AssetDatabase.FindAssets($"{name} t:{typeFilter}");
                if (objectsFound.Length == 0)
                {
                    UnityEngine.Debug.LogWarning($"No '{typeFilter}' with name: '{name}' found, you have to add manually");
                    return false;
                }

                if (objectsFound.Length > 1)
                {
                    UnityEngine.Debug.LogWarning($"Multiple '{typeFilter}' with name: '{name}' found, using first");
                }

                setObject(UnityEditor.AssetDatabase.LoadAssetAtPath<T>(UnityEditor.AssetDatabase.GUIDToAssetPath(objectsFound.First())));
                return true;

            }
#endif
            return false;
        }
    }
}