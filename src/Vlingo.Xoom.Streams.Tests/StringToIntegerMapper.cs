using System;
using System.Collections.Concurrent;
using Vlingo.Xoom.Actors.TestKit;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Tests
{
  public class StringToIntegerMapper : Operator<string, int>
  {
    private AccessSafely _access = AccessSafely.AfterCompleting(0);
    private readonly AtomicInteger _transformCount = new AtomicInteger(0);
    private readonly BlockingCollection<int> _values = new BlockingCollection<int>();

    public override void PerformInto(string value, Action<int> consumer)
    {
      var amount = int.Parse(value);
      _access.WriteUsing("values", amount);
      _access.WriteUsing("transformCount", 1);
      consumer.Invoke(amount);
    }

    public AccessSafely AfterCompleting(int times)
    {
      _access = AccessSafely.AfterCompleting(times);
      _access.WritingWith("transformCount", (int value) => _transformCount.AddAndGet(value));
      _access.WritingWith("values", (int value) => _values.Add(value));

      _access.ReadingWith("transformCount", () => _transformCount.Get());
      _access.ReadingWith("values", () => _values);
      return _access;
    }
  }
}