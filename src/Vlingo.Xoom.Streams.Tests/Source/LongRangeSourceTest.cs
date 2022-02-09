// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Xoom.Streams.Source;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Source
{
    public class LongRangeSourceTest
    {
        private long _current = 0;
        private long _expected = 0;
        private bool _terminated = false;
        
        [Fact]
        public void TestThatRangeCompletes()
        {
            var range = Source<long>.RangeOf(1, 11);

            while (!_terminated)
            {
                ++_expected;
                range.Next().AndThenConsume(elements => {
                    if (!elements.IsTerminated)
                    {
                        _current = elements.Values[0];
                        Assert.Equal(_expected, _current);
                    }
                    _terminated = elements.IsTerminated;
                });
            }

            Assert.Equal(10, _current);
        }

        [Fact]
        public void TestThatZeroZeroRangeCompletes()
        {
            var range = Source<long>.RangeOf(0, 0);

            range.Next().AndThenConsume(elements =>
            {
                if (!elements.IsTerminated)
                {
                    _current = elements.Values[0];
                    Assert.Equal(_expected, _current);
                }
                else
                {
                    _terminated = true;
                }
            });

            Assert.True(_terminated);
        }

        [Fact]
        public void TestThatOverflowThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LongRangeSource(0, -1));
        }

        [Fact]
        public void TestThatStartUnderflowThrows()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new LongRangeSource(-1, long.MaxValue));
        }
    }
}