using System;
using System.Collections;
using ImmersiveVRTools.Runtime.Common;
using UnityEngine;

[DefaultExecutionOrder(CustomScriptExecutionOrderPriority.Highest)]
public class ConfigDependency : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _dependentComponent;
    [SerializeField] private bool _enableOnlyOnceConfigResolved;
    [SerializeField] private bool _disableWholeGameObject;
    
    void Awake()
    {
        if (!_dependentComponent)
        {
            Debug.LogWarning($"No {nameof(_dependentComponent)} for {nameof(ConfigDependency)}. (this may not be intentional when running on server)");
            return;
        }
         
        if (!ApplicationConfigBaseNonGeneric.IsInitialized)
        {
            ApplicationConfigBaseNonGeneric.SettingsInitialized += (sender, args) => HandleSettingsInitialized();
        }
        else
        {
            StartCoroutine(HandleSettingsInitializedDelayed());
        }
        
        if (_enableOnlyOnceConfigResolved)
        {
            _dependentComponent.enabled = false;
        }

        if (_disableWholeGameObject)
        {
            _dependentComponent.gameObject.SetActive(false);
        }
    }

    //even when settings are already initialized we still want to wait at least frame, this will give ConfigSetters chance to run and set what's needed
    private IEnumerator HandleSettingsInitializedDelayed()
    {
        yield return null;

        HandleSettingsInitialized();
    }

    private void HandleSettingsInitialized()
    {
        if (_enableOnlyOnceConfigResolved)
        {
            _dependentComponent.enabled = true;
        }
        
        if (_disableWholeGameObject)
        {
            _dependentComponent.gameObject.SetActive(true);
        }
    }
}
