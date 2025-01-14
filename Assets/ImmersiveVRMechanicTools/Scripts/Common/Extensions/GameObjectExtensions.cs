using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
    public static class GameObjectExtensions
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T comp = go.GetComponent<T>();
            if (!comp)
                comp = go.AddComponent<T>();
            return comp;
        }

        public static void SetChildrenActive(this GameObject root, bool isActive)
        {
            for (int i = 0; i < root.transform.childCount; i++)
            {
                root.transform.GetChild(i).gameObject.SetActive(isActive);
            }
        }
    }
}