using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.Infrastructure
{
    public class NullableComparer<T> : IEqualityComparer<T?>
         where T : struct
    {
        public bool Equals(T? x, T? y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            return x.Equals(y);
        }

        public int GetHashCode(T? obj)
        {
            return obj.GetHashCode();
        }
    }
}
