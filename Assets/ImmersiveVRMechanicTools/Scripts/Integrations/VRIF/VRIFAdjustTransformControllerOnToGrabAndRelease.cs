#if INTEGRATIONS_VRIF

using BNG;
using UnityEngine;

[RequireComponent(typeof(VRIFTransformControl))]
public class VRIFAdjustTransformControllerOnToGrabAndRelease : GrabbableEvents
{
    [SerializeField] private VRIFTransformControl _vRIFTransformControl;
    
    private void Start()
    {
        _vRIFTransformControl = GetComponent<VRIFTransformControl>();
    }

    private void Reset()
    {
        _vRIFTransformControl = GetComponent<VRIFTransformControl>();
    }
    
    public override void OnRelease()
    {
        base.OnRelease();
        // if (!_vRIFTransformControl.IsFrameworkControllingTransform)
        // {
        //     _vRIFTransformControl.TriggerUnsnap();
        // }
    }
}

#endif