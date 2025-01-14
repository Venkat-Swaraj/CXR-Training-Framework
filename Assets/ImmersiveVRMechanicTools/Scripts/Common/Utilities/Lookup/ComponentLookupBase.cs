using System;
using System.Linq;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using ImmersiveVRTools.Runtime.Common.Variable;
using UnityEngine;
using UnityEngine.Serialization;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public abstract class ComponentLookupBase<T>: MonoBehaviour
        where T: Component
    {
        [SerializeField] private LookupType _lookupType;

        public bool IsUseValueVisible => _lookupType == LookupType.Direct;
        [SerializeField] [ShowIf(nameof(IsUseValueVisible))] private T _useValue;
        
        public bool IsFindNameVisible => _lookupType == LookupType.ByName;
        // [ShowIf(nameof(IsFindNameVisible))] //TODO: ShowIf for custom types do not work
        [SerializeField] private StringReference _findName;
        [FormerlySerializedAs("_findNameMaxResolutionTimeBeforeGivingUp")] [SerializeField] [ShowIf(nameof(IsFindNameVisible))] private float _maxResolutionTimeBeforeGivingUp = 3f;

        [SerializeField] private bool _requireSpecificScene;
        [SerializeField] private StringReference _sceneName;
        
        public bool IsByReferenceVisibleVisible => _lookupType == LookupType.ByFeatureReference;
        // [ShowIf(nameof(IsByReferenceVisibleVisible))] //TODO: ShowIf for custom types do not work
        [SerializeField] [ReferenceOptions(ForceVariableOnly = true)] private FeatureReference _findFeatureReference;
        
        
        private float _firstResolutionTime;
        private T _cachedFound;
        private bool _isResolutionByNameAbandoned;

        public T Resolve() => Resolve(false);
        
        public T Resolve(bool resetCache)
        {
            switch (_lookupType)
            {
                case LookupType.Direct:
                    return _useValue;

                case LookupType.ByFeatureReference:
                case LookupType.ByName:
                    return GetCachedOrFind(resetCache);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private T GetCachedOrFind(bool resetCache)
        {
            if (_cachedFound && !resetCache)
            {
                return _cachedFound;
            }

            if (_isResolutionByNameAbandoned && !resetCache)
            {
                return null;
            }

            if (_firstResolutionTime == 0)
            {
                _firstResolutionTime = Time.realtimeSinceStartup;
            }

            if (resetCache || (!_cachedFound &&
                               Time.realtimeSinceStartup - _firstResolutionTime < _maxResolutionTimeBeforeGivingUp))
            {
                if (_requireSpecificScene)
                {
                    switch (_lookupType)
                    {
                        case LookupType.Direct:
                            return _useValue;
                        case LookupType.ByName:
                            _cachedFound = Resources.FindObjectsOfTypeAll<GameObject>()
                                .Where(go => go.name == _findName.Value)
                                .FirstOrDefault(go => go.scene.name == _sceneName.Value)?.GetComponent<T>();
                            break;
                        case LookupType.ByFeatureReference:
                            _cachedFound = Resources.FindObjectsOfTypeAll<FeatureReferenceComponent>()
                                .Where(fr => fr.FeatureReference.Variable == _findFeatureReference.Variable)
                                .FirstOrDefault(fe => fe.gameObject.scene.name == _sceneName.Value)?.GetComponent<T>();
                            
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_lookupType), _lookupType, null);
                    }
                }
                else
                {
                    switch (_lookupType)
                    {
                        case LookupType.Direct:
                            return _useValue;
                        case LookupType.ByName:
                            _cachedFound = GameObject.Find(_findName.Value)?.GetComponent<T>();
                            break;
                        case LookupType.ByFeatureReference:
#if UNITY_2019
                            _cachedFound = GameObject.FindObjectsOfType<FeatureReferenceComponent>()
#else
                            _cachedFound = GameObject.FindObjectsOfType<FeatureReferenceComponent>(true)
#endif
                                .FirstOrDefault(fr => fr.FeatureReference.Variable == _findFeatureReference.Variable)
                                ?.GetComponent<T>();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(_lookupType), _lookupType, null);
                    }
                }
            }
            else
            {
                _isResolutionByNameAbandoned = true;
                UnityEngine.Debug.LogWarning(
                    $"Unable to resolve transform by {_lookupType}: {_firstResolutionTime}, giving up..");
            }

            return _cachedFound;
        }

        public void RequireSpecificScene(string sceneName)
        {
            _requireSpecificScene = true;
            _sceneName.UseConstant = true;
            _sceneName.ConstantValue = sceneName;
        }
    }
    
    public enum LookupType
    {
        Direct,
        ByName,
        ByFeatureReference
    }
}