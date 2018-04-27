using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGrouper.Cli
{
    static class Extensions
    {
        public static IEnumerable<T[]> Split<T>(this T[] source, Func<T, bool> predicate)
        {
            List<T> split = new List<T>();
            foreach (var item in source)
            {
                if (predicate(item))
                {
                    yield return split.ToArray();
                    split.Clear();
                }
                else
                    split.Add(item);
            }
            yield return split.ToArray();
        }
    }
}
