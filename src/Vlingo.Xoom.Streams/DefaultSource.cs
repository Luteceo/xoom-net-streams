// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
    public abstract class DefaultSource<T> : ISource<T>
    {
        public ISource<T> Empty() => Source<T>.Empty();

        public ISource<T> Only(IEnumerable<T> elements) => Source<T>.Only(elements);

        public ISource<long> RangeOf(long startInclusive, long endExclusive) => Source<T>.RangeOf(startInclusive, endExclusive);

        public long OrElseMaximum(long elements) => Source<T>.OrElseMaximum(elements);

        public long OrElseMinimum(long elements) => Source<T>.OrElseMinimum(elements);

        public ISource<T> With(IEnumerable<T> iterable) => Source<T>.With(iterable);

        public ISource<T> With(IEnumerable<T> iterable, bool slowIterable) => Source<T>.With(iterable, slowIterable);

        public ISource<T> With(Func<T> supplier) => Source<T>.With(supplier);

        public ISource<T> With(Func<T> supplier, bool slowSupplier) => Source<T>.With(supplier, slowSupplier);

        public abstract ICompletes<Elements<T>> Next();

        public abstract ICompletes<Elements<T>> Next(int maximumElements);

        public abstract ICompletes<Elements<T>> Next(long index);

        public abstract ICompletes<Elements<T>> Next(long index, int maximumElements);

        public abstract ICompletes<bool> IsSlow();
    }
}