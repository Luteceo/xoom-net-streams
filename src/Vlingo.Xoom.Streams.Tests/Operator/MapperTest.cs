using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Operator
{
  public class MapperTest
  {
    [Fact]
    public void TestThatMappersMaps()
    {
      var mapper = Operator<string, int>.MapWith(int.Parse);

      var results = new List<int>();
      new[] {"123", "456", "789"}
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
      var list1 = new[] {"1", "2", "3"};
      var list2 = new[] {"4", "5", "6"};
      var list3 = new[] {"7", "8", "9"};

      var lists = new List<IEnumerable<string>>();
      lists.Add(list1);
      lists.Add(list2);
      lists.Add(list3);

      Func<List<IEnumerable<string>>, IEnumerable<int>> mapper = (los) =>
        los.SelectMany(list => list.Select(int.Parse));

      var results = new List<int>();
      var flatMapper = Operator<List<IEnumerable<string>>, IEnumerable<int>>
        .MapWith(mapper);
      flatMapper.PerformInto(lists, (numbers) => results.AddRange(numbers));

      Assert.Equal(9, results.Count);
      Assert.Equal(1, results[0]);
      Assert.Equal(2, results[1]);
      Assert.Equal(3, results[2]);
    }
  }
}