using System;
using UnityEngine.Events;

namespace ImmersiveVRTools.Runtime.Common.Extensions
{
    public static class UnityEventExtensions
    {
        public static void SafeInvoke<T>(this UnityEvent<T> unityEvent, T args)
        {
            try
            {
                unityEvent?.Invoke(args);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"Exception when invoking event code for '{unityEvent?.GetType().Name}', {ex}");
                throw;
            }
        }
        
        public static void SafeInvoke(this UnityEvent unityEvent)
        {
            try
            {
                unityEvent?.Invoke();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"Exception when invoking event code for '{unityEvent?.GetType().Name}', {ex}");
                throw;
            }
        }
    }
}