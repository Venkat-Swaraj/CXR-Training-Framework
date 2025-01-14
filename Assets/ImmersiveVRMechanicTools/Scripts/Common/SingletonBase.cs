using System;
using ImmersiveVrToolsCommon.Runtime.Logging;
using UnityEngine;

public class SingletonBase<T> : MonoBehaviour, ISingleton
    where T: MonoBehaviour, ISingleton
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = InitSingleton();
            }

            try
            {
                if (!_instance.gameObject.activeInHierarchy)
                {
                    Debug.Log($"{nameof(T)} is disabled, enabling");
                    _instance.gameObject.SetActive(true);
                }
            }
#pragma warning disable CS0168
            catch (Exception e)
#pragma warning restore CS0168
            {
#if ImmersiveVrTools_VerboseErrors
                Debug.LogWarning("Unable to access instance game object, this is usually caused when accessing via non-main thread");    
#endif
            }
            
            return _instance ?? (_instance = InitSingleton());
        }
    }
    
    private static T InitSingleton()
    {
        if (_instance != null)
        {
            Debug.LogWarning($"Too many {nameof(T)} - deleting this instance: '{_instance.name}'");
            _instance.enabled = false;
            DestroyImmediate(_instance);
            return null;
        }
#if UNITY_2019
        _instance = FindObjectOfType<T>();
#else
        _instance = FindObjectOfType<T>(true);
#endif

        if (!_instance)
        {
            LoggerScoped.LogDebug($"Singleton ({typeof(T).Name}), not found - auto creating");
            var go = new GameObject(typeof(T).Name);
            _instance = go.AddComponent<T>();
        }
        
        return _instance;
    }

    public T EnsureInitialized()
    {
        return Instance;
    }
}

public interface ISingleton
{
    
}