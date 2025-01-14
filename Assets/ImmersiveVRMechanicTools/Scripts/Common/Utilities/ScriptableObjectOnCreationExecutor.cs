using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class ScriptableObjectOnCreationExecutor
    {
        private const float MAX_TIME_TO_WAIT_FOR_OBJECT_TO_BE_CREATED_BEFORE_REMOVING_EVENT = 30;
        
        private float _editorUpdateEventAddedAt;
        private bool _editorUpdateEventTimeSet;
        
        private Action _executeOnCreation;
        private UnityEngine.ScriptableObject _so;
        private float _abandonAfterNSecondsIfNotCreated;

        public ScriptableObjectOnCreationExecutor(Action executeOnCreation, UnityEngine.ScriptableObject so, float abandonAfterNSecondsIfNotCreated = MAX_TIME_TO_WAIT_FOR_OBJECT_TO_BE_CREATED_BEFORE_REMOVING_EVENT)
        {
            _executeOnCreation = executeOnCreation;
            _so = so;
            _abandonAfterNSecondsIfNotCreated = abandonAfterNSecondsIfNotCreated;
        }
        
        public void ExecuteOnCreationOnly()
        {
#if UNITY_EDITOR
            if (ScriptableObjectAwakeCalledReasonResolver.ResolveAwakeCallReason(_so) == ScriptableObjectAwakeCalledReason.ObjectBeingCreated)
            {
                UnityEditor.EditorApplication.update += ExecuteOnCreationOnlyHandler;
            }
#endif
            //nothing happens outside of editor on purpose
        }

#if UNITY_EDITOR
        private void ExecuteOnCreationOnlyHandler()
        {

            if (!_editorUpdateEventTimeSet) //Time can't be called in Awake / Reset as it's considered ctor and will throw error in editor
            {
                _editorUpdateEventAddedAt = Time.realtimeSinceStartup;
                _editorUpdateEventTimeSet = true;
            }
            
            var createdAssetPath = UnityEditor.AssetDatabase.GetAssetPath(_so);
            if (!string.IsNullOrEmpty(createdAssetPath))
            {
                _executeOnCreation();
                UnityEditor.EditorApplication.update -= ExecuteOnCreationOnlyHandler;
            }
            else if (Time.realtimeSinceStartup > _editorUpdateEventAddedAt + _abandonAfterNSecondsIfNotCreated)
            {
                UnityEngine.Debug.Log("Step completed event never create not created, removing event handler");
                UnityEditor.EditorApplication.update -= ExecuteOnCreationOnlyHandler;
            }
        }
#endif

    }
}