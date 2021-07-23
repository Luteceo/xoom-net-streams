using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Source
{
  public class IterableSourceTest : SourceTest
  {
    [Fact]
    public void TestThatEmptyHasNoElements()
    {
      // GIVEN
      var source = Source<string>.Empty();

      // WHEN
      var result = StringFromSource(source);

      //THEN
      Assert.True(string.IsNullOrEmpty(result));
    }

    [Fact]
    public void TestThatSourceProvidesElements()
    {
      // GIVEN
      var source = Source<string>.Only(new[] {"A", "B", "C"});

      // WHEN
      var result = StringFromSource(source);

      // THEN
      Assert.Equal("ABC", result);
    }

    [Fact]
    public void TestThatSourceCollectionProvidesElements()
    {
      // GIVEN
      var source = Source<string>.With(new[] {"A", "B", "C"});
      
      // WHEN
      var result = StringFromSource(source);
      
      // THEN
      Assert.Equal("ABC", result);
    }
  }
}