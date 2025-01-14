using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class ReflectionHelper
    {
        public static Type GetType(string fullTypeName)
        {
            return GetAllTypes().FirstOrDefault(t => t.FullName == fullTypeName);
        }
    
        public static List<Type> GetAllInstantiableTypesDerivedFrom(Type type, List<Type> except = null)
        {
            var all = GetAllTypes()
                .Where(t => t.BaseType != null && ((t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == type)
                    || (t.IsSubclassOf(type) && t != type && !t.IsAbstract)))
                .Distinct()
                .ToList();

            return except != null
                ? all.Except(except).ToList()
                : all;
        }

        public static List<Type> GetAllTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .ToList();
        }
        
        public static FieldInfo GetFieldInfoIncludingBaseClasses(Type type, string name, BindingFlags bindingFlags) {
            if(type.BaseType == typeof(object)) {
                return type.GetField(name, bindingFlags);
            } 
            else {
                var fieldInfo = type.GetField(name, bindingFlags);
                if (fieldInfo == null)
                {
                    return GetFieldInfoIncludingBaseClasses(type.BaseType, name, bindingFlags);
                }
                else
                {
                    return fieldInfo;
                }
            }
        }
    }
}