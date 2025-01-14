namespace ImmersiveVRTools.Runtime.Common.Utilities {
    public class ScriptableObjectFieldInitializer
    {
        public static bool TrySetIfNotAssigned<TScriptableObject>(object so, System.Action<TScriptableObject> setSo, string soName)
            where TScriptableObject: UnityEngine.ScriptableObject
        {
            return EditorFieldInitializerGeneric<TScriptableObject>.TrySetIfNotAssigned(so, setSo, soName, typeof(TScriptableObject).Name);
        }
    }
}