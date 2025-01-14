using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

[DefaultExecutionOrder(CustomScriptExecutionOrderPriority.High)]
public class DynamicConfigValueSetterFromFile : DynamicConfigValueBase
{
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.MonoScript _configTypeScript;
#endif
    [SerializeField] [HideInInspector] private string _configTypeName; 
    [SerializeField] private string _configKeyName;
    
    protected override void SetConfigValueOnObjectInternal(Action<object> setOnObject)
    {
        var getMethod = ReflectionHelper.GetType(_configTypeName).GetMethod(nameof(ApplicationConfigBase<object>.Get), BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        Action<dynamic> onSettingsResolved = (settings) =>
        {
            var settingsObject = (object) settings;
            var value = GetConfigValue(settingsObject);

            setOnObject(value);
        };

        getMethod.Invoke(null, new object[] {this, onSettingsResolved});
    }
    
    private object GetConfigValue(object settingsObject)
    {
        var configKeyField = GetConfigKeyFieldInfo();
        if (configKeyField != null)
        {
            return configKeyField.GetValue(settingsObject);
        }

        var configKeyProperty = GetConfigKeyPropertyInfo();
        if (configKeyProperty != null)
        {
            return configKeyProperty.GetValue(settingsObject);
        }

        throw new Exception("Unable to find field or property on config object");
    }

#pragma warning disable CS0108, CS0114
    private void OnValidate()
#pragma warning restore CS0108, CS0114
    {
#if UNITY_EDITOR
         _configTypeName = _configTypeScript.GetClass().FullName;
#endif
        if (!string.IsNullOrEmpty(_configTypeName) && !string.IsNullOrEmpty(_configKeyName))
        {
            var configKeyField = GetConfigKeyFieldInfo();
            if (configKeyField == null)
            {
                var configKeyProperty = GetConfigKeyPropertyInfo();

                if (configKeyProperty == null)
                {
                    throw new Exception($"Unable to find config key: {_configKeyName}, " +
                                        $"for config type '{_configTypeName}'. " +
                                        $"Make sure you're using public fields or public properties.");
                }
            }
        }

        base.OnValidate();
    }

    private FieldInfo GetConfigKeyFieldInfo()
    {
        return ReflectionHelper.GetType(_configTypeName).GetField(_configKeyName);
    }
    
    private PropertyInfo GetConfigKeyPropertyInfo()
    {
        return ReflectionHelper.GetType(_configTypeName).GetProperty(_configKeyName);
    }
}