using System;
using System.Collections;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

public abstract class ApplicationConfigBase<T> : ApplicationConfigBaseNonGeneric
{
    public const string ApplicationSettingsStreamingAssetsFilePath = @"Config/application-settings.json";
    
    private static T _instance;
    public static T Instance => _instance;
    protected static bool _isLoading;

    public static void Get(MonoBehaviour coroutineRunner, Action<T> onSettingsResolved)
        => coroutineRunner.StartCoroutine(GetCoroutine(coroutineRunner, onSettingsResolved));
    
    private static IEnumerator GetCoroutine(MonoBehaviour coroutineRunner, Action<T> onSettingsResolved)
    {
        do
        {
            yield return null;
        } while (_isLoading);

        if (_instance == null)
        {
            _isLoading = true;

            Debug.Log("ApplicationSettings: loading starts");
            var applicationSettingsCd = StreamingAssetsLoader.LoadJson<T>(ApplicationSettingsStreamingAssetsFilePath, coroutineRunner);
            yield return applicationSettingsCd.Coroutine;
            _instance = applicationSettingsCd.Result;
            IsInitialized = true;
            Debug.Log($"ApplicationSettings: loaded, result: {_instance}");

            try
            {
                InvokeSettingsInitializedEvent();
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during {nameof(SettingsInitialized)} event. ex: {e}");
            }
            
            _isLoading = false;
        }

        if (onSettingsResolved != null)
        {
            onSettingsResolved?.Invoke(_instance);
        }
    }
}

//TODO: this approach of optionally overriding is not fully clean, although it's a good compromise between refactoring lots of elements to get support in both prod and local-dev environments. Ideally we'd have some kind of DynamicConfigValueSetterFromCommandArgs, that'd read some local dev-fallback file
public class ApplicationConfigPropertyOptionsAttribute : Attribute
{
    public string OptionallyOverrideWithCommandLineArgumentValue { get; set; }
}

public class ApplicationConfigBaseNonGeneric {
    //static event for generic class will end up having multiple versions (one for each generic class created)
    public static event EventHandler<EventArgs> SettingsInitialized;
    public static bool IsInitialized { get; protected set; }

    protected static void InvokeSettingsInitializedEvent()
    {
        SettingsInitialized?.Invoke(null, EventArgs.Empty);
    }
}