using System;
using System.Collections.Generic;
using System.Linq;

namespace ImmersiveVRTools.Runtime.Common.ScriptableObject
{
    public class ScriptableObjectStateRoot
    {
        private readonly Dictionary<object, Dictionary<Type, ScriptableObjectState>> _ownerObjectToStateMap = new Dictionary<object, Dictionary<Type, ScriptableObjectState>>();
        
        public T Get<T>(object owner) where T: ScriptableObjectState, new()
        {
            AssertOwnerExists<T>(owner);
            
            if (!_ownerObjectToStateMap.ContainsKey(owner))
            {
                _ownerObjectToStateMap.Add(owner, new Dictionary<Type, ScriptableObjectState>());
            }

            var scriptableObjectStates = _ownerObjectToStateMap[owner];
            if (!scriptableObjectStates.ContainsKey(typeof(T)))
            {
                scriptableObjectStates.Add(typeof(T), new T());
            }
            
            return (T)scriptableObjectStates[typeof(T)];
        }
        
        public T GetGlobal<T>() where T: ScriptableObjectState, new()
        {
            return Get<T>(this);
        }
        
        public void Remove<T>(object owner) where T: ScriptableObjectState
        {
            if (!_ownerObjectToStateMap.ContainsKey(owner))
            {
                UnityEngine.Debug.LogWarning($"Unable to remove state object: {typeof(T).Name} for owner: {owner} as there's no states for that owner at all");
            }
            else
            {
                var scriptableObjectStates = _ownerObjectToStateMap[owner];
                if (!scriptableObjectStates.ContainsKey(typeof(T)))
                {
                    UnityEngine.Debug.LogWarning($"Unable to remove state object: {typeof(T).Name} for owner: {owner} as there's no state with specified type for that owner");
                }
                else
                {
                    scriptableObjectStates.Remove(typeof(T));
                    if (!scriptableObjectStates.Any())
                    {
                        _ownerObjectToStateMap.Remove(owner);
                    }
                }
            }
        }

        public void RemoveGlobal<T>() where T : ScriptableObjectState
        {
            Remove<T>(this);
        }
        
        private static void AssertOwnerExists<T>(object owner) where T : ScriptableObjectState, new()
        {
            if (owner == null)
                throw new ArgumentNullException(
                    "owner needs to be specified to retrieve state object from ScriptableObjectStateRoot");
        }
    }
}