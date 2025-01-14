using System;
using System.Diagnostics;
using ImmersiveVrToolsCommon.Runtime.Logging;

namespace ImmersiveVRTools.Editor.Common.Utilities {
    public static class PerfMeasure
    {
        public static T1 Elapsed<T1>(Func<T1> executeFn, out long elapsedMilliseconds, string logMessageOnFinish = null)
        {
            T1 result;
            var sw = new Stopwatch();
            sw.Start();
            result = executeFn();
            sw.Stop();

            elapsedMilliseconds = sw.ElapsedMilliseconds;
            
            if (!string.IsNullOrEmpty(logMessageOnFinish))
            {
                LoggerScoped.Log(logMessageOnFinish);
            }

            return result;
        }
    }
}