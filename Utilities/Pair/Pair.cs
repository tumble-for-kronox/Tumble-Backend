using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Pair;

public class Pair<TKey, TValue> : IPair<TKey, TValue>
    where TKey : notnull
{
    public TKey Key { get; }
    public TValue Value { get; }

    public Pair(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public Pair(KeyValuePair<TKey, TValue> pair)
        : this(pair.Key, pair.Value)
    { }
}
