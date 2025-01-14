using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
    public static class TransformExtensions
    {
        public static Transform FindChildRecursive(this Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name.Contains(name))
                    return child;

                var result = child.FindChildRecursive(name);
                if (result != null)
                    return result;
            }
            return null;
        }
        
        public static void RemoveAllChildren(this Transform transform)
        {
            foreach (Transform child in transform) {
                GameObject.Destroy(child.gameObject);
            }
        }
        
        public static void RemoveAllChildrenExcept(this Transform transform, List<Transform> except)
        {
            foreach (Transform child in transform) {
                if (except.Contains(child))
                {
                    continue;
                }
                
                GameObject.Destroy(child.gameObject);
            }
        }
        
        public static IEnumerable<Transform> GetAllChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                yield return child;
            }
        }
    }
}