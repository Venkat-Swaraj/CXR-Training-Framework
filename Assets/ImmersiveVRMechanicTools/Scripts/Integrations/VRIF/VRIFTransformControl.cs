#if INTEGRATIONS_VRIF

using System.Reflection;
using BNG;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(VRIFAdjustTransformControllerOnToGrabAndRelease))]
public class VRIFTransformControl : XRFrameworkTransformControl
{
    [SerializeField] private Grabbable Grabbable;
    [SerializeField] private GuidedSnapEnabledElement GuidedSnapEnabledElement;
    [SerializeField] private VRIFAdjustTransformControllerOnToGrabAndRelease _vRIFAdjustTransformControllerOnToGrabAndRelease;
    
    private bool _originalRemoteGrabbing;

    public bool IsFrameworkControllingTransform { get; private set; } = true;

    public static FieldInfo RemoteGrabbingField = typeof(Grabbable).GetField("remoteGrabbing", BindingFlags.Instance | BindingFlags.NonPublic);

    public override void TakeControlFromXrFramework()
    {
        _originalRemoteGrabbing = (bool)RemoteGrabbingField.GetValue(Grabbable);
        RemoteGrabbingField.SetValue(Grabbable, false);
        
        Grabbable.BeingHeld = false;

        IsFrameworkControllingTransform = false;
    }

    public override void PassControlBackToXrFramework()
    {
        RemoteGrabbingField.SetValue(Grabbable, _originalRemoteGrabbing);
        Grabbable.BeingHeld = true;

        IsFrameworkControllingTransform = true;
    }

    public void TriggerUnsnap()
    {
        GuidedSnapEnabledElement.Unsnap();
    }

    void Reset()
    {
        TryResolveDepedencies();
    }

    void Start()
    {
        TryResolveDepedencies();
        Grabbable.ParentToHands = false;
    }

    private void TryResolveDepedencies()
    {
        if (Grabbable == null) Grabbable = GetComponent<Grabbable>();
        if (GuidedSnapEnabledElement == null) GuidedSnapEnabledElement = GetComponent<GuidedSnapEnabledElement>();
        if (_vRIFAdjustTransformControllerOnToGrabAndRelease) _vRIFAdjustTransformControllerOnToGrabAndRelease = GetComponent<VRIFAdjustTransformControllerOnToGrabAndRelease>();
    }
}

#endif