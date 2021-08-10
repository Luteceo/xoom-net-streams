using System;
using System.Linq;
using Nito.Collections;
using Reactive.Streams;
using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Common;
using Vlingo.Xoom.Streams.Sink;

namespace Vlingo.Xoom.Streams
{
  public sealed class StreamProcessor<T, TR> : Actor, IProcessor<T, TR>, IControlledSubscription<TR>,
    IScheduled<object>
  {
    private static StreamPublisherDelegate<TR> _publisherDelegate;
    private static PublisherSource _publisherSource;
    private static long _requestThreshold;
    private static StreamSubscriberDelegate<T> _subscriberDelegate;

    public StreamProcessor(Operator<T, TR> @operator, long requestThreshold, PublisherConfiguration configuration)
    {
      _requestThreshold = requestThreshold;
      _subscriberDelegate =
        new StreamSubscriberDelegate<T>(new ConsumerSink<T>(ConsumerOperator(@operator)), requestThreshold, Logger);
      _publisherSource = new PublisherSource();
      _publisherDelegate = new StreamPublisherDelegate<TR>(_publisherSource, configuration,
        SelfAs<IControlledSubscription<TR>>(), Scheduler, SelfAs<IScheduled<object>>(),
        SelfAs<IStoppable>());
    }

    public void OnSubscribe(ISubscription subscription)
    {
      _subscriberDelegate.OnSubscribe(subscription);
    }

    public void OnNext(T value)
    {
      _subscriberDelegate.OnNext(value);
    }

    public void OnError(Exception cause)
    {
      _publisherDelegate.Publish(cause);
      _subscriberDelegate.OnError(cause);

      _publisherSource.Terminate();
    }

    public void OnComplete()
    {
      _subscriberDelegate.OnComplete();
      _publisherSource.Terminate();
    }

    public void Subscribe(ISubscriber<TR> subscriber)
    {
      _publisherDelegate.Subscribe(subscriber);
    }

    public void Cancel(SubscriptionController<TR> controller)
    {
      _subscriberDelegate.CancelSubscription();
      _publisherDelegate.Cancel(controller);
    }

    public void Request(SubscriptionController<TR> subscription, long maximum)
    {
      throw new NotImplementedException();
    }

    public void IntervalSignal(IScheduled<object> scheduled, object data)
    {
      _publisherDelegate.ProcessNext();
    }

    public override void Stop()
    {
      _publisherSource.Terminate();
      base.Stop();
    }

    private class PublisherSource : Source<TR>
    {
      private bool _terminated;
      private readonly Deque<TR> _values;

      public PublisherSource()
      {
        _values = new Deque<TR>();
        _terminated = false;
      }

      public override ICompletes<Elements<TR>> Next()
      {
        return Next((int) _requestThreshold);
      }

      public override ICompletes<Elements<TR>> Next(int maximumElements)
      {
        if (!_values.Any())
        {
          if (_subscriberDelegate.IsFinalized() || _terminated)
            return Common.Completes.WithSuccess(Elements<TR>.Terminated());
          return Common.Completes.WithSuccess(Elements<TR>.Empty());
        }

        return Common.Completes.WithSuccess(Elements<TR>.Of(NextValue(maximumElements)));
      }

      private TR[]? NextValue(int maximum)
      {
        var elements = Math.Min(_values.Count, maximum);
        var nextValues = new object[elements] as TR[];
        for (var idx = 0; idx < nextValues?.Length; ++idx)
        {
          nextValues[idx] = _values.RemoveFromBack();
        }

        return nextValues;
      }

      public override ICompletes<Elements<TR>> Next(long index)
      {
        return Next((int) _requestThreshold);
      }

      public override ICompletes<Elements<TR>> Next(long index, int maximumElements)
      {
        return Next(maximumElements);
      }

      public override ICompletes<bool> IsSlow()
      {
        return Common.Completes.WithSuccess(false);
      }

      public void Enqueue(TR value)
      {
        _values.AddToFront(value);
      }

      public void Terminate()
      {
        _terminated = true;
      }
    }

    private Action<T> ConsumerOperator(Operator<T, TR> @operator) => delegate(T value)
    {
      try
      {
        @operator.PerformInto(value, (transformed) => _publisherSource.Enqueue(transformed));
      }
      catch (Exception e)
      {
        _publisherDelegate.Publish(e);
      }
    };
  }
}