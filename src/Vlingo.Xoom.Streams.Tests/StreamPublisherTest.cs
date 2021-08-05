using Xunit;

namespace Vlingo.Xoom.Streams.Tests
{
  public class StreamPublisherTest : StreamPubSubTest
  {
    [Fact]
    public void TestThatSubscriberSubscribes()
    {
      CreatePublisherWith(SourceOfABC);
      var subscriber = new TestSubscriber<string>(3);
      var access = subscriber.AfterCompleting(1);

      Publisher.Subscribe(subscriber);

      var subscriberCount = access.ReadFrom<int>("onSubscribe");
      Assert.Equal(1, subscriberCount);
    }

    [Fact(Skip = "WIP")]
    public void TestThatSubscriberReceives()
    {
      CreatePublisherWith(SourceOfABC);
      var subscriber = new TestSubscriber<string>(3);
      var access = subscriber.AfterCompleting(8);

      Publisher.Subscribe(subscriber);

      var subscriberCount = access.ReadFrom<int>("onSubscribe");
      Assert.Equal(1, subscriberCount);
    }
  }
}