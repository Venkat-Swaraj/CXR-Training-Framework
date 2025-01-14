using System;
using System.Collections.Generic;
using ImmersiveVRTools.Runtime.Common;
using ImmersiveVRTools.Runtime.Common.Extensions;
using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using UnityEngine;

public class RotationProgressElement : GuidedSnapEnabledElement
{
#pragma warning disable 649
    [Header("Rotation Progress")]
    [SerializeField] private int RotationAnglesToFinish = 180;
    [SerializeField] [Range(0, 1)] private float RequireCorrectRotationForceFromRotationProgress = 0.9f;
    [SerializeField] private int RequiredToolRotationForce = 30;
    [SerializeField] private RotationProgressLengthDeterminedByMode _rotationProgressLengthDeterminedBy = RotationProgressLengthDeterminedByMode.RotationProgressElement;
    [ShowIf(nameof(ShowRotationProgressEndPointInEditor))] [SerializeField] private Transform _rotationProgressEndPoint;
    [SerializeField] [Range(0, 1)] private float _finalRotationStageDampProgressMultiplier = 0.3f;
    [SerializeField] private float MaxOverRotation = 0.2f;

    [Header("Filter")]
    [SerializeField] [Tag] private List<string> _limitAttachToRotationToolsWithTag;

    [Header("No Tools Rotation")] //TODO: document, experimental - useful if you need to say unscrew by hand. Use at your own risk, no official support
    [SerializeField] private bool _allowAddingRotationProgressWithNoTools = false;

    [SerializeField] private Vector3Axis _noToolRotationProgressAccumulationDriverAxis = Vector3Axis.Z;
    private float _noToolLastTransformPositionRotationAngleOnProgressAccumulatingAxis;
    
    [Header("Events")]
    public RotationTool.RotationChangeEvent RotationChanged = new RotationTool.RotationChangeEvent();
    public RotationTool.RotationStartedEvent RotationStarted = new RotationTool.RotationStartedEvent();
    public RotationTool.RotationStoppedEvent RotationStopped = new RotationTool.RotationStoppedEvent();

    [Header("Debug")]
    [ShowIf(nameof(IsDebug))] [SerializeField] [ReadOnly] private float _rotationProgress;
#pragma warning restore 649

    public List<string> LimitAttachToRotationToolsWithTag => _limitAttachToRotationToolsWithTag;
    public float FinalRotationStageDampProgressMultiplier => _finalRotationStageDampProgressMultiplier;
    public RotationProgressLengthDeterminedByMode RotationProgressLengthDeterminedBy => _rotationProgressLengthDeterminedBy;
    public Transform RotationProgressEndPoint => _rotationProgressEndPoint;
    public float RotationProgress  {  get => _rotationProgress; private set => _rotationProgress = value; }

