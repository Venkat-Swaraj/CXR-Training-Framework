using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using ImmersiveVRTools.Runtime.Common.Utilities;
using ImmersiveVRTools.Runtime.Common.Variable;
using UnityEngine;

[DefaultExecutionOrder(CustomScriptExecutionOrderPriority.High)]
public class DynamicConfigValueSetterFromCommandLine : DynamicConfigValueBase
{
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.MonoScript _commandLineOptionsType;
#endif
    [SerializeField] [HideInInspector] private string _commandLineOptionsTypeName; 
    [SerializeField] private string _commandLineOptionsClassPropertyName;
    
    [SerializeField] [ReadOnly] private string _commandLineArgName; //this will be auto resolved when editing in editor
    
    [ContextMenu(nameof(TriggerSetConfigValueOnObjectFresh))]
    private void TriggerSetConfigValueOnObjectFresh()
    {
        //for editor run we need to nullify singleton instances so they have a chance to rebuild
        ReflectionHelper.GetType(_commandLineOptionsTypeName).BaseType
            .GetField("_instance", BindingFlags.Static | BindingFlags.NonPublic)
            .SetValue(null, null);
        SetConfigValueOnObject();
    }
    
    protected override void SetConfigValueOnObjectInternal(Action<object> setOnObject)
    {
        var instance = ReflectionHelper.GetType(_commandLineOptionsTypeName)
            .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .GetValue(null);

        var value = instance.GetType().GetProperty(_commandLineOptionsClassPropertyName).GetValue(instance);
        setOnObject(value);
    }

#pragma warning disable CS0108, CS0114
    private void OnValidate()
#pragma warning restore CS0108, CS0114
    {
#if UNITY_EDITOR
        _commandLineOptionsTypeName = _commandLineOptionsType.GetClass().FullName;
#endif

        if (!string.IsNullOrEmpty(_commandLineOptionsTypeName) && !string.IsNullOrEmpty(_commandLineOptionsClassPropertyName))
        {
            var commandLineOptionsProp = ReflectionHelper.GetType(_commandLineOptionsTypeName).GetProperty(_commandLineOptionsClassPropertyName);
            if (commandLineOptionsProp == null)
            {
                throw new Exception($"Unable to find config prop: {_commandLineOptionsClassPropertyName}, " +
                                        $"for config type '{_commandLineOptionsTypeName}'. " +
                                        $"Make sure you're using public fields or public properties.");
            }
            
            var commandLineOptionsPropArgInfoAttribute = commandLineOptionsProp.GetCustomAttribute<DynamicConfigValueSetterCommandLineArgInfoAttribute>();
            if (commandLineOptionsPropArgInfoAttribute == null)
            {
                throw new Exception($"There's none {nameof(DynamicConfigValueSetterCommandLineArgInfoAttribute)} on: {_commandLineOptionsClassPropertyName}, " +
                                    $"for config type '{_commandLineOptionsTypeName}'. " +
                                    $"Make sure to add.");
            }

            _commandLineArgName = commandLineOptionsPropArgInfoAttribute.Name;
        }

        base.OnValidate();
    }
}

public class DynamicConfigValueSetterCommandLineArgInfoAttribute: Attribute
{
    public string Name { get; }

    public DynamicConfigValueSetterCommandLineArgInfoAttribute(string name)
    {
        Name = name;
    }
}