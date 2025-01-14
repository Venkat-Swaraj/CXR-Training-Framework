using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToStringEventDataTypeConverter : EventDataTypeConverterBase<string>
{
    public override void Convert(object convertValue)
    {
        var asString = convertValue.ToString();
        Converted?.Invoke(asString);
    }
}


public abstract class EventDataTypeConverterBase<T> : MonoBehaviour
{
    public UnityEvent<T> Converted;

    public abstract void Convert(object convertValue);
}