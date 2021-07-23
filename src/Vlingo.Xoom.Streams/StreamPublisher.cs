using Reactive.Streams;
using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
  public class StreamPublisher<T> : Actor, IPublisher<T>, ControlledSubscription<T>, IScheduled<object>, IStoppable
    where T : class
  {
    private readonly StreamPublisherDelegate<T> _delegate;

    public StreamPublisher(Source<T> source, PublisherConfiguration configuration)
    {
      _delegate = new StreamPublisherDelegate<T>(source, configuration, SelfAs<ControlledSubscription<T>>(),
        Scheduler, SelfAs<IScheduled<object>>(), SelfAs<IStoppable>());
    }

    public void Cancel(SubscriptionController<T> subscription)
    {
      _delegate.Cancel(subscription);
    }

    public override void Stop()
    {
      _delegate.Stop();

      base.Stop();
    }

    public void IntervalSignal(IScheduled<object> scheduled, object data)
    {
      _delegate.ProcessNext();
    }

    public void Request(SubscriptionController<T> subscription, long maximum)
    {
      _delegate.Request(subscription, maximum);
    }

    public void Subscribe(ISubscriber<T> subscriber)
    {
      _delegate.Subscribe(subscriber);
    }
  }
}