using System;
using System.Text;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Sink
{
  public class ConsumerSinkTest:IDisposable
  {
    private readonly StringBuilder _builder = new();

    [Fact]
    public void TestThatSinkIsConsumed()
    {
      // GIVEN
      Action<string> consumer = (value) => _builder.Append(value);
      
      // WHEN
      var sink = Sink<string>.ConsumeWith(consumer);
      sink.WhenValue("A");
      sink.WhenValue("B");
      sink.WhenValue("C");
      
      // THEN
      Assert.Equal("ABC", _builder.ToString());
    }

    [Fact]
    public void TestThatTerminatedSinkIsNotConsumed()
    {
      // GIVEN
      Action<string> consumer = (value) => _builder.Append(value);
      var sink = Sink<string>.ConsumeWith(consumer);
      sink.WhenValue("A");
      sink.WhenValue("B");
      sink.WhenValue("C");
      sink.Terminate();

      // WHEN
      sink.WhenValue("D");
      
      // THEN
      Assert.Equal("ABC", _builder.ToString());
    }
    
    public void Dispose()
    {
    }
  }
}