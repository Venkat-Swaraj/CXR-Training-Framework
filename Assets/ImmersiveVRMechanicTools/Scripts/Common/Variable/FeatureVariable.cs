using System;

namespace ImmersiveVRTools.Runtime.Common.Variable
{
    /// <summary>
    /// StringReference Class.
    /// </summary>
    [Serializable]
    public class FeatureReference : Reference<string, FeatureVariable>
    {
        public FeatureReference(string Value) : base(Value) { }
        public FeatureReference() { }
    }

    /// <summary>
    /// StringVariable Class.
    /// </summary>
#if ScriptableObjectVariablesMenuCreationEnabled
[CreateAssetMenu]
#endif
    public class FeatureVariable : Variable<string> { }
}