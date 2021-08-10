using System;
using Reactive.Streams;
using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
  public sealed class StreamPublisher<T> : Actor, IPublisher<T>, IControlledSubscription<T>, IScheduled<object>
  {
    private readonly StreamPublisherDelegate<T> _delegate;

    public StreamPublisher(Source<T> source, PublisherConfiguration configuration)
    {
      _delegate = new StreamPublisherDelegate<T>(source, configuration, SelfAs<IControlledSubscription<T>>(),
        Scheduler, SelfAs<IScheduled<object>>(), SelfAs<IStoppable>());
    }

    public void Cancel(SubscriptionController<T> subscription)
    {
      _delegate.Cancel(subscription);
    }

    public void IntervalSignal(IScheduled<object> scheduled, object data)
    {
      _delegate.ProcessNext();
    }

    public void Request(SubscriptionController<T> subscription, long maximum)
    {
      Console.WriteLine($"{GetType()} : {nameof(Request)}");

      _delegate.Request(subscription, maximum);
    }

    public void Subscribe(ISubscriber<T> subscriber)
    {
      Console.WriteLine($"2: {GetType()} : {nameof(Subscribe)}");
      _delegate.Subscribe(subscriber);
    }

    public override void Stop()
    {
      _delegate.Stop();

      base.Stop();
    }
  }
}