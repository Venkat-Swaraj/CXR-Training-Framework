using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.PreferenceDefinition
{
    public abstract class ListProjectEditorPreferenceDefinitionBase<T> : ProjectEditorPreferenceDefinitionBase
    {
        private readonly bool _isReadonly;
        public const string Delimiter = ":|:";
        private static string[] ArrayDelimiters = new string[] { Delimiter };
        
        protected ReorderableList _excludedFilesGuiList;
        private readonly List<T> _values = new List<T>();
        
        public ListProjectEditorPreferenceDefinitionBase(string label, string preferenceKey, object defaultValue,
            HandleOnEditorPersistedValueChange handleOnEditorPersistedValueChange = null, EditorPreferenceInitialize onInitialize = null, bool isReadonly = false) 
            : base(label, preferenceKey, defaultValue, handleOnEditorPersistedValueChange, onInitialize)
        {
            _isReadonly = isReadonly;
            _values.AddRange(GetEditorPersistedValueInternal() as IEnumerable<T> ?? Array.Empty<T>());
            _excludedFilesGuiList = new ReorderableList(_values, typeof(string));
            _excludedFilesGuiList.drawHeaderCallback += rect =>
            {
                GUI.Label(rect, label);
            };
            _excludedFilesGuiList.drawElementCallback += DrawListElementCallback(label, _values, _isReadonly);
            _excludedFilesGuiList.displayAdd = !isReadonly;
            _excludedFilesGuiList.onAddCallback += OnAddCallback;
        }

        protected virtual void OnAddCallback(ReorderableList list)
        {
            if (typeof(T) == typeof(string))
            {
                AddElement((T)(object)string.Empty);
            }
            else
            {
                AddElement(Activator.CreateInstance<T>());
            }
        }
        
        public abstract ReorderableList.ElementCallbackDelegate DrawListElementCallback(string label, List<T> values, bool isReadonly);


        protected override object GetEditorPersistedValueInternal()
        {
            return EditorPrefs.GetString(PreferenceKey,  
                string.Join(Delimiter, (DefaultValue as IEnumerable<T>) ?? Array.Empty<T>())
            ).Split(ArrayDelimiters, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        protected override void SetEditorPersistedValueInternal(object newValue)
        {
            EditorPrefs.SetString(PreferenceKey, string.Join(Delimiter, newValue as IEnumerable<T> ?? Array.Empty<T>()));
        }

        public override object RenderEditorAndCaptureInput(object currentValue, GUIStyle style, params GUILayoutOption[] layoutOptions)
        {
            _excludedFilesGuiList.DoLayoutList();
            return _values;
        }

        public void AddElement(T element)
        {
            var current = (List<T>)GetEditorPersistedValueOrDefault();
            current.Add(element);
            SetEditorPersistedValue(current);
            _values.Clear();
            _values.AddRange(current);
        }
        
        public void RemoveElement(T element)
        {
            var current = (List<T>)GetEditorPersistedValueOrDefault();
            current.Remove(element);
            SetEditorPersistedValue(current);
            _values.Clear();
            _values.AddRange(current);
        }

        public IEnumerable<T> GetElements()
        {
            return (IEnumerable<T>)GetEditorPersistedValueOrDefault();
        }
    }
}