using System;
using System.Linq;
using Nito.Collections;
using Reactive.Streams;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
  public class SubscriptionController<T> : ISubscription
  {
    private static readonly AtomicInteger NextId = new(0);

    private readonly Deque<T> _buffer;
    private readonly ISubscriber<T> _subscriber;
    private readonly IControlledSubscription<T> _subscription;
    private readonly PublisherConfiguration _configuration;

    private bool _cancelled;
    private long _count;
    private long _maximum;
    private int _dropIndex;
    public ISubscriber<T> Subscriber => _subscriber;
    public int Id { get; }

    public SubscriptionController(ISubscriber<T> subscriber, IControlledSubscription<T> subscription,
      PublisherConfiguration configuration)
    {
      Id = NextId.IncrementAndGet();

      _subscriber = subscriber;
      _subscription = subscription;
      _configuration = configuration;
      _buffer = new Deque<T>();
      _cancelled = false;
    }

    public void Cancel()
    {
      _subscription.Cancel(this);
    }

    public void CancelSubscription()
    {
      _cancelled = true;
      _count = 0;
      _maximum = 0;
    }

    public void Request(long maximum)
    {
      Console.WriteLine($"{GetType()} : {nameof(Request)}");
      if (maximum <= 0)
      {
        var exception = new ArgumentException("Must be >=1 and <= long.MaxValue.");
        _subscriber.OnError(exception);
        return;
      }

      _subscription.Request(this, maximum);
    }

    internal bool HasBufferedElements() => _buffer.Any();

    internal void OnNext(T element)
    {
      Console.WriteLine($"Element: {element} : Remaining:{Remaining}");

      Console.WriteLine($"{GetType()} : {nameof(OnNext)}");
      if (Remaining > 0)
      {
        SendNext(element);
      }
      else if (element == null)
        return;
      else if (_buffer.Count < _configuration.BufferSize)
        _buffer.AddToFront(element);
      else
      {
        switch (_configuration.OverflowPolicy)
        {
          case Streams.OverflowPolicy.DropHead:
            DropHeadFor(element);
            break;
          case Streams.OverflowPolicy.DropTail:
            DropTailFor(element);
            break;
          case Streams.OverflowPolicy.DropCurrent: break;
        }
      }
    }

    private void DropTailFor(T element)
    {
      _dropIndex = 0;
      var lastElement = _buffer.Count - 1;
      _buffer.ToList().RemoveAll(e => _dropIndex++ == lastElement);
      _buffer.AddToFront(element);
    }

    private void DropHeadFor(T element)
    {
      _buffer.RemoveFromFront();
      _buffer.AddToFront(element);
    }

    private void SendNext(T element)
    {
      var throttleCount = ThrottleCount;
      var currentElement = element;
      while (throttleCount-- > 0)
      {
        var next = SwapBufferedOrElse(currentElement);
        if (next != null)
        {
          currentElement = default(T);
          _subscriber.OnNext(next);
          Increment();
        }
        else
        {
          break;
        }
      }
    }

    private T SwapBufferedOrElse(T element)
    {

      if (!_buffer.Any())
        return element;
      
      var next = _buffer.RemoveFromFront();
      if (element != null)
        _buffer.AddToFront(element);

      return next;
    }

    private void Increment()
    {
      if (_count < _maximum)
        ++_count;
    }

    private long Remaining => _cancelled ? 0 : _maximum - _count;

    private long ThrottleCount => _configuration.MaxThrottle == Streams.DefaultMaxThrottle
      ? Remaining
      : Math.Min(_configuration.MaxThrottle, Remaining);

    public long Accumulate(long amount)
    {
      if (_maximum >= long.MaxValue) return _maximum;
      
      var accumulated = _maximum + amount;
      if (accumulated < 0) {
        accumulated = long.MaxValue;
      }
      return accumulated;
    }

    public void RequestFlow(long maximum)
    {
      Console.WriteLine($"{GetType()} : {nameof(RequestFlow)}");

      _maximum = maximum;
    }

    public void OnError(Exception cause)
    {
      _subscriber.OnError(cause);
    }
  }
}