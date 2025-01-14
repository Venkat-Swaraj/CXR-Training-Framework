using System;
using UnityEngine;

namespace ImmersiveVRTools.Runtime.Common.CustomYieldInstuctions
{
    public sealed class WaitUntilOrTimeout : CustomYieldInstruction
    {
        private Func<bool> predicate;
        private float m_timeout;

        public WaitUntilOrTimeout(float timeout, Func<bool> predicate)
        {
            this.predicate = predicate;
            m_timeout = timeout;
        }
        
        private bool WaitForDoneProcess()
        {
            m_timeout -= Time.deltaTime;
            return m_timeout <= 0f || predicate();
        }
 
        public override bool keepWaiting => !WaitForDoneProcess();
    }
}