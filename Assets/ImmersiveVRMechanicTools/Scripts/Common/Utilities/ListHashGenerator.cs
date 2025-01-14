using System.Collections.Generic;

namespace ImmersiveVRTools.Runtime.Common.Utilities
{
    public class ListHashGenerator
    {
        public static int GetHashBasedOnElements<T>(IEnumerable<T> items) where T : struct
        {
            unchecked
            {
                var hash = 19;
                foreach (var item in items)
                {
                    hash = hash * 31 + item.GetHashCode();
                }
                return hash;
            }
        }
    }
}