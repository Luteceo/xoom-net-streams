// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Source;

public class SupplierSourceTest : SourceTest
{
    private int _index;
        
    [Fact]
    public void TestThatSourceSupplierProvidesElements()
    {
        _index = 0;

        Func<string> supplier = () =>
        {
            if (_index >= 3) return null;
            var next = (char) ('A' + _index);
            var value = next.ToString();
            ++_index;
            return value;
        };

        var source = Source<string>.With(supplier);

        var result = StringFromSource(source);

        Assert.Equal("ABC", result);
    }
}