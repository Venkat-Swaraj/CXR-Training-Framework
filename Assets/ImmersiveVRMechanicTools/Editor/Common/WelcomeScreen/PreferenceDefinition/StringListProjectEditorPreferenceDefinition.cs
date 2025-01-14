using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.PreferenceDefinition
{
    public class StringListProjectEditorPreferenceDefinition : ListProjectEditorPreferenceDefinitionBase<string>
    {
        public StringListProjectEditorPreferenceDefinition(string label, string preferenceKey, object defaultValue, 
            HandleOnEditorPersistedValueChange handleOnEditorPersistedValueChange = null,
            EditorPreferenceInitialize onInitialize = null, bool isReadonly = false)
            : base(label, preferenceKey, defaultValue, handleOnEditorPersistedValueChange, onInitialize, isReadonly)
        {
            

        }
        
        public override ReorderableList.ElementCallbackDelegate DrawListElementCallback(string label, List<string> values, bool isReadonly)
        {
            return (rect, index, active, focused) =>
            {
                if (isReadonly)
                {
                    EditorGUI.LabelField(rect, values[index]);
                }
                else
                {
                    values[index] = EditorGUI.TextField(rect, values[index]);
                }
            };
        }
    }
}