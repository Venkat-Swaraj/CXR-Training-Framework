﻿using System;
using UnityEditor;
using UnityEngine;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.PreferenceDefinition
{
    public class EnumProjectEditorPreferenceDefinition : ProjectEditorPreferenceDefinitionBase
    {
        private readonly Type _enumType;

        public EnumProjectEditorPreferenceDefinition(string label, string preferenceKey, object defaultValue, Type enumType, 
            HandleOnEditorPersistedValueChange handleOnEditorPersistedValueChange = null, EditorPreferenceInitialize onInitialize = null)
            : base(label, preferenceKey, defaultValue, handleOnEditorPersistedValueChange, onInitialize)
        {
            _enumType = enumType;
        }

        protected override object GetEditorPersistedValueInternal()
        {
            return EditorPrefs.GetInt(PreferenceKey, (int)DefaultValue);
        }

        protected override void SetEditorPersistedValueInternal(object newValue)
        {
            EditorPrefs.SetInt(PreferenceKey, (int)newValue);
        }

        public override object RenderEditorAndCaptureInput(object currentValue, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            var enumValue = (Enum)Enum.ToObject(_enumType, currentValue);
            return EditorGUILayout.EnumPopup(GuiContent, enumValue, style ?? EditorStyles.popup, layoutOptions);
        }
    }
}