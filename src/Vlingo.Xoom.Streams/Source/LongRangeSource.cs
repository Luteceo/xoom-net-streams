// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Diagnostics.Contracts;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Source
{
    public class LongRangeSource : Source<long>
    {
        private readonly long _startInclusive;
        private readonly long _endExclusive;
        private long _current;

        public LongRangeSource(long startInclusive, long endExclusive)
        {
            Contract.Assert(startInclusive <= endExclusive);
            Contract.Assert(startInclusive >= 0 && startInclusive <= long.MaxValue);
            _startInclusive = startInclusive;
            Contract.Assert(endExclusive >= 0 && endExclusive <= long.MaxValue);
            _endExclusive = endExclusive;

            _current = startInclusive;
        }

        public override ICompletes<Elements<long>> Next()
        {
            if (_current >= _endExclusive)
            {
                return Completes.WithSuccess(new Elements<long>(new long[0], true));
            }
            var elements = new long[1];
            elements[0] = _current++;
            return Completes.WithSuccess(new Elements<long>(elements, false));
        }

        public override ICompletes<Elements<long>> Next(int maximumElements) => Next();

        public override ICompletes<Elements<long>> Next(long index) => Next();

        public override ICompletes<Elements<long>> Next(long index, int maximumElements) => Next();

        public override ICompletes<bool> IsSlow() => Completes.WithSuccess(false);

        public override string ToString() => 
            $"LongRangeSource [startInclusive={_startInclusive} endExclusive={_endExclusive} current={_current}]";
    }
}