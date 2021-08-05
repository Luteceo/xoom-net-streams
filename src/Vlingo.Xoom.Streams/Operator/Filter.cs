using System;

namespace Vlingo.Xoom.Streams.Operator
{
  public class Filter<T> : Operator<T, T>
  {
    private readonly Predicate<T> _predicate;

    public Filter(Predicate<T> predicate)
    {
      _predicate = predicate;
    }

    public override void PerformInto(T value, Action<T> consumer)
    {
      try
      {
        if (_predicate.Invoke(value))
          consumer.Invoke(value);
      }
      catch (Exception e)
      {
        Streams.Logger.Error($"Filter failed because: {e.Message}", e);
      }
    }
  }
}