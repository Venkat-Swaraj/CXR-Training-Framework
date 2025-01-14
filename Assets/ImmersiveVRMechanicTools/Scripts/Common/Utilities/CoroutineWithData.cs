using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class CoroutineWithData<TResult>
    {
        private IEnumerator _target;
        
        public Coroutine Coroutine { get; private set; }
        public TResult Result { get; private set; }

        public CoroutineWithData(MonoBehaviour owner, IEnumerator target)
        {
            _target = target;
            Coroutine = owner.StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            while (_target.MoveNext())
            {
                if (_target.Current is TResult result)
                {
                    Result = result;
                }

                yield return _target.Current;
            }
        }
    }
}