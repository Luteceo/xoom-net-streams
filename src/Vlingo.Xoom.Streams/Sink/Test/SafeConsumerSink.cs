using System;
using System.Collections.Generic;
using System.Threading;
using Vlingo.Xoom.Actors.TestKit;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Sink.Test
{
  public class SafeConsumerSink<T> : Sink<T>
  {
    private AccessSafely _access;
    private readonly AtomicInteger _readyCount = new (0);
    private readonly AtomicInteger _terminateCount = new (0);
    private readonly AtomicInteger _valueCount = new (0);

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

    public int AccessValueMustBe(string name, int expected)
    {
      Console.WriteLine(nameof(AccessValueMustBe));
      var current = 0;
      for (var tries = 0; tries < 10; ++tries)
      {
        var value = _access.ReadFrom<int>(name);
        if (value >= expected) return value;
        if (!current.Equals(value)) current = value;
        Thread.Sleep(100);
      }

      return expected == 0 ? -1 : current;
    }
    
    public override void Ready()
    {      Console.WriteLine($"G{GetType()} : { nameof(Ready)}");

      _access.WriteUsing("ready", 1);
    }

    public override void Terminate()
    {
      _access.WriteUsing("terminate", 1);
    }

    public override void WhenValue(T value)
    {
      _access.WriteUsing("value", 1);
      _access.WriteUsing("values", value);
    }
  }
}