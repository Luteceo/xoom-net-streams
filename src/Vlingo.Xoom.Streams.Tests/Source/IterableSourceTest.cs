using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Source
{
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
      var source = Source<string>.Only(new[] {"A", "B", "C"});

      var result = StringFromSource(source);

      Assert.Equal("ABC", result);
    }

    [Fact]
    public void TestThatSourceCollectionProvidesElements()
    {
      var source = Source<string>.With(new[] {"A", "B", "C"});

      var result = StringFromSource(source);

      Assert.Equal("ABC", result);
    }
  }
}