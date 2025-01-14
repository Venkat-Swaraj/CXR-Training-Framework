using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace ImmersiveVRTools.Editor.Common.WelcomeScreen.PreferenceDefinition
{
    public class JsonObjectListProjectEditorPreferenceDefinition<T> : ListProjectEditorPreferenceDefinitionBase<string>
        where T: IJsonObjectListSerializable, new()
    {
        private readonly Func<T> _onAddNewCreateInstance;
        private readonly List<IJsonObjectRepresentationRenderingInfo> _jsonObjectRepresentationRenderingInfos;
        
        public JsonObjectListProjectEditorPreferenceDefinition(string label, string preferenceKey, object defaultValue, Func<T> onAddNewCreateInstance,
            HandleOnEditorPersistedValueChange handleOnEditorPersistedValueChange = null,
            EditorPreferenceInitialize onInitialize = null, bool isReadonly = false)
            : base(label, preferenceKey, defaultValue, handleOnEditorPersistedValueChange, onInitialize, isReadonly)
        {
            _onAddNewCreateInstance = onAddNewCreateInstance;
            //TODO: not ideal as using generate infos in class invites use of instance fields, instead of passed in ones
            _jsonObjectRepresentationRenderingInfos = new T().GenerateRenderingInfo().ToList();
        }
        
        public override ReorderableList.ElementCallbackDelegate DrawListElementCallback(string label, List<string> values, bool isReadonly)
        {
            return (rect, index, active, focused) =>
            {
                var parsed = new T(); //TODO - PERF: do not recreate every call, do dictionary based on index?
                parsed.DeserializeOntoExisting(values[index]);
                
                if (isReadonly)
                {
                    EditorGUILayout.BeginHorizontal();
                    foreach (var renderingInfo in _jsonObjectRepresentationRenderingInfos)
                    {
                        EditorGUI.LabelField(rect, renderingInfo.GetValueDelegateNonGeneric(parsed).ToString());
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    const int spaceBetweenFields = 10;
                    const int spaceBetweenLabelAndField = 5;
                    var currentX = rect.x;
                    var useRect = new Rect(currentX, rect.y, 0, rect.height);
                    for (var i = 0; i < _jsonObjectRepresentationRenderingInfos.Count; i++)
                    {
                        var renderingInfo = _jsonObjectRepresentationRenderingInfos[i];
                        var labelContent = new GUIContent(renderingInfo.Label);
                        var labelWidth = EditorStyles.label.CalcSize(labelContent).x;
                        useRect.x = currentX;
                        useRect.width = labelWidth;
                        
                        EditorGUI.LabelField(useRect, labelContent);
                        currentX += labelWidth + spaceBetweenLabelAndField;

                        var remainingWidthForField = renderingInfo.WidthPx - (labelWidth + spaceBetweenLabelAndField);
                        if (remainingWidthForField < 0)
                        {
                            Debug.LogWarning($"Not enough available width to render field for label: '{renderingInfo.Label}', please adjust {nameof(JsonObjectRepresentationStringRenderingInfo<T>)}");
                        }

                        useRect.width = remainingWidthForField;
                        useRect.x = currentX;
                        currentX += remainingWidthForField + spaceBetweenFields;
                        if (renderingInfo is JsonObjectRepresentationStringRenderingInfo<T>)
                        {
                            var value = EditorGUI.TextField(useRect, renderingInfo.GetValueDelegateNonGeneric(parsed).ToString());
                            renderingInfo.SetValueDelegateNonGeneric(parsed, value);
                        }
                        else if (renderingInfo is JsonObjectRepresentationBoolRenderingInfo<T>)
                        {
                            var value = EditorGUI.Toggle(useRect, (bool)renderingInfo.GetValueDelegateNonGeneric(parsed));
                            renderingInfo.SetValueDelegateNonGeneric(parsed, value);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    values[index] = parsed.Serialize();
                }
            };
        }
        
        protected override void OnAddCallback(ReorderableList list)
        {
            AddElement(_onAddNewCreateInstance().Serialize());
        }

        public List<T> GetElementsTyped()
        {
            return GetElements().Select(str =>
            {
                var el = new T();
                el.DeserializeOntoExisting(str);
                return el;
            }).ToList();
        }
    }

    public abstract class JsonObjectListSerializable<T>: IJsonObjectListSerializable
        where T: IJsonObjectListSerializable
    {
        public virtual string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public virtual void DeserializeOntoExisting(string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
        }

        public abstract List<IJsonObjectRepresentationRenderingInfo> GenerateRenderingInfo();
        
        public List<object> GenerateRenderingInfoNonGeneric()
        {
            return GenerateRenderingInfo().Cast<object>().ToList();
        }
    }

    public interface IJsonObjectListSerializable
    {
        List<IJsonObjectRepresentationRenderingInfo> GenerateRenderingInfo();
        void DeserializeOntoExisting(string json);
        string Serialize();
    }

    public class JsonObjectRepresentationStringRenderingInfo<T>: JsonObjectRepresentationRenderingInfoBase<T, string>
        where T : IJsonObjectListSerializable
    {
        public JsonObjectRepresentationStringRenderingInfo(string label, Func<T, string> getValueDelegate, Action<T, string> setValueDelegate, int widthPx) 
            : base(getValueDelegate, setValueDelegate, label, widthPx)
        {
        }
    }
    
    public class JsonObjectRepresentationBoolRenderingInfo<T>: JsonObjectRepresentationRenderingInfoBase<T, bool>
        where T : IJsonObjectListSerializable
    {
        public JsonObjectRepresentationBoolRenderingInfo(string label, Func<T, bool> getValueDelegate, Action<T, bool> setValueDelegate, int widthPx) 
            : base(getValueDelegate, setValueDelegate, label, widthPx)
        {
        }
    }

    public interface IJsonObjectRepresentationRenderingInfo
    {
        string Label { get; }
        int WidthPx { get; }
        Func<object, object> GetValueDelegateNonGeneric { get; }
        Action<object, object> SetValueDelegateNonGeneric { get; }
    }
    
    public class JsonObjectRepresentationRenderingInfoBase<TObj, TProp>: IJsonObjectRepresentationRenderingInfo
        where TObj : IJsonObjectListSerializable
    {
        public Func<TObj, TProp> GetValueDelegate { get; }
        public Action<TObj, TProp> SetValueDelegate { get; }
        public string Label { get; }
        public int WidthPx { get; } //TODO: ideally width would be auto-calculated by container
        public Func<object, object> GetValueDelegateNonGeneric { get; private set; }

        public Action<object, object> SetValueDelegateNonGeneric { get; private set; }

        public JsonObjectRepresentationRenderingInfoBase(Func<TObj, TProp> getValueDelegate, Action<TObj, TProp> setValueDelegate, string label, int widthPx)
        {
            GetValueDelegate = getValueDelegate;
            SetValueDelegate = setValueDelegate;
            Label = label;
            WidthPx = widthPx;

            GetValueDelegateNonGeneric = o => (TProp)GetValueDelegate((TObj)o);
            SetValueDelegateNonGeneric = (o, p) => SetValueDelegate((TObj)o,(TProp)p);
        }
    }
}