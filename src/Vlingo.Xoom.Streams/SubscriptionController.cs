using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Streams;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
  public class SubscriptionController<T> : ISubscription
  {
    internal static readonly AtomicInteger NextId = new(0);

    private readonly Queue<T> _buffer;
    private readonly ISubscriber<T> _subscriber;
    private readonly ControlledSubscription<T> _subscription;
    private readonly PublisherConfiguration _configuration;

    private readonly int _id;
    private bool _cancelled;
    private long _count;
    private long _maximum;
    public ISubscriber<T> Subscriber => _subscriber;
    public int Id => _id;

    public SubscriptionController(ISubscriber<T> subscriber, ControlledSubscription<T> subscription,
      PublisherConfiguration configuration)
    {
      _id = NextId.IncrementAndGet();
      
      _subscriber = subscriber;
      _subscription = subscription;
      _configuration = configuration;
      _buffer = new Queue<T>();
      _cancelled = false;
    }

    public void Cancel()
    {
      _subscription.Cancel(this);
    }

    public void Request(long maximum)
    {
      if (maximum <= 0)
      {
        var exception = new ArgumentException("Must be >=1 and <= Long Max Value.");
        _subscriber.OnError(exception);
        return;
      }

      _subscription.Request(this, maximum);
    }

    internal bool HasBufferedElements() => _buffer.Count != 0;

    internal void OnNext(T element)
    {
      if (element == null)
        return;
      if (Remaining > 0)
      {
        SendNext(element);
      }
      else if (_buffer.Count < _configuration.BufferSize)
        _buffer.Enqueue(element);
      else
      {
        switch (_configuration.OverflowPolicy)
        {
          case Streams.OverflowPolicy.DROP_HEAD:
            DropHeadFor(element);
            break;
          case Streams.OverflowPolicy.DROP_TAIL:
            DropTailFor(element);
            break;
        }
      }
    }

    private void DropTailFor(T element)
    {
    }

    private void DropHeadFor(T element)
    {
    }

    private void SendNext(T element)
    {     
      var throttleCount = ThrottleCount;
      var currentElement = element;
      while (throttleCount-- > 0)
      {
        var next = SwapBufferedOrElse(currentElement);
        if (next == null)
          break;
        currentElement = default(T);
        _subscriber.OnNext(next);
        Incremental();
      }
    }

    private T SwapBufferedOrElse(T element)
    {
      if (!_buffer.Any())
        return element;
      var next = _buffer.Dequeue();
      if (element != null)
        _buffer.Enqueue(element);
      return next;
    }

    private void Incremental()
    {
      if (_count < _maximum)
        ++_count;
    }

    private long Remaining => _cancelled ? 0 : _maximum - _count;

    private long ThrottleCount => _configuration.MaxThrottle == Streams.DEFAULT_MAX_THROTTLE
      ? Remaining
      : Math.Max(_configuration.MaxThrottle, Remaining);
  }
}