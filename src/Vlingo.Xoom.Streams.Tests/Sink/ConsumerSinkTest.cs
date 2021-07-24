using System;
using System.Text;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Sink
{
  public class ConsumerSinkTest
  {
    private readonly StringBuilder _builder = new StringBuilder();

    [Fact]
    public void TestThatSinkIsConsumed()
    {
      Action<string> consumer = (value) => _builder.Append(value);

      var sink = Sink<string>.ConsumeWith(consumer);
      sink.WhenValue("A");
      sink.WhenValue("B");
      sink.WhenValue("C");

      Assert.Equal("ABC", _builder.ToString());
    }

    [Fact]
    public void TestThatTerminatedSinkIsNotConsumed()
    {
      Action<string> consumer = (value) => _builder.Append(value);
      var sink = Sink<string>.ConsumeWith(consumer);
      sink.WhenValue("A");
      sink.WhenValue("B");
      sink.WhenValue("C");
      sink.Terminate();

      sink.WhenValue("D");

      Assert.Equal("ABC", _builder.ToString());
    }
  }
}