using System;

namespace Vlingo.Xoom.Streams.Operator
{
  public class FlatMapper<T, R> : Operator<T, R>
  {
    private readonly Func<T, Source<R>> _mapper;
    private const int MaximumBuffer = 32;

    public FlatMapper(Func<T, Source<R>> mapper)
    {
      _mapper = mapper;
    }

    public override void PerformInto(T value, Action<R> consumer)
    {
      try
      {
        var result = _mapper.Invoke(value);
        PropagateSource(result, consumer);
      }
      catch (Exception e)
      {
        Streams.Logger.Error($"FlatMapper failed because: {e.Message}", e);
      }
    }

    private static void PropagateSource(Source<R> source, Action<R> consumer)
    {
      source
        .Next(MaximumBuffer)
        .AndThenConsume(elements =>
        {
          foreach (var element in elements.Values)
            consumer.Invoke(element);
          if (!elements.Terminated)
            PropagateSource(source, consumer);
        });
    }
  }
}