    public float RotationProgressLength
    {
        get
        {
            switch (RotationProgressLengthDeterminedBy)
            {
                case RotationProgressLengthDeterminedByMode.RotationProgressElement:
                    return Vector3.Distance(SnapRaycastOrigin.position, RotationProgressEndPoint.position);
                    
                case RotationProgressLengthDeterminedByMode.SnapTarget:
                    if (_currentlySnappedToTarget == null) return 0;
                    return Vector3.Distance(_currentlySnappedToTarget.TargetOrigin.position, _currentlySnappedToTarget.RotationProgressEndPoint.position);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private bool ShowRotationProgressEndPointInEditor => RotationProgressLengthDeterminedBy == RotationProgressLengthDeterminedByMode.RotationProgressElement;

    public bool AllowAddingRotationProgressWithNoTools => _allowAddingRotationProgressWithNoTools;

    protected override void Start()
    {
        base.Start();

        if (_allowAddingRotationProgressWithNoTools)
        {
            ElementDriverUnregistered += (s1, a1) =>
            {
                RotationStopped?.Invoke();
            };

            Unsnapped += (s2, a2) =>
            {
                RotationStopped?.Invoke();
            };
        }

        SnappedInFinishPosition += (sender, e) =>
        {
            if (_allowAddingRotationProgressWithNoTools)
            {
                //this will update local value so next time rotation is worked out there's a good baseline
                GetNoToolRotationChangeSinceLastCall();
                
                RotationStarted?.Invoke();
            }
        };
    }
    public AddRotationProgressStatus AddRotationProgress(float movedAngles, int toolRotationForce)
    {
        var newRotationProgress = RotationProgress + GetRotationProgress(movedAngles);
        var clampedNewRotationProgress = Mathf.Clamp(newRotationProgress, -0.1f, 1f + MaxOverRotation);

        if (RotationProgress >= RequireCorrectRotationForceFromRotationProgress)
        {
            if (toolRotationForce < RequiredToolRotationForce)
            {
                return AddRotationProgressStatus.NotEnoughToolRotationForce; //not enough force, don't apply progress
            }
        }

        if (newRotationProgress > RequireCorrectRotationForceFromRotationProgress && newRotationProgress < 1f)
        {
            RotationProgress += GetRotationProgress(movedAngles) * FinalRotationStageDampProgressMultiplier;
            return AddRotationProgressStatus.FinalRotationStage;
        }

        RotationProgress = clampedNewRotationProgress;
        if (newRotationProgress > 1f + MaxOverRotation) return AddRotationProgressStatus.OverRotatingBreakingPoint;
        if (RotationProgress > 1f) return AddRotationProgressStatus.OverRotating;
        if (RotationProgress < 0f) return AddRotationProgressStatus.UnderRotating;

        return AddRotationProgressStatus.Added;
    }

    public void TriggerRotationChangedEvent(RotationChanged eventArgs)
    {
        RotationChanged?.Invoke(eventArgs);
    }

    public void TriggerRotationStartedEvent()
    {
        RotationStarted?.Invoke();
    }

    public void TriggerRotationStoppedEvent()
    {
        RotationStopped?.Invoke();
    }

    public override void SetTransformValuesForLockedPosition()
    {
        base.SetTransformValuesForLockedPosition();
        transform.SetRotation(RotationWhenLocked);
    }

    public void UpdateLockedAtPositionBasedOnProgress(float? newRotationProgress = null)
    {
        if (newRotationProgress.HasValue)
            RotationProgress = newRotationProgress.Value;

        var moveBy = Mathf.Lerp(0, RotationProgressLength, Mathf.Min(1f, RotationProgress));
        LockedAtPosition = _initialLockedAtPosition + RaycastDirection * moveBy;
    }

    public float GetRotationProgress(float movedAngles)
    {
        return movedAngles / RotationAnglesToFinish;
    }

    public override bool ShouldPreventDetach(float detachNotAllowedIfRotationProgressMoreThan)
    {
        return RotationProgress > detachNotAllowedIfRotationProgressMoreThan;
    }

    public override bool IsConsideredTool { get; } = false;

    public float GetNoToolRotationChangeSinceLastCall()
    {
        var currentAngleOnProgressAccumulatingAxis = TransformPositionDriver.transform.rotation.eulerAngles[(int)_noToolRotationProgressAccumulationDriverAxis];
        var rotationChangeAngle = Mathf.DeltaAngle(currentAngleOnProgressAccumulatingAxis ,_noToolLastTransformPositionRotationAngleOnProgressAccumulatingAxis);
        _noToolLastTransformPositionRotationAngleOnProgressAccumulatingAxis = currentAngleOnProgressAccumulatingAxis;

        return rotationChangeAngle;
    }

    protected override void Reset()
    {
        base.Reset();

        TrySetRotationProgressEndPoint();
    }

    [ContextMenu(nameof(TrySetRotationProgressEndPoint))]
    private void TrySetRotationProgressEndPoint()
    {
        if (RotationProgressEndPoint == null)
        {
            var go = new GameObject() { name = "RotationProgressEndPoint" };
            _rotationProgressEndPoint = go.transform;
            RotationProgressEndPoint.SetParent(transform, false);
        }
    }
}

public enum AddRotationProgressStatus
{
    None,
    Added,
    NotEnoughToolRotationForce,
    FinalRotationStage,
    OverRotating,
    OverRotatingBreakingPoint,
    UnderRotating
}

public enum RotationProgressLengthDeterminedByMode
{
    RotationProgressElement,
    SnapTarget
}