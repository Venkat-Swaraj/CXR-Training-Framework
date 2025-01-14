#if !UNITY_2022_1_OR_NEWER //Unity API change

using System;
using UnityEngine.EventSystems;



namespace ImmersiveVRTools.Runtime.Common.Extensions 
{
    public static class EventTriggerExtensions
    {
        public static void AddListener(this EventTrigger trigger, EventTriggerType type, Action<PointerEventData> callback)
        {
            var entry = new EventTrigger.Entry {eventID = type};
            entry.callback.AddListener((data) => { callback((PointerEventData)data); });
            trigger.triggers.Add(entry);
        }
    }
}

#endif

