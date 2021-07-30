using System;
using System.Collections.Generic;
using Vlingo.Xoom.Streams.Source;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Operator
{
  public class FlatMapperTest : IDisposable
  {
    [Fact]
    public void TestThatPropagatesRecordsFormTheProvidedSource()
    {
      // GIVEN
      var flatMapper = Operator<long, long>.FlatMapWith((record) => new LongRangeSource(record, record + 2));
      var providedLongs = new List<long>();

      // WHEN
      flatMapper.PerformInto(1L, providedLongs.Add);
      flatMapper.PerformInto(3L, providedLongs.Add);

      // THEN
      Assert.Equal(new[] {1L, 2L, 3L, 4L}, providedLongs.ToArray());
    }

    public void Dispose()
    {
    }
  }
}