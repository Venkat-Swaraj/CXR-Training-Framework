using System.Reflection;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

[DefaultExecutionOrder(CustomScriptExecutionOrderPriority.Highest)]
public class DynamicApplicationConfigInitializer : MonoBehaviour
{
    void Awake()
    {
        var allConfigTypes = ReflectionHelper.GetAllInstantiableTypesDerivedFrom(typeof(ApplicationConfigBase<>));
        foreach (var configType in allConfigTypes)
        {
            var getConfigMethod = configType.GetMethod("Get", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            getConfigMethod.Invoke(null, new[] {(MonoBehaviour) this, null});
        }
    }
}
