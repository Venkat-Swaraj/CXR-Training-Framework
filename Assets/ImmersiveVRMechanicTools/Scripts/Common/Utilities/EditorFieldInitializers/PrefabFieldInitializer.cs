using System.Linq;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class PrefabFieldInitializer
    {
        public static bool TrySetIfNotAssigned(GameObject prefab, System.Action<GameObject> setPrefab, string prefabName)
        {
            return EditorFieldInitializerGeneric<GameObject>.TrySetIfNotAssigned(prefab, setPrefab, prefabName, "Prefab");
        }
    }
}