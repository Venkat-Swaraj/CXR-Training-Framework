using System;
using ImmersiveVRTools.Runtime.Common.Extensions;
using UnityEngine;
using ImmersiveVRTools.Runtime.Common.Utilities;

namespace ImmersiveVRTools.Runtime.Common
{
    public class FollowObjectTarget : MonoBehaviour
    {
        [SerializeField] private Transform Source;
        [SerializeField] private string SourceName;
        public Func<Vector3> GetPositionOffset;

        [SerializeField] private bool FollowPosition = true;
    
        [SerializeField] private Boolean3 _followRotation = new Boolean3(true);
        [SerializeField] private bool RelativeToInitialSourceRotation;

        [SerializeField] private float _sourceUpdateEveryNSeconds = 0;
        
        private Quaternion _initialSourceRotation;
        private Quaternion _initialTargetRotation;

        private float _secondsSinceLastSourceUpdate;

        public Boolean3 FollowRotation
        {
            get => _followRotation;
            set => _followRotation = value;
        }

        public void SetSource(Transform sourceTransform, Func<Vector3> getPostionOffset = null, bool  relativeToInitialSourceRotation = false)
        {
            Source = sourceTransform;
            GetPositionOffset = getPostionOffset;

            if (sourceTransform != null)
            {
                _initialSourceRotation = sourceTransform.rotation;
                _initialTargetRotation = transform.rotation;
            }

            RelativeToInitialSourceRotation = relativeToInitialSourceRotation;
        }

        [ContextMenu("Update")]
        void Update()
        {
            if(!isActiveAndEnabled) return;

            if (!Source && !string.IsNullOrEmpty(SourceName))
            {
                var foundSource = GameObject.Find(SourceName);
                if (foundSource)
                {
                    Source = foundSource.transform;
                }
            }
            
            if (Source != null && (_sourceUpdateEveryNSeconds == 0 || _secondsSinceLastSourceUpdate > _sourceUpdateEveryNSeconds))
            {
                if (FollowPosition)
                {
                    UpdateTargetPosition(Source.transform.position - (GetPositionOffset?.Invoke() ?? Vector3.zero));
                }
                
                if (_followRotation.AnyTrue())
                {
                    Quaternion targetRotation;
                    if (RelativeToInitialSourceRotation)
                    {
                        var sourceRotationDiffFromInitial = _initialSourceRotation * Quaternion.Inverse(Source.rotation);
                        var newTargetRotation = Quaternion.Inverse(sourceRotationDiffFromInitial) * _initialTargetRotation;
                        targetRotation = newTargetRotation;
                    }
                    else
                    {
                        targetRotation = Source.rotation;
                    }

                    UpdateTargetRotation(targetRotation);
                }

                _secondsSinceLastSourceUpdate = 0;
            }

            _secondsSinceLastSourceUpdate += Time.deltaTime;
        }
        
        protected virtual void UpdateTargetPosition(Vector3 newPosition)
        {
            transform.position = newPosition;
        }

        protected virtual void UpdateTargetRotation(Quaternion targetRotation)
        {
            transform.SetRotation(GenerateRotationAdheringToLimitsSet(targetRotation));
        }

        protected Quaternion GenerateRotationAdheringToLimitsSet(Quaternion targetRotation)
        {
            return Quaternion.Euler(
                _followRotation.x ? targetRotation.eulerAngles.x : 0f,
                _followRotation.y ? targetRotation.eulerAngles.y : 0f,
                _followRotation.z ? targetRotation.eulerAngles.z : 0f
            );
        }
    }
}
