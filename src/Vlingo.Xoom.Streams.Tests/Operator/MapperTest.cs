using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Operator
{
  public class MapperTest : IDisposable
  {
    [Fact]
    public void TestThatMappersMaps()
    {
      // GIVEN
      var mapper = Operator<string, int>.MapWith(int.Parse);

      // WHEN
      var results = new List<int>();
      new[] {"123", "456", "789"}
        .ToList()
        .ForEach(digits => mapper.PerformInto(digits, number => results.Add(number)));

      // THEN
      Assert.Equal(3, results.Count);
      Assert.Equal(123, results[0]);
      Assert.Equal(456, results[1]);
      Assert.Equal(789, results[2]);
    }

    public void Dispose()
    {
    }
  }
}