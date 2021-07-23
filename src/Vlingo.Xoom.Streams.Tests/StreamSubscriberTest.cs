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
  }
}