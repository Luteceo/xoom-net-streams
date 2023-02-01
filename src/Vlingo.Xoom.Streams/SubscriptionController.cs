// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Streams;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams;

public class SubscriptionController<T> : ISubscription
{
    private static readonly AtomicInteger NextId = new AtomicInteger(0);

    private readonly Queue<Optional<T>> _buffer;
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
        _buffer = new Queue<Optional<T>>();
        _cancelled = false;
    }

    public void Cancel() => _subscription.Cancel(this);

    public void Request(long maximum)
    {
        //Console.WriteLine($"{GetType()} : {nameof(Request)}");
        if (maximum <= 0)
        {
            var exception = new ArgumentException("Must be >=1 and <= long.MaxValue.");
            _subscriber.OnError(exception);
            return;
        }

        _subscription.Request(this, maximum);
    }
        
    public override int GetHashCode() => 31 * Id.GetHashCode();
        
    public override bool Equals(object? other)
    {
        if (this == other)
        {
            return true;
        }

        if (other == null || GetType() != other.GetType())
        {
            return false;
        }

        return Id == ((SubscriptionController<T>) other).Id;
    }
        
    public override string ToString() => 
        $"SubscriptionController [id={Id} count={_count} maximum={_maximum} remaining={Remaining} unbounded={Unbounded}]";

    //===================================
    // Publish
    //===================================

    internal bool HasBufferedElements() => _buffer.Any();

    internal void OnNext(Optional<T> element)
    {
        //Console.WriteLine($"SubscriptionController | Element: {element} : Remaining: {Remaining}");
        //Console.WriteLine($"{GetType()} : {nameof(OnNext)}");
        if (Remaining > 0)
        {
            SendNext(element);
        }
        else if (!element.IsPresent)
        {
            // do nothing
        }
        else if (_buffer.Count < _configuration.BufferSize)
        {
            _buffer.Enqueue(element);
        }
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
        
    public void OnError(Exception cause) => _subscriber.OnError(cause);

    private void DropHeadFor(Optional<T> element)
    {
        _buffer.Dequeue();
        _buffer.Enqueue(element);
    }
        
    private void DropTailFor(Optional<T> element)
    {
        _dropIndex = 0;
        var lastElement = _buffer.Count - 1;
        foreach (var el in _buffer)
        {
            if (_dropIndex++ == lastElement)
            {
                _buffer.Dequeue();
            }
        }
        _buffer.Enqueue(element);
    }
        
    private void SendNext(Optional<T> element)
    {
        //Console.WriteLine($"SubscriptionController | REMAINING: {Remaining}");
        var throttleCount = ThrottleCount;
        //Console.WriteLine($"SubscriptionController | THROTTLE: {throttleCount}");
        var currentElement = element;
        while (throttleCount-- > 0)
        {
            var next = SwapBufferedOrElse(currentElement);
            if (next.IsPresent)
            {
                //Console.WriteLine($"SENDING: {next}");
                currentElement = Optional.Empty<T>();
                _subscriber.OnNext(next.Get());
                Increment();
            }
            else
            {
                break;
            }
        }
    }
        
    private Optional<T> SwapBufferedOrElse(Optional<T> element)
    {
        if (!_buffer.Any())
        {
            return element;
        }

        var next = _buffer.Dequeue();
        if (element.IsPresent)
        {
            _buffer.Enqueue(element);
        }

        return next;
    }
        
    //===================================
    // Back pressure
    //===================================
        
    public long Accumulate(long amount)
    {
        if (_maximum < long.MaxValue)
        {
            var accumulated = _maximum + amount;
            if (accumulated < 0)
            {
                accumulated = long.MaxValue;
            }

            //Console.WriteLine($"SubscriptionController | ACCUMULATE: CURRENT MAXIMUM: {_maximum} AMOUNT: {amount} ACCUMULATED: {accumulated}");
            return accumulated;
        }

        return _maximum;
    }
        
    public void CancelSubscription()
    {
        _cancelled = true;
        _count = 0;
        _maximum = 0;
    }
        
    public void RequestFlow(long maximum)
    {
        //Console.WriteLine($"{GetType()} : {nameof(RequestFlow)}");

        //  NOTE: Since Accumulate() works as follows:
        //  long accumulated = maximum + amount;
        //  it means that resetting count must NOT be done:
        //  _count = 0;
        _maximum = maximum;
    }
        
    private void Increment()
    {
        if (_count < _maximum)
        {
            ++_count;
        }
    }
        
    private long Remaining => _cancelled ? 0 : _maximum - _count;

    private long ThrottleCount => _configuration.MaxThrottle == Streams.DefaultMaxThrottle
        ? Remaining
        : Math.Min(_configuration.MaxThrottle, Remaining);
        
    private bool Unbounded => _maximum == long.MaxValue;
}