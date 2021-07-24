using System.Collections.Generic;
using Vlingo.Xoom.Streams.Sink.Test;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests
{
  public class StreamSubscriberTest : StreamPubSubTest
  {
    private readonly SafeConsumerSink<string> _sink;

    public StreamSubscriberTest()
    {
      _sink = new SafeConsumerSink<string>();
    }

    [Fact]
    public void TestThatSubscriberSubscribes()
    {
      // GIVEN
      CreatePublisherWith(SourceOfABC);
      CreateSubscriberWith(_sink, 2);

      var access = _sink.AfterCompleting(1);

      // WHEN
      Publisher.Subscribe(Subscriber);

      // THEN
      var subscriberCount = access.ReadFrom<int>("ready");
      Assert.Equal(1, subscriberCount);
    }

    [Fact(Skip = "WIP")]
    public void TestThatSubscriberFeedsSink()
    {
      // GIVEN
      CreatePublisherWith(SourceOfABC);
      CreateSubscriberWith(_sink, 2);

      var access = _sink.AfterCompleting(5);
      
      // WHEN
      Publisher.Subscribe(Subscriber);
      
      // THEN
      var subscriberCount = access.ReadFrom<int>("ready");
      Assert.Equal(1, subscriberCount);
      var valueCount = _sink.AccessValueMustBe("value", 3);
      Assert.Equal(3, valueCount);
      var terminateCount = _sink.AccessValueMustBe("terminate", 1);
      Assert.Equal(1, terminateCount);
      var values = access.ReadFrom<List<string>>("values");
      Assert.Equal(3, values.Count);
      
      Assert.Equal("A", values[0]);
      Assert.Equal("B", values[1]);
      Assert.Equal("C", values[2]);
    }
  }
}