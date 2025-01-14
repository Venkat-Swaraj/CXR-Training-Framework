namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class ScriptableObjectReferenceFieldInitializer
    {
        public static bool TrySetIfNotAssigned<T, VariableT>(Reference<T, VariableT> so, System.Action<VariableT> setSo, string soName)
            where VariableT : ImmersiveVRTools.Runtime.Common.Variable.Variable<T>
        {
            return EditorFieldInitializerGeneric<VariableT>.TrySetIfNotAssigned(so.Variable, setSo, soName, typeof(VariableT).Name);
        }
    }
}