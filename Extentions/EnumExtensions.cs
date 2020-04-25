using System;
using System.Collections.Generic;
using System.Linq;

namespace FlexibleDBMS
{
 public static   class EnumExtensions
    {
        public static IEnumerable<T> GetEnumValueCollection<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
