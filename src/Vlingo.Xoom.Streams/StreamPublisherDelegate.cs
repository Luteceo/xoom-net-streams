using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Streams;
using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
  public class StreamPublisherDelegate<T> : IPublisher<T>, ControlledSubscription<T> where T : class
  {
    private Source<T> _source;
    private PublisherConfiguration _configuration;
    private ControlledSubscription<T> _controlledSubscription;
    private Scheduler _scheduler;
    private IScheduled<object> _scheduled;
    private IStoppable _stoppable;

    private readonly Dictionary<int, SubscriptionController<T>> _subscriptions;

    private bool _slow;
    private ICancellable _cancellable;

    public StreamPublisherDelegate(Source<T> source, PublisherConfiguration configuration,
      ControlledSubscription<T> controlledSubscription, Scheduler scheduler,
      IScheduled<object> scheduled, IStoppable stoppable)
    {
      _source = source;
      _configuration = configuration;
      _controlledSubscription = controlledSubscription;
      _scheduler = scheduler;
      _scheduled = scheduled;
      _stoppable = stoppable;

      _subscriptions = new Dictionary<int, SubscriptionController<T>>(2);

      DetermineIfSlow();
    }

    internal void ProcessNext()
    {
      if (!_subscriptions.Any())
        return;
      _source
        .Next()
        .AndThen(maybeElements =>
        {
          if (!maybeElements.Terminated)
          {
            Publish(maybeElements.Values);
            Schedule(false);
            return maybeElements;
          }
          else
          {
            if (Flush()) return maybeElements;
            CompleteAll();
            _stoppable.Stop();
          }

          return maybeElements;
        })
        .Await();
    }

    private T[] Publish(T[] maybeElements)
    {
      if (maybeElements.Any())
      {
        foreach (var controller in _subscriptions.Values.Select((value, idx) =>
        { 
          Publish(maybeElements[idx]);
          return value;
        }));
      }

      return maybeElements;
    }

    T Publish(T elementOrNull)
    {
      foreach (var controller in _subscriptions.Values.Select(controller =>
      {
        controller.OnNext(elementOrNull);
        return controller;
      }));
      return elementOrNull;
    }

    private bool _flushed;

    private bool Flush()
    {
      _flushed = false;
      foreach (var controller in _subscriptions.Values.Select(controller =>
      {
        if (controller.HasBufferedElements())
        {
          controller.OnNext(null);
          _flushed = true;
        }

        return controller;
      }));
      return _flushed;
    }

    public void Cancel(SubscriptionController<T> subscription)
    {
    }

    public void Request(SubscriptionController<T> subscription, long maximum)
    {
    }

    public void Subscribe(ISubscriber<T> subscriber)
    {
      Schedule(true);

      var controller = new SubscriptionController<T>(subscriber, _controlledSubscription, _configuration);

      if (!_subscriptions.ContainsKey(controller.Id))
        _subscriptions.Add(controller.Id, controller);

      subscriber.OnSubscribe(controller);
    }

    public void Stop()
    {
      _cancellable.Cancel();
      CompleteAll();
    }

    private void CompleteAll()
    {
      foreach (var controller in _subscriptions.Values)
        controller.Subscriber.OnComplete();

      _subscriptions.Clear();
    }

    private void DetermineIfSlow()
    {
      _slow = _source.IsSlow().Await();
    }

    private void Schedule(bool isSubscribing)
    {
      if (_slow)
        _cancellable = _scheduler.ScheduleOnce<object>(_scheduled, default, new TimeSpan(0),
          new TimeSpan(_configuration.ProbeInterval));
      else if (isSubscribing && _cancellable == null)
        _cancellable = _scheduler.Schedule<object>(_scheduled, default, new TimeSpan(0),
          new TimeSpan(_configuration.ProbeInterval));
    }
  }
}