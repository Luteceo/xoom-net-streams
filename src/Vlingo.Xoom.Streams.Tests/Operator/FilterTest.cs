using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Operator
{
  public class FilterTest
  {
    [Fact]
    public void TestThatFilterFilters()
    {
      var filter = Operator<string, string>.FilterWith((s) => s.Contains("1"));
      
      var results = new List<string>();
      new[] {"ABC", "321", "123", "456", "DEF", "214"}
        .ToList()
        .ForEach(possible => filter.PerformInto(possible, (match) => results.Add(match)));
      
      Assert.Equal(3, results.Count);
      Assert.Equal("321", results[0]);
      Assert.Equal("123", results[1]);
      Assert.Equal("214", results[2]);
    }
  }
}