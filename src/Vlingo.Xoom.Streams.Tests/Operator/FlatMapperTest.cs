// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Vlingo.Xoom.Streams.Source;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Operator
{
    public class FlatMapperTest
    {
        [Fact]
        public void TestThatPropagatesRecordsFormTheProvidedSource()
        {
            var flatMapper = Operator<long, long>.FlatMapWith((record) => new LongRangeSource(record, record + 2));
            var providedLongs = new List<long>();

            flatMapper.PerformInto(1L, providedLongs.Add);
            flatMapper.PerformInto(3L, providedLongs.Add);

            Assert.Equal(new[] { 1L, 2L, 3L, 4L }, providedLongs.ToArray());
        }
    }
}