using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Streams;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams.Tests
{
  public abstract class StreamPubSubTest : IDisposable
  {
    protected PublisherConfiguration Configuration;
    protected IControlledSubscription<string> ControlledSubscription;
    protected Source<string> SourceOf123;
    protected Source<string> SourceOfABC;
    protected Source<string> SourceRandomNumberOfElements;
    protected IPublisher<string> Publisher;
    protected ISubscriber<string> Subscriber;
    protected World World;

    public StreamPubSubTest()
    {
      World = World.StartWithDefaults("streams");

      Configuration = new PublisherConfiguration(5, Streams.OverflowPolicy.DropHead);

      SourceOf123 = Source<string>.Only(new[] {"1", "2", "3"});

      SourceOfABC = Source<string>.Only(new[] {"A", "B", "C"});

      SourceRandomNumberOfElements = new RandomNumberOfElementsSource(100);
    }

    protected void CreatePublisherWith<T>(Source<T> source)
    {
      var definition = Definition.Has<StreamPublisher<T>>(Definition.Parameters(source, Configuration));

      var protocols = World.ActorFor(new[] {typeof(IPublisher<T>), typeof(IControlledSubscription<T>)}, definition);

      Publisher = protocols.Get<IPublisher<string>>(0);

      ControlledSubscription = protocols.Get<IControlledSubscription<string>>(1);
    }

    protected void CreateSubscriberWith<T>(Sink<T> sink, long requestThreshold)
    {
      var protocols = World.ActorFor(new[] {typeof(ISubscriber<T>)}, typeof(StreamSubscriber<T>), sink,
        requestThreshold);

      Subscriber = protocols.Get<ISubscriber<string>>(0);
      Publisher.Subscribe(Subscriber);
    }

    protected ISubscriber<T> CreateSubscriberWithoutSubscribing<T>(Sink<T> sink, long requestThreshold)
    {
      var protocols = World.ActorFor(new[] {typeof(ISubscriber<T>)}, typeof(StreamSubscriber<T>), sink,
        requestThreshold);

      var subscriber = protocols.Get<ISubscriber<T>>(0);
      return subscriber;
    }

    protected IEnumerable<string> StringListOf1To(int n) => Enumerable.Range(1, n)
      .Select(idx => $"{idx}");

    protected IEnumerable<int> IntegerListOf1To(int n) => Enumerable.Range(1, n);

    public void Dispose()
    {
      World.Terminate();
    }
  }
}