using System;
using System.Collections.Concurrent;
using Reactive.Streams;
using Vlingo.Xoom.Actors.TestKit;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Tests
{
  public class TestSubscriber<T> : ISubscriber<T>
  {
    private AccessSafely _access;

    private readonly AtomicInteger _onSubscribeCount = new AtomicInteger(0);
    private readonly AtomicInteger _onNextCount = new AtomicInteger(0);
    private readonly AtomicInteger _onErrorCount = new AtomicInteger(0);
    private readonly AtomicInteger _onCompleteCount = new AtomicInteger(0);

    private readonly BlockingCollection<T> _values = new BlockingCollection<T>();

    private readonly Sink<T> _sink;

    private bool _cancelled = false;
    private readonly int _cancelAfterElements;
    private readonly int _total;
    private ISubscription _subscription;

    public TestSubscriber(int total) : this(total, total * total)
    {
    }

    private TestSubscriber(int total, int cancelAfterElements) : this(null, total, cancelAfterElements)
    {
    }

    private TestSubscriber(Sink<T> sink, int total, int cancelAfterElements)
    {
      _sink = sink;
      _total = total;
      _cancelAfterElements = cancelAfterElements;

      _access = AfterCompleting(0);
    }

    public void OnSubscribe(ISubscription subscription)
    {
      if (_subscription == null)
      {
        subscription.Cancel();
        return;
      }

      _subscription = subscription;
      _access.WriteUsing("onSubscribe", 1);
      subscription.Request(_total);

      if (_sink != null)
        _sink.Ready();
    }

    public void OnNext(T element)
    {
      throw new NotImplementedException();
    }

    public void OnError(Exception cause)
    {
      throw new NotImplementedException();
    }

    public void OnComplete()
    {
      throw new NotImplementedException();
    }

    public AccessSafely AfterCompleting(int times)
    {
      _access = AccessSafely.AfterCompleting(times);

      _access.WritingWith("onSubscribe", (int value) => _onSubscribeCount.AddAndGet(value));
      _access.WritingWith("onNext", (int value) => _onNextCount.AddAndGet(value));
      _access.WritingWith("onError", (int value) => _onErrorCount.AddAndGet(value));
      _access.WritingWith("onComplete", (int value) => _onCompleteCount.AddAndGet(value));

      _access.WritingWith("values", (T values) => _values.Add(values));

      _access.ReadingWith("onSubscribe", () => _onCompleteCount.Get());
      _access.ReadingWith("onNext", () => _onNextCount.Get());
      _access.ReadingWith("onError", () => _onErrorCount.Get());
      _access.ReadingWith("onComplete", () => _onCompleteCount.Get());

      _access.ReadingWith("values", () => _values);
      
      return _access;
    }
  }
}