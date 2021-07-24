using System;
using Reactive.Streams;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams
{
  /// <summary>
  /// The standard <c>StreamSubscriber<T></c> of streams.
  /// </summary>
  /// <typeparam name="T">The type od value consumed</typeparam>
  public class StreamSubscriber<T> : Actor, ISubscriber<T>, IStoppable where T : class
  {
    private readonly ISubscriber<T> _subscriber;

    public StreamSubscriber(Sink<T> sink, long requestThreshold)
    {
      _subscriber = new StreamSubscriberDelegate<T>(sink, requestThreshold, Logger);
    }

    public void OnComplete()
    {
      _subscriber.OnComplete();
    }

    public void OnError(Exception cause)
    {
      _subscriber.OnError(cause);
    }

    public void OnNext(T element)
    {
      _subscriber.OnNext(element);
    }

    public void OnSubscribe(ISubscription subscription)
    {
      _subscriber.OnSubscribe(subscription);
    }
  }
}