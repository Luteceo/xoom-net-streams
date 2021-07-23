using System;

namespace Vlingo.Xoom.Streams.Snik
{
  public class ConsumerSink<T> : Sink<T>
  {
    private readonly Action<T> _consumer;
    private bool _terminated;

    public ConsumerSink(Action<T> consumer)
    {
      _consumer = consumer;
      _terminated = false;
    }

    public override void Ready()
    {
      throw new NotImplementedException();
    }

    public override void Terminate()
    {
      _terminated = true;
    }

    public override void WhenValue(T value)
    {
      if (!_terminated)
        _consumer.Invoke(value);
    }
  }
}