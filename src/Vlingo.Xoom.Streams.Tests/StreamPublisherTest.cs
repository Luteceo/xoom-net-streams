using System.Collections.Generic;
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

    [Fact]
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

      var completeCount = access.ReadFrom<int>("onComplete");
      Assert.Equal(1, completeCount);

      var valueCount = subscriber.AccessValueMustBe("onNext", 3);
      Assert.Equal(3, valueCount);

      var values = access.ReadFrom<List<string>>("values");
      var expected = new[] {"A", "B", "C"};
      Assert.Equal(expected, values);
    }

    [Fact]
    public void TestThatSubscriberReceivesTotalRandomNumberOfElements()
    {
      // GIVEN
      CreatePublisherWith(SourceRandomNumberOfElements);
      var subscriber = new TestSubscriber<string>(100);
      var access = subscriber.AfterCompleting(102);

      // WHEN
      Publisher.Subscribe(subscriber);

      // THEN
      var subscriberCount = access.ReadFrom<int>("onSubscribe");
      Assert.Equal(1, subscriberCount);

      var completeCount = subscriber.AccessValueMustBe("onComplete", 1);
      Assert.Equal(1, completeCount);

      var valueCount = subscriber.AccessValueMustBe("onNext", 100);
      Assert.Equal(100, valueCount);

      var values = access.ReadFrom<List<string>>("values");
      var expected = StringListOf1To(100);
      Assert.Equal(expected, values);
    }

    [Fact]
    public void TestThatSubscriberReceivesUpToCancel()
    {
      // GIVEN
      CreatePublisherWith(SourceRandomNumberOfElements);
      var subscriber = new TestSubscriber<string>(100, 50);
      var access = subscriber.AfterCompleting(50);

      // WHEN
      Publisher.Subscribe(subscriber);

      // THEN
      var subscriberCount = access.ReadFrom<int>("onSubscribe");
      Assert.Equal(1, subscriberCount);

      var valueCount = subscriber.AccessValueMustBe("onNext", 50);
      Assert.True(50 <= valueCount);

      var values = access.ReadFrom<List<string>>("values");
      var expected = StringListOf1To(valueCount);
      Assert.Equal(expected, values);
    }

    [Fact]
    public void TestThatSubscriberReceiversFromSlowSource()
    {
      // GIVEN
      SourceRandomNumberOfElements = new RandomNumberOfElementsSource(100, true);
      CreatePublisherWith(SourceRandomNumberOfElements);
      var subscriber = new TestSubscriber<string>(100);
      var access = subscriber.AfterCompleting(102);
      
      // WHEN
      Publisher.Subscribe(subscriber);
      
      // THEN
      var subscriberCount = access.ReadFrom<int>("onSubscribe");
      Assert.Equal(1, subscriberCount);

      var completeCount = subscriber.AccessValueMustBe("onComplete", 1);
      Assert.Equal(1, completeCount);

      var valueCount = subscriber.AccessValueMustBe("onNext", 100);
      Assert.Equal(100, valueCount);

      var values = access.ReadFrom<List<string>>("values");
      var expected = StringListOf1To(100);
      Assert.Equal(expected, values);
    }
  }
}