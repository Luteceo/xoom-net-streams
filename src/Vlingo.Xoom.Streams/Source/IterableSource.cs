// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Source
{
    public class IterableSource<T> : DefaultSource<T>
    {
        private readonly IEnumerator<T> _iterator;
        private readonly bool _slowIterable;

        public IterableSource(IEnumerable<T> iterator, bool slowIterable)
        {
            _iterator = iterator.GetEnumerator();
            _slowIterable = slowIterable;
        }

        public override ICompletes<Elements<T>> Next()
        {
            if (!_iterator.MoveNext())
            {
                return Completes.WithSuccess(new Elements<T>(Array.Empty<T>(), true));
            }

            var elements = new T[1];
            elements[0] = _iterator.Current;
            return Completes.WithSuccess(new Elements<T>(elements, false));
        }

        public override ICompletes<Elements<T>> Next(int maximumElements) => Next();

        public override ICompletes<Elements<T>> Next(long index) => Next();

        public override ICompletes<Elements<T>> Next(long index, int maximumElements) => Next();

        public override ICompletes<bool> IsSlow() => Completes.WithSuccess(_slowIterable);
    }
}