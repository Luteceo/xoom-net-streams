// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Source
{
    public class LongRangeSource : DefaultSource<long>
    {
        private readonly long _startInclusive;
        private readonly long _endExclusive;
        private long _current;

        public LongRangeSource(long startInclusive, long endExclusive)
        {
            if (startInclusive > endExclusive)
            {
                throw new ArgumentOutOfRangeException(nameof(endExclusive),
                    "End exclusive should be lesser than start Inclusive");
            }
            if (startInclusive < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startInclusive),
                    "Start inclusive should not be lower than 0");
            }
            if (endExclusive < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(endExclusive),
                    "End inclusive should not be lower than 0");
            }
            
            _startInclusive = startInclusive;
            _endExclusive = endExclusive;

            _current = startInclusive;
        }

        public override ICompletes<Elements<long>> Next()
        {
            if (_current >= _endExclusive)
            {
                return Completes.WithSuccess(new Elements<long>(Array.Empty<long>(), true));
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