using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperExamples.Utils
{
public static class DapperExtensions
{
    public static void Map<TFirst, TSecond, TKey>
    (
        this IEnumerable<TFirst> list,
        IEnumerable<TSecond> child,
        Func<TFirst, TKey> firstKey,
        Func<TSecond, TKey> secondKey,
        Action<TFirst, IEnumerable<TSecond>> addChildren
    )
    {
        var childMap = child.GroupBy(secondKey).ToDictionary(g => g.Key, g => g.AsEnumerable());

        Parallel.ForEach(list, item =>
        {
            if (!childMap.Any()) return;
            IEnumerable<TSecond> children;

            var first = firstKey(item);

            if (first != null && childMap.TryGetValue(first, out children))
            {
                addChildren(item, children);
            }
        });
    }
}
}
