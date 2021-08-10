using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Streams;
using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
  public class StreamPublisherDelegate<T> : IPublisher<T>, IControlledSubscription<T>
  {
    private Source<T> _source;
    private PublisherConfiguration _configuration;
    private IControlledSubscription<T> _controlledSubscription;
    private Scheduler _scheduler;
    private IScheduled<object> _scheduled;
    private IStoppable _stoppable;

    private readonly Dictionary<int, SubscriptionController<T>> _subscriptions;

    private bool _slow;
    private ICancellable _cancellable;

    public StreamPublisherDelegate(Source<T> source, PublisherConfiguration configuration,
      IControlledSubscription<T> controlledSubscription, Scheduler scheduler,
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

    public void Subscribe(ISubscriber<T> subscriber)
    {
      Console.WriteLine($"3: {GetType()} : {nameof(Subscribe)}");
      Schedule(true);

      var controller = new SubscriptionController<T>(subscriber, _controlledSubscription, _configuration);

      if (!_subscriptions.ContainsKey(controller.Id))
        _subscriptions.Add(controller.Id, controller);

      subscriber.OnSubscribe(controller);
    }

    public void Cancel(SubscriptionController<T> controller)
    {
      controller.CancelSubscription();
      _subscriptions.Remove(controller.Id);
    }

    public void Request(SubscriptionController<T> controller, long maximum)
    {
      Console.WriteLine($"{GetType()} : {nameof(Request)}");

      controller.RequestFlow(controller.Accumulate(maximum));

      Publish(controller, default);
    }

    internal void ProcessNext()
    {
      if (!_subscriptions.Any())
        return;
      try
      {
        _source
          .Next()
          .AndThen(maybeElements =>
          {
            if (!maybeElements.IsTerminated)
            {
              Publish(maybeElements.Values);
              Schedule(false);
              return maybeElements;
            }
            else
            {
              // if (Flush()) return maybeElements;
              CompleteAll();
              _stoppable.Stop();
            }
            
            return maybeElements;
          })
          .Await();
      }
      catch (Exception e)
      {
        Publish(e);
      }
    }

    public void Publish(Exception cause)
    {
      foreach (var controller in _subscriptions.Values)
        controller.OnError(cause);
    }

    private T[] Publish(T[] maybeElements)
    {
      Console.WriteLine($"{GetType()} : T[] {nameof(Publish)}");

      if (maybeElements.Any())
        for (var idx = 0; idx < _subscriptions.Values.Count; ++idx)
          Publish(maybeElements[idx]);

      return maybeElements;
    }

    void Publish(T elementOrNull)
    {
      foreach (var controller in _subscriptions.Values)
        controller.OnNext(elementOrNull);
    }

    void Publish(SubscriptionController<T> controller, T elementOrNull)
    {
      Console.WriteLine($"{GetType()} : {nameof(Publish)}");
      controller.OnNext(elementOrNull);
    }

    private bool _flushed;

    private bool Flush()
    {
      Console.WriteLine($"{GetType()} : {nameof(Flush)}");

      _flushed = false;
      foreach (var controller in _subscriptions.Values.Where(controller => controller.HasBufferedElements()))
      {
        controller.OnNext(default);
        _flushed = true;
      }

      return _flushed;
    }

    public void Stop()
    {
      _cancellable.Cancel();
      CompleteAll();
    }

    private void CompleteAll()
    {
      Console.WriteLine($"{GetType()} : {nameof(CompleteAll)}");

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
      else
      {
        if (isSubscribing && _cancellable == null)
          _cancellable = _scheduler.Schedule<object>(_scheduled, default, new TimeSpan(0),
            new TimeSpan(_configuration.ProbeInterval));
      }
    }
  }
}