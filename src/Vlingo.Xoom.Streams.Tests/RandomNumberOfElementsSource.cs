// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Tests
{
    internal class RandomNumberOfElementsSource : DefaultSource<string>
    {
        private readonly AtomicInteger _element = new AtomicInteger(0);
        private readonly Random _count = new Random();
        private readonly int _total;
        private readonly bool _slow;

        public RandomNumberOfElementsSource(int total) : this(total, false)
        {
        }

        public RandomNumberOfElementsSource(int total, bool slow)
        {
            _total = total;
            _slow = slow;
        }

        public override ICompletes<bool> IsSlow() => Completes.WithSuccess(_slow);

        public override ICompletes<Elements<string>> Next()
        {
            var current = _element.Get();
            if (current >= _total)
            {
                return Completes.WithSuccess(new Elements<string>(Array.Empty<string>(), true));
            }

            var next = RandomNumberOfElements(current);
            for (var idx = 0; idx < next.Length; ++idx)
            {
                next[idx] = $"{_element.IncrementAndGet()}";
            }

            return Completes.WithSuccess(new Elements<string>(next, false));
        }

        private string[] RandomNumberOfElements(int current)
        {
            var bound = current > _total - 10 && current < _total ? _total - current : 7;
            var number = _count.Next(bound);
            var next = new string[number <= 0 ? 1 : number];
            return next;
        }

        public override ICompletes<Elements<string>> Next(int maximumElements) => Next();

        public override ICompletes<Elements<string>> Next(long index) => Next();

        public override ICompletes<Elements<string>> Next(long index, int maximumElements) => Next();
    }
}