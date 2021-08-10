using System;
using Vlingo.Xoom.Streams.Sink;

namespace Vlingo.Xoom.Streams
{
  public abstract class Sink<T>
  {
    public static Sink<T> ConsumeWith(Action<T> consumer)
    {
      return new ConsumerSink<T>(consumer);
    }

    public abstract void Ready();
    public abstract void Terminate();
    public abstract void WhenValue(T value);
  }
}