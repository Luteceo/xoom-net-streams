using System;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams
{
  public static class Streams
  {
    private static ILogger? _logger;

    public const int DefaultBufferSize = 256;
    public const int DefaultMaxThrottle = -1;

    public enum OverflowPolicy
    {
      DropHead,
      DropTail,
      DropCurrent
    }

    public static ILogger Logger
    {
      set
      {
        if (_logger != null)
          throw new ArgumentException("Logger is already set.");
        _logger = value;
      }
      get => _logger ?? throw new ArgumentNullException("Logger is null.");
    }
  }
}