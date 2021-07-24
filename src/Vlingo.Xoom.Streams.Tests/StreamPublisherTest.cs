using Xunit;

namespace Vlingo.Xoom.Streams.Tests
{
  public class StreamPublisherTest : StreamPubSubTest
  {
    [Fact]
    public void TestThatSubscriberSubscribes()
    {
      // GIVEN
      CreatePublisherWith(SourceOfABC);
      var subscriber = new TestSubscriber<string>(3);
      var access = subscriber.AfterCompleting(1);

      // WHEN
      Publisher.Subscribe(subscriber);

      // THEN
      var subscriberCount = access.ReadFrom<int>("onSubscribe");
      Assert.Equal(1, subscriberCount);
    }

    [Fact(Skip = "WIP")]
    public void TestThatSubscriberReceives()
    {
      // GIVEN
      CreatePublisherWith(SourceOfABC);
      var subscriber = new TestSubscriber<string>(3);
      var access = subscriber.AfterCompleting(8);

      // WHEN
      Publisher.Subscribe(subscriber);

      // THEN
      var subscriberCount = access.ReadFrom<int>("onSubscribe");
      Assert.Equal(1, subscriberCount);
    }
  }
}