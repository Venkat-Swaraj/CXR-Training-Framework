using System;
using System.Linq;
using ImmersiveVRTools.Runtime.Common.Utilities;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.PropertyDrawer
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
    public class NarrowObjectPickerAttribute : PropertyAttribute
    {
        private Type _toType;
        private static readonly Type DefaultTypeIfUnresolvedFromString = typeof(UnityEngine.Object);

        public Type ToType
        {
            get
            {
                if (_toType == null)
                {
                    try
                    {
                        var resolvedType = ReflectionHelper.GetType(ToTypeName) ?? DefaultTypeIfUnresolvedFromString;
                        _toType = resolvedType;
                    }
                    catch (Exception)
                    {
                        _toType = DefaultTypeIfUnresolvedFromString;
                    }
                }
                return _toType;
            }
        }

        private string ToTypeName { get; }


        public NarrowObjectPickerAttribute(Type toType)
        {
            _toType = toType;
        }

        //used for conditional compilation scenarios when type might not be loaded
        public NarrowObjectPickerAttribute(string toTypeName)
        {
            ToTypeName = toTypeName;
        }
    }
}