using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class ScriptableObjectAwakeCalledReasonResolver
    {
        public static ScriptableObjectAwakeCalledReason ResolveAwakeCallReason(UnityEngine.ScriptableObject so)
        {
#if UNITY_EDITOR
            if(string.IsNullOrEmpty(UnityEditor.AssetDatabase.GetAssetPath(so)))
            {
                return ScriptableObjectAwakeCalledReason.ObjectBeingCreated;
            }
#endif
            return ScriptableObjectAwakeCalledReason.ObjectStartedButNotCreated;
        }
    }
    
    public enum ScriptableObjectAwakeCalledReason {
        ObjectBeingCreated,
        ObjectStartedButNotCreated
    }
}