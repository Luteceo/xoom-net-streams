using System;
using System.IO;
using Vlingo.Xoom.Streams.Sink;

namespace Vlingo.Xoom.Streams
{
  public abstract class Sink<T>
  {
    public static Sink<T> ConsumeWith(Action<T> consumer)
    {
      return new ConsumerSink<T>(consumer);
    }
    
    public static Sink<T> PrintTo(StreamWriter printStream, string prefix) => new PrintSink<T>(printStream, prefix);

    public abstract void Ready();
    public abstract void Terminate();
    public abstract void WhenValue(T value);
  }
}
