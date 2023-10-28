using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Pair;

public static class PairSequenceExtensions
{
    public static IEnumerable<IPair<TKey, TValue>> GetCovariantView<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> source)
        where TKey : notnull
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        return source.Select(kvp => new Pair<TKey, TValue>(kvp));
    }

    public static IEnumerable<IPair<TKey, TValue>> CastPairs<TKey, TValue>(this IEnumerable<IPair<TKey, TValue>> source)
        where TKey : notnull
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        return source;
    }
}
