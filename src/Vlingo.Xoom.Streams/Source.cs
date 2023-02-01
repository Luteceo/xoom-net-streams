// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Vlingo.Xoom.Streams.Source;

namespace Vlingo.Xoom.Streams;

public static class Source<T>
{
    public static ISource<T> Empty() => new IterableSource<T>(new List<T>(0), false);
        
    public static ISource<T> Only(IEnumerable<T> elements) => new IterableSource<T>(new List<T>(elements), false);
        
    public static ISource<long> RangeOf(long startInclusive, long endExclusive) => new LongRangeSource(startInclusive, endExclusive);
        
    public static long OrElseMaximum(long elements)
    {
        if (elements < 0)
        {
            return long.MaxValue;
        }
            
        return elements;
    }
        
    public static long OrElseMinimum(long elements)
    {
        if (elements < 0)
        {
            return 0;
        }
            
        return elements;
    }
        
    public static ISource<T> With(IEnumerable<T> iterable) => With(iterable, false);
        
    public static ISource<T> With(IEnumerable<T> iterable, bool slowIterable) => new IterableSource<T>(iterable, slowIterable);
        
    public static ISource<T> With(Func<T> supplier) => With(supplier, false);
        
    public static ISource<T> With(Func<T> supplier, bool slowSupplier) => new SupplierSource<T>(supplier, slowSupplier);
}