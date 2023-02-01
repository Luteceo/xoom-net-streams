// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Vlingo.Xoom.Common;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Operator;

public class MapperTest
{
    [Fact]
    public void TestThatMappersMaps()
    {
        var mapper = Operator<string, int>.MapWith(int.Parse);

        var results = new List<int>();
        new[] { "123", "456", "789" }
            .ToList()
            .ForEach(digits => mapper.PerformInto(digits, number => results.Add(number)));

        Assert.Equal(3, results.Count);
        Assert.Equal(123, results[0]);
        Assert.Equal(456, results[1]);
        Assert.Equal(789, results[2]);
    }

    [Fact]
    public void TestThatMapperFlatMapsCollections()
    {
        var list1 = new[] { "1", "2", "3" };
        var list2 = new[] { "4", "5", "6" };
        var list3 = new[] { "7", "8", "9" };

        var lists = new List<IEnumerable<string>>();
        lists.Add(list1);
        lists.Add(list2);
        lists.Add(list3);

        Func<List<IEnumerable<string>>, IEnumerable<int>> mapper = los =>
            los.SelectMany(list => list.Select(int.Parse));

        var results = new List<int>();
        var flatMapper = Operator<List<IEnumerable<string>>, IEnumerable<int>>.MapWith(mapper);
        flatMapper.PerformInto(lists, numbers => results.AddRange(numbers));

        Assert.Equal(9, results.Count);
        Assert.Equal(1, results[0]);
        Assert.Equal(2, results[1]);
        Assert.Equal(3, results[2]);
    }
        
    [Fact]
    public void TestThatMapperFlatMapsOptionals()
    {
        var list1 = new[] { Optional.Of("1"), Optional.Of("2"), Optional.Of("3"), Optional.Empty<string>() };
        var list2 = new[] { Optional.Of("4"), Optional.Empty<string>(), Optional.Of("5"), Optional.Of("6") };
        var list3 = new[] { Optional.Of("7"), Optional.Of("8"), Optional.Empty<string>(), Optional.Of("9") };

        var lists = new List<List<Optional<string>>> {list1.ToList(), list2.ToList(), list3.ToList()};

        var results = new List<Optional<int>>();
            
        Func<List<List<Optional<string>>>, List<Optional<int>>> mapper = los =>
            los.SelectMany(
                list => list.Where(s => s.IsPresent)
                    .Select(s => Optional.Of(int.Parse(s.Get())))).ToList();
            
        var flatMapper = Operator<List<List<Optional<string>>>, List<Optional<int>>>.MapWith(mapper);
            
        flatMapper.PerformInto(lists, numbers => results.AddRange(numbers));

        Assert.Equal(9, results.Count);
        Assert.Equal(1, results[0].Get());
        Assert.Equal(2, results[1].Get());
        Assert.Equal(3, results[2].Get());
    }
}