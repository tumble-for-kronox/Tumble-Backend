using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Pair;

public interface IPair<TKey, out TValue>
    where TKey : notnull
{
    TKey Key { get; }
    TValue Value { get; }
}
