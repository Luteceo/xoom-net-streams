using System;
using Vlingo.Xoom.Streams.Snik;

namespace Vlingo.Xoom.Streams
{
  public abstract class Sink<T>
  {
    public static Sink<T> ConsumeWith(Action<T> consumer)
    {
      return new ConsumerSink<T>(consumer);
    }

    public abstract void Ready();
  }
}