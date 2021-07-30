using System;
using Vlingo.Xoom.Streams.Operator;

namespace Vlingo.Xoom.Streams
{
  public abstract class Operator<T, R>
  {
    public static Operator<T, T> FilterWith(Predicate<T> filter) => new Filter<T>(filter);
    public static Operator<T, R> MapWith(Func<T, R> mapper) => new Mapper<T, R>(mapper);
    public static Operator<T, R> FlatMapWith(Func<T, Source<R>> mapper) => new FlatMapper<T, R>(mapper);
    public abstract void PerformInto(T value, Action<R> consumer);
  }
}