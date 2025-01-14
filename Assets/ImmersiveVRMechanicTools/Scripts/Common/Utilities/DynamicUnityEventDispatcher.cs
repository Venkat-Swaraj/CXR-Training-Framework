using System;
using System.Reflection;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities 
{
    public class DynamicUnityEventDispatcher : MonoBehaviour, ITriggerable
    {
    #if UNITY_EDITOR
        [SerializeField] private UnityEditor.MonoScript _findComponentTypeScript;
    #endif
        [SerializeField] [ReadOnly] private string _findComponentTypeName; 
        [SerializeField] private DynamicUnityEventDispatcherFindMode _dynamicUnityEventDispatcherFindMode; 
        
        [SerializeField] private string _methodNameToTrigger;

        [SerializeField] private bool _includeDisabledGameObjects;

        public string FindComponentTypeName => _findComponentTypeName;

        public string MethodNameToTrigger => _methodNameToTrigger;

        private void Awake()
        {
            //ensure method exits
            FindMethodToTrigger(ResolveComponentType());
        }

        [ContextMenu(nameof(Trigger))]
        public void Trigger()
        {
            var componentType = ResolveComponentType();
            
            UnityEngine.Object component;
            switch (_dynamicUnityEventDispatcherFindMode)
            {
                case DynamicUnityEventDispatcherFindMode.FirstOfType:
    #if UNITY_2019
                    component = GameObject.FindObjectOfType(componentType);
    #else
                    component = GameObject.FindObjectOfType(componentType, _includeDisabledGameObjects);
    #endif

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (component == null)
            {
                throw new Exception($"Unable to find component for type '{FindComponentTypeName}' using mode: '{_dynamicUnityEventDispatcherFindMode}'");
            }

            var methodToTrigger = FindMethodToTrigger(componentType);

            methodToTrigger.Invoke(component, null);
        }

        public string TriggerName => $"{FindComponentTypeName}:{MethodNameToTrigger}";

        private MethodInfo FindMethodToTrigger(Type componentType)
        {
            var methodToTrigger = componentType.GetMethod(MethodNameToTrigger);
            if (methodToTrigger == null)
            {
                throw new Exception($"Unable to find method: {MethodNameToTrigger}, component type '{FindComponentTypeName}' using mode: '{_dynamicUnityEventDispatcherFindMode}'. Make sure it's public parameterless method.");
            }

            return methodToTrigger;
        }

        private void OnValidate()
        {
    #if UNITY_EDITOR
            _findComponentTypeName = _findComponentTypeScript.GetClass().FullName;
    #endif

            if (!string.IsNullOrEmpty(FindComponentTypeName) && !string.IsNullOrEmpty(MethodNameToTrigger))
            {
                var method = FindMethodToTrigger(ResolveComponentType());
            }
        }
        
        private Type ResolveComponentType()
        {
            return ReflectionHelper.GetType(FindComponentTypeName);
        }
    }

    public interface ITriggerable
    {
        void Trigger();
        string TriggerName { get; }
    }

    public enum DynamicUnityEventDispatcherFindMode
    {
        FirstOfType
    }
}
