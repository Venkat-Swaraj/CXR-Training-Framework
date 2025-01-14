#if INTEGRATIONS_XRTOOLKIT

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using CommonUsages = UnityEngine.XR.CommonUsages;

[RequireComponent(typeof(ActionBasedController))]
public class ActionBasedXRToolkitFrameworkToolInput : XRFrameworkToolInput
{
    [SerializeField] private ActionBasedController XRController;
    [SerializeField] InputActionProperty JoystickVector2ValueAction;
    [SerializeField] InputActionProperty JoystickClickAction;

    protected override bool IsIncreasingToolForce()
    {
        if (JoystickVector2ValueAction.action.inProgress)
        {
            if (JoystickVector2ValueAction.action.ReadValue<Vector2>().y > 0.3f) return true;
        }

        return false;
    }

    protected override bool IsDecreasingToolForce() 
    {
        if (JoystickVector2ValueAction.action.inProgress)
        {
            if (JoystickVector2ValueAction.action.ReadValue<Vector2>().y < -0.3f) return true;
        }

        return false;
    }

    protected override bool IsChangingToolDirection()
    {
        if (JoystickClickAction.action.inProgress)
        {
            return JoystickClickAction.action.triggered;
        }
        
        return false;
    }

    void Reset()
    {
        XRController = GetComponent<ActionBasedController>();
    }
}

#endif