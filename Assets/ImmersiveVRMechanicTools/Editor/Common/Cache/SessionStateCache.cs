using System;
using UnityEditor;

namespace ImmersiveVRTools.Editor.Common.Cache
{
    public static class SessionStateCache
    {
        public static string GetOrCreateString(string key, Func<string> create)
        {
            var val = SessionState.GetString(key, string.Empty);
            if (val == string.Empty)
            {
                val = create();
                SessionState.SetString(key, val);
            }

            return val;
        }
    }
}