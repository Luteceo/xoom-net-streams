using static Vlingo.Xoom.Streams.Streams;

namespace Vlingo.Xoom.Streams
{
  public class PublisherConfiguration
  {
    public const int DefaultProbeInterval = 5;
    public const int FastProbeInterval = 2;
    public const int FastestProbeInterval = 1;

    public readonly int BufferSize;
    public readonly int MaxThrottle;
    public readonly OverflowPolicy OverflowPolicy;
    public readonly int ProbeInterval;

    public static PublisherConfiguration DefaultDropCurrent() =>
      new(DefaultProbeInterval, DefaultMaxThrottle, DefaultBufferSize,
        OverflowPolicy.DropCurrent);

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
      DefaultMaxThrottle, DefaultBufferSize, overflowPolicy)
    {
    }
  }
}