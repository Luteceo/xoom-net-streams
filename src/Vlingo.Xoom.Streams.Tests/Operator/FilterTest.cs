// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Operator;

public class FilterTest
{
    [Fact]
    public void TestThatFilterFilters()
    {
        var filter = Operator<string, string>.FilterWith((s) => s.Contains("1"));

        var results = new List<string>();
        new[] { "ABC", "321", "123", "456", "DEF", "214" }
            .ToList()
            .ForEach(possible => filter.PerformInto(possible, (match) => results.Add(match)));

        Assert.Equal(3, results.Count);
        Assert.Equal("321", results[0]);
        Assert.Equal("123", results[1]);
        Assert.Equal("214", results[2]);
    }
}