using System;
using Reactive.Streams;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams
{
  public class StreamSubscriberDelegate<T> : ISubscriber<T>
  {
    private Sink<T> _sink;
    private long _requestThreshold;
    private ILogger _logger;
    private long _count;
    private bool _completed;
    private bool _errored;
    private ISubscription _subscription;

    public StreamSubscriberDelegate(Sink<T> sink, long requestThreshold, ILogger logger)
    {
      _sink = sink;
      _requestThreshold = requestThreshold;
      _logger = logger;
      _count = 0;
      _completed = false;
      _errored = false;
    }

    public void OnSubscribe(ISubscription subscription)
    {
      if (_subscription == null)
      {
        _subscription = subscription;
        _sink.Ready();
        _subscription.Request(_requestThreshold);
      }
      else
        subscription.Cancel();
    }

    public void OnComplete()
    {
      //throw new NotImplementedException();
    }

    public void OnError(Exception cause)
    {
      //throw new NotImplementedException();
    }

    public void OnNext(T element)
    {
      //throw new NotImplementedException();
    }
  }
}