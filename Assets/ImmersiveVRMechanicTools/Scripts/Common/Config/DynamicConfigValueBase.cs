using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

[DefaultExecutionOrder(CustomScriptExecutionOrderPriority.High)]
public abstract class DynamicConfigValueBase : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _setOnObject;
    [SerializeField] private string _setOnObjectFieldName;

    [SerializeField] private bool _disableComponentTillConfigValueSet;

    void Awake()
    {
        if (!_setOnObject)
        {
            Debug.LogWarning($"No {nameof(_setOnObject)} for {nameof(DynamicConfigValueBase)}. (this may not be intentional when running on server)");
            return;
        }
        
        if (_disableComponentTillConfigValueSet)
        {
            _setOnObject.enabled = false;
        }

        SetConfigValueOnObject();
    }

    protected abstract void SetConfigValueOnObjectInternal(Action<object> setOnObject);
    

    [ContextMenu(nameof(SetConfigValueOnObject))]
    protected void SetConfigValueOnObject()
    {
        SetConfigValueOnObjectInternal((configValue) =>
        {
            var setOnObjectType = _setOnObject.GetType();
            var setOnField = GetSetOnFieldInfo(setOnObjectType);

            setOnField.SetValue(_setOnObject, configValue);
            
            if (_disableComponentTillConfigValueSet)
            {
                _setOnObject.enabled = true;
            }
        });
    }

    protected virtual void OnValidate()
    {
        if (_setOnObject)
        {
            var setOnObjectType = _setOnObject.GetType();
            if (!string.IsNullOrEmpty(_setOnObjectFieldName))
            {
                var setOnField = GetSetOnFieldInfo(setOnObjectType);
                
                if (setOnField == null)
                {
                    throw new Exception($"Unable to find: {_setOnObjectFieldName}, " +
                                        $"for type '{setOnObjectType}'. " +
                                        $"Make sure you're using fields.");
                }
            }
        }
    }

    private FieldInfo GetSetOnFieldInfo(Type setOnObjectType)
    {
        return ReflectionHelper.GetFieldInfoIncludingBaseClasses(setOnObjectType, _setOnObjectFieldName,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
    }
}