using System;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using ImmersiveVRTools.Runtime.Common.SmoothTransformOperations;
using UnityEngine;

public class FollowObjectTargetSmooth : FollowObjectTarget
{
    [SerializeField] private float _speedPerSecond = 1f;
    [SerializeField] private bool _moveInstantlyWhenPositionDifferenceOverThreshold;
    [SerializeField] [ShowIf(nameof(_moveInstantlyWhenPositionDifferenceOverThreshold))] private float _positionDistanceThresholdToMoveInstantly = 20f;
    [SerializeField] private bool _isRotationSpeedSynchronizedWithPosition;
    private bool ShowRotateAnglesPerSecond => !_isRotationSpeedSynchronizedWithPosition;
    [SerializeField] [ShowIf(nameof(ShowRotateAnglesPerSecond))] private float _rotateAnglesPerSecond = 1;
    
    private float _lastPositionUpdateDuration;
    
    private TrackableCoroutine _existingMoveRoutine { get; set; }
    private TrackableCoroutine _existingRotationRoutine { get; set; }

    public float SpeedPerSecond
    {
        get => _speedPerSecond;
        set => _speedPerSecond = value;
    }

    protected override void UpdateTargetRotation(Quaternion targetRotation)
    {
        if (targetRotation != transform.rotation && _existingRotationRoutine  == null)
        {
            if (_isRotationSpeedSynchronizedWithPosition)
            {
                if (_lastPositionUpdateDuration == 0)
                {
                    transform.rotation = targetRotation;
                }
                else
                {
                    _existingRotationRoutine = TransformSmoothRotator.RotateOverSeconds(transform, GenerateRotationAdheringToLimitsSet(targetRotation), _lastPositionUpdateDuration);
                    _existingRotationRoutine.Finished += ClearExistingRotationRoutine;
                    _existingRotationRoutine.Start(StartCoroutine);
                }
            }
            else
            {
                _existingRotationRoutine = TransformSmoothRotator.RotateConstantSpeed(transform, GenerateRotationAdheringToLimitsSet(targetRotation), _rotateAnglesPerSecond);
                _existingRotationRoutine.Finished += ClearExistingRotationRoutine;
                _existingRotationRoutine.Start(StartCoroutine);
            }
        }
    }

    protected override void UpdateTargetPosition(Vector3 newPosition)
    {
        if (newPosition != transform.position && _existingMoveRoutine  == null)
        {
            if (_moveInstantlyWhenPositionDifferenceOverThreshold && Vector3.Distance(newPosition, transform.position) > _positionDistanceThresholdToMoveInstantly)
            {
                transform.position = newPosition;
                _lastPositionUpdateDuration = 0;
            }
            else
            {
                _existingMoveRoutine = TransformSmoothMover.MoveConstantSpeed(transform, newPosition, _speedPerSecond, out _lastPositionUpdateDuration);
                _existingMoveRoutine.Finished += ClearExistingMoveRoutine;
                _existingMoveRoutine.Start(StartCoroutine);
            }
        }
    }

    private void ClearExistingMoveRoutine(object sender, EventArgs e)
    {
        if (_existingMoveRoutine != null)
        {
            _existingMoveRoutine.Finished -= ClearExistingMoveRoutine;
            _existingMoveRoutine = null;
        }
    }
    
    private void ClearExistingRotationRoutine(object sender, EventArgs e)
    {
        if (_existingRotationRoutine != null)
        {
            _existingRotationRoutine.Finished -= ClearExistingRotationRoutine;
            _existingRotationRoutine = null;
        }
    }
}
