using System;
using System.Collections;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class ChangedSincePreviousFrameHandler<T> : ChangeSincePreviousFrameMonitor<T>
    {
        public delegate void ValueChanged(T oldValue, T newValue);
        
        private ValueChanged _onValueChanged;
        
        private readonly bool _triggerHandlerOnInitialUpdateCall;
        private bool _isFirstUpdateFinished;

        public ChangedSincePreviousFrameHandler(Func<T> getValue, MonoBehaviour coroutineRunner, ValueChanged onValueChanged, bool triggerHandlerOnInitialUpdateCall) 
            : base(getValue, coroutineRunner, false)
        {
            _onValueChanged = onValueChanged;
            _triggerHandlerOnInitialUpdateCall = triggerHandlerOnInitialUpdateCall;
            
            //starting coroutine here and using ctor that won't start it in base class, this is to allow _triggerHandlerOnInitialUpdateCall field to be set before first run
            coroutineRunner.StartCoroutine(Update());
        }

        protected override IEnumerator Update()
        {
            var updateEnumerator = base.Update();
            do
            {
                if (!updateEnumerator.MoveNext())
                {
                    yield break;
                }    
                
                if (!_isFirstUpdateFinished && _triggerHandlerOnInitialUpdateCall)
                {
                    TriggerHandlerSafe();
                }
                _isFirstUpdateFinished = true;
                
                if (IsValueUpdatedSinceLastUpdateCall)
                {
                    TriggerHandlerSafe();
                }

                yield return updateEnumerator.Current;
            } while (true);
        }

        private void TriggerHandlerSafe()
        {
            try
            {
                _onValueChanged?.Invoke(_previousValue, CurrentValue);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }
        }
    }
}