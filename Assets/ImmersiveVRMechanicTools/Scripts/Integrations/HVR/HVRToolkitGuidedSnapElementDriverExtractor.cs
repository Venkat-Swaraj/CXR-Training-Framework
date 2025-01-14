#if INTEGRATIONS_HVR

using Assets.TorqueWrenchTools.Scripts.Utilities;
using HurricaneVR.Framework.Core;
using HurricaneVR.Framework.Core.Grabbers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class HVRToolkitGuidedSnapElementDriverExtractor : MonoBehaviour
{
    [Serializable] public class DriverExtractedEvent : UnityEngine.Events.UnityEvent<TransformPositionDriver> { }
    public DriverExtractedEvent DriverExtractedForGrab = new DriverExtractedEvent();
    public DriverExtractedEvent DriverExtractedForUngrab = new DriverExtractedEvent();

    [SerializeField] private float _triggerUngrabOnlyIfDistanceBetweenHandAndGrabbableGreaterThan = 0.2f;

    public void ExctractGuidedSnapElementDriverForGrab(HVRGrabberBase grabber, HVRGrabbable grabbable)
    {
        ExtractDriverAndProcessEvent(grabber, DriverExtractedForGrab);
    }

    public void ExctractGuidedSnapElementDriverForUngrab(HVRGrabberBase grabber, HVRGrabbable grabbable)
    {
        //unhover event fires quite often, also on grab start, make sure item is actually away from grabber to proceed
        if (Vector3.Distance(grabber.transform.position, grabbable.transform.position) >
            _triggerUngrabOnlyIfDistanceBetweenHandAndGrabbableGreaterThan)
        {
            ExtractDriverAndProcessEvent(grabber, DriverExtractedForUngrab);
        }
    }

    private void ExtractDriverAndProcessEvent(HVRGrabberBase xrBaseInteractor, DriverExtractedEvent driverExtractedEvent)
    {
        var driver = xrBaseInteractor.GetComponent<TransformPositionDriver>();
        if (driver != null)
        {
            driverExtractedEvent?.Invoke(driver);
        }
    }

    void Reset()
    {
#if UNITY_EDITOR
        SetUpEventCommunication();
#endif
    }

    void Start()
    {
#if UNITY_EDITOR
        var isAnyAdded = SetUpEventCommunication();
        if (isAnyAdded)
        {
            Debug.LogWarning($"Events for {nameof(HVRToolkitGuidedSnapElementDriverExtractor)} were added automatically, this will only happen in Editor." +
                             $"Please click on this message and set up event communication. You can right click on script and use '{nameof(SetUpEventCommunication)}' context menu option to do that." +
                             $"If you don't set event communication it'll fail at runtime", gameObject);
        }
#endif
    }

#if UNITY_EDITOR
    [ContextMenu(nameof(SetUpEventCommunication))]
    private bool SetUpEventCommunication()
    {
        var grabInteractable = GetComponent<HVRGrabbable>();
        
        var isOnSelectEnterAdded = AddPersistentListenerIfNotExists(grabInteractable.HoverEnter, ExctractGuidedSnapElementDriverForGrab, this);
        var isOnSelectEnterExitAdded = AddPersistentListenerIfNotExists(grabInteractable.HoverExit, ExctractGuidedSnapElementDriverForUngrab, this);


        var guidedSnapEnabledElement = GetComponent<GuidedSnapEnabledElement>();
        var isDriverExtractedForGrabAdded = UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForGrab, guidedSnapEnabledElement.RegisterElementDriver, guidedSnapEnabledElement);
        var isDriverExtractedForUngrabAdded = UnityEventToolsHelper.AddPersistentListenerIfNotExists(DriverExtractedForUngrab, guidedSnapEnabledElement.UnregisterElementDriver, guidedSnapEnabledElement);

        return isOnSelectEnterAdded || isOnSelectEnterExitAdded || isDriverExtractedForGrabAdded || isDriverExtractedForUngrabAdded;
    }
    
    //helper methods added as types with more than one generic param not supported 
    private static bool AddPersistentListenerIfNotExists(VRGrabberEvent unityEvent, UnityAction<HVRGrabberBase, HVRGrabbable> unityAction, UnityEngine.Object target)
    {
#if UNITY_EDITOR
        if (IterateExistingEvents(unityEvent).Any(e => e.MethodName == unityAction.Method.Name && e.Target == target))
        {
            return false;
        }
        UnityEditor.Events.UnityEventTools.AddPersistentListener(unityEvent, unityAction);
        return true;
#elif DEBUG
            Debug.LogWarning("Can't run in non-editor mode");
            return false;
#else
            return false;
#endif
    }
    
    public static IEnumerable<UnityEventToolsHelper.UnityEventMetadata> IterateExistingEvents(VRGrabberEvent unityEvent)
    {
#if UNITY_EDITOR
        for (var i = 0; i < unityEvent.GetPersistentEventCount(); ++i)
        {
            yield return new UnityEventToolsHelper.UnityEventMetadata(
                unityEvent.GetPersistentTarget(i),
                unityEvent.GetPersistentMethodName(i)
            );
        }
#elif DEBUG
            Debug.LogWarning("Can't run in non-editor mode");
            return Enumerable.Empty<UnityEventMetadata>();
#else
            return Enumerable.Empty<UnityEventMetadata>();
#endif
    }

#endif
}

#endif