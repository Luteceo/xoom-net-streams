using System;

namespace Vlingo.Xoom.Streams.Operator
{
  public class Mapper<T, R> : Operator<T, R>
  {
    private readonly Func<T, R> _mapper;

    public Mapper(Func<T, R> mapper)
    {
      _mapper = mapper;
    }

    public override void PerformInto(T value, Action<R> consumer)
    {
      try
      {
        consumer.Invoke(_mapper.Invoke(value));
      }
      catch (Exception e)
      {
        Streams.Logger.Error($"Mapper failed because: {e.Message}", e);
      }
    }
  }
}