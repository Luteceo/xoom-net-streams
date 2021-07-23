using static Vlingo.Xoom.Streams.Streams;

namespace Vlingo.Xoom.Streams
{
  public class PublisherConfiguration
  {
    public static readonly int DEFAULT_PROBE_INTERVAL = 5;
    public static readonly int FAST_PROBE_INTERVAL = 2;
    public static readonly int FASTEST_PROBE_INTERVAL = 1;

    public readonly int BufferSize;
    public readonly int MaxThrottle;
    public readonly OverflowPolicy OverflowPolicy;
    public readonly int ProbeInterval;

    public static PublisherConfiguration DefaultDropCurrent() =>
      new(DEFAULT_PROBE_INTERVAL, DEFAULT_MAX_THROTTLE, DEFAULT_BUFFER_SIZE,
        OverflowPolicy.DROP_CURRENT);

    public PublisherConfiguration(int probeInterval, int maxThrottle, int bufferSize, OverflowPolicy overflowPolicy)
    {
      // Assert(ProbeInterval > 0)
      ProbeInterval = probeInterval;
      // Assert(maxThrottle == Streams.DEFAULT_MAX_THROTTLE || maxThrottle > 0)
      MaxThrottle = maxThrottle;
      // Assert(bufferSize > 0)
      BufferSize = bufferSize;
      // Assert(overflowpolicy != null)
      OverflowPolicy = overflowPolicy;
    }

    public PublisherConfiguration(int probeInterval, OverflowPolicy overflowPolicy) : this(probeInterval,
      DEFAULT_MAX_THROTTLE, DEFAULT_BUFFER_SIZE, overflowPolicy)
    {
    }
  }
}