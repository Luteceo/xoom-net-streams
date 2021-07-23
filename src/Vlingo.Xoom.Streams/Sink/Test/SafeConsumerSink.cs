using System.Collections.Generic;
using Vlingo.Xoom.Actors.TestKit;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Sink.Test
{
  public class SafeConsumerSink<T> : Sink<T> where T : class
  {
    private AccessSafely _access;
    private readonly AtomicInteger _readyCount = new AtomicInteger(0);
    private readonly AtomicInteger _terminateCount = new AtomicInteger(0);
    private readonly AtomicInteger _valueCount = new AtomicInteger(0);

    private readonly IList<T> _values = new List<T>();

    public SafeConsumerSink()
    {
      _access = AfterCompleting(0);
    }

    public AccessSafely AfterCompleting(int times)
    {
      _access = AccessSafely.AfterCompleting(times);

      _access.WritingWith("ready", (int value) => _readyCount.AddAndGet(value));
      _access.WritingWith("terminate", (int value) => _terminateCount.AddAndGet(value));
      _access.WritingWith("value", (int value) => _valueCount.AddAndGet(value));

      _access.WritingWith("values", (T value) => _values.Add(value));

      _access.ReadingWith("ready", () => _readyCount.Get());
      _access.ReadingWith("terminate", () => _terminateCount.Get());
      _access.ReadingWith("value", () => _valueCount.Get());

      _access.ReadingWith("values", () => _values);

      return _access;
    }

    public override void Ready()
    {
      _access.WriteUsing("ready", 1);
    }
  }
}