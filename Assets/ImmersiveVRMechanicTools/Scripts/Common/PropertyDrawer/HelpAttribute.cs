using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.PropertyDrawer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
    
    public class HelpAttribute : PropertyAttribute
    {
        public string Text { get; }

        public UnityMessageType Type { get; }
        
        public HelpAttribute(string text, UnityMessageType type = UnityMessageType.Info)
        {
            Text = text;
            Type = type;
        }
    }
    
    public enum UnityMessageType
    {
        None,
        Info,
        Warning,
        Error
    }
}