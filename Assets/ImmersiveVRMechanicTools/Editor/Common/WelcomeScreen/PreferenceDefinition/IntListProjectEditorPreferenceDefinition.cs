using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.PreferenceDefinition
{
    public class IntListProjectEditorPreferenceDefinition : ListProjectEditorPreferenceDefinitionBase<int>
    {
        
        public IntListProjectEditorPreferenceDefinition(string label, string preferenceKey, object defaultValue, 
            HandleOnEditorPersistedValueChange handleOnEditorPersistedValueChange = null,
            EditorPreferenceInitialize onInitialize = null, bool isReadonly = false)
            : base(label, preferenceKey, defaultValue, handleOnEditorPersistedValueChange, onInitialize, isReadonly)
        {


        }
        
        public override ReorderableList.ElementCallbackDelegate DrawListElementCallback(string label, List<int> values, bool isReadonly)
        {
            return (rect, index, active, focused) =>
            {
                if (isReadonly)
                {
                    EditorGUI.LabelField(rect, values[index].ToString());
                }
                else
                {
                    values[index] = EditorGUI.IntField(rect, values[index]);
                }
            };
        }
    }
}