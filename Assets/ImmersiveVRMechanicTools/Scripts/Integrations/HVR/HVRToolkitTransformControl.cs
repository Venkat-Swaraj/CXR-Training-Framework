#if INTEGRATIONS_HVR

using System;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HVRGrabbable))]
public class HVRToolkitTransformControl : XRFrameworkTransformControl
{
    [SerializeField] private HVRGrabbable _XRGrabInteractable;
    private HVRGrabberBase lastGrabber;

    public bool IsFrameworkControllingTransform { get; private set; } = true;

    void Start()
    {
        _XRGrabInteractable = GetComponent<HVRGrabbable>();
    }

    private void Reset()
    {
        _XRGrabInteractable = GetComponent<HVRGrabbable>();
    }

    public override void PassControlBackToXrFramework()
    {
        lastGrabber.TryGrab(_XRGrabInteractable, true);

        IsFrameworkControllingTransform = true;
    }

    public override void TakeControlFromXrFramework()
    {
        lastGrabber = _XRGrabInteractable.PrimaryGrabber;
        _XRGrabInteractable.ForceRelease();
        
        IsFrameworkControllingTransform = false;
    }   
}

#endif