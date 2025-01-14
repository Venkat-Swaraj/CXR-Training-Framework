#if INTEGRATIONS_XRTOOLKIT

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(ActionBasedController))]
public class ActionBasedXRToolkitHapticFeedbackControl : HapticFeedbackControl
{
    private ActionBasedController _xrController;

    void Start()
    {
        _xrController = GetComponent<ActionBasedController>();
    }


    public override void SendHapticFeedback(float amplitude, float duration)
    {
        _xrController.SendHapticImpulse(amplitude, duration);
    }
}

#endif