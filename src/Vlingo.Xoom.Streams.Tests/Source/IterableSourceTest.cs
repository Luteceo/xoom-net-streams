// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Source;

public class IterableSourceTest : SourceTest
{
    [Fact]
    public void TestThatEmptyHasNoElements()
    {
        var source = Source<string>.Empty();

        var result = StringFromSource(source);

        Assert.True(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void TestThatSourceProvidesElements()
    {
        var source = Source<string>.Only(new[] { "A", "B", "C" });

        var result = StringFromSource(source);

        Assert.Equal("ABC", result);
    }

    [Fact]
    public void TestThatSourceCollectionProvidesElements()
    {
        var source = Source<string>.With(new[] { "A", "B", "C" });

        var result = StringFromSource(source);

        Assert.Equal("ABC", result);
    }
}