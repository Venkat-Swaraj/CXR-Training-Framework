using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common
{
	[ExecuteAlways]
	public class UnityMainThreadDispatcher : SingletonBase<UnityMainThreadDispatcher>
	{
		private static readonly Queue<Action> _executionQueue = new Queue<Action>();

		public void Update()
		{
			lock (_executionQueue)
			{
				while (_executionQueue.Count > 0) _executionQueue.Dequeue().Invoke();
			}
		}

		public void Enqueue(IEnumerator action)
		{
			lock (_executionQueue)
			{
				_executionQueue.Enqueue(() => { StartCoroutine(action); });
			}
		}

		public void Enqueue(Action action)
		{
			Enqueue(ActionWrapper(action));
		}

		public Task EnqueueAsync(Action action)
		{
			var tcs = new TaskCompletionSource<bool>();

			void WrappedAction()
			{
				try
				{
					action();
					tcs.TrySetResult(true);
				}
				catch (Exception ex)
				{
					tcs.TrySetException(ex);
				}
            }

            Enqueue(ActionWrapper(WrappedAction));
            return tcs.Task;
        }


        private IEnumerator ActionWrapper(Action a)
        {
            a();
            yield return null;
        }
	}
}