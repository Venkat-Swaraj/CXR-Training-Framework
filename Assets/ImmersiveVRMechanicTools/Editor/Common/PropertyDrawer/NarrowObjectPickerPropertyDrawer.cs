using ImmersiveVRTools.Runtime.Common.PropertyDrawer;
using UnityEditor;
using UnityEngine;

namespace ImmersiveVRTools.Editor.Common.PropertyDrawer
{
    [CustomPropertyDrawer(typeof(NarrowObjectPickerAttribute))]
    public class NarrowObjectPickerPropertyDrawer : UnityEditor.PropertyDrawer
    {
        private int _openedPickerControlId;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //event type changes after ObjectField when clicked
            var eventTypeAtStartOfOnGui = Event.current.type;
           
            EditorGUI.ObjectField(position, property.displayName, property.objectReferenceValue, ((NarrowObjectPickerAttribute) attribute).ToType, false);
           
            //picker ID needs to be persisted to make sure correct one is handle
            if ((eventTypeAtStartOfOnGui == EventType.MouseDown || eventTypeAtStartOfOnGui == EventType.MouseUp) 
                && position.Contains(Event.current.mousePosition))
            {
                _openedPickerControlId = EditorGUIUtility.GetObjectPickerControlID();
            }
            
            if (Event.current.commandName == "ObjectSelectorClosed")
            {
                var currentPicketControlId = EditorGUIUtility.GetObjectPickerControlID();
                if (_openedPickerControlId == currentPicketControlId)
                {
                    var selectedObject = EditorGUIUtility.GetObjectPickerObject ();

                    if (selectedObject != property.objectReferenceValue)
                    {
                        property.objectReferenceValue = selectedObject;
                    }
                }
            }
        }
    }
}