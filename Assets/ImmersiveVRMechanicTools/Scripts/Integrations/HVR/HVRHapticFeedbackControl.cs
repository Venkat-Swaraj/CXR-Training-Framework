#if INTEGRATIONS_HVR

using UnityEngine;

public class HVRHapticFeedbackControl : HapticFeedbackControl
{
    [SerializeField] private float _vibrateFrequency = 1;
    
    public override void SendHapticFeedback(float amplitude, float duration)
    {
       //TODO: implement
    }
}

#endif