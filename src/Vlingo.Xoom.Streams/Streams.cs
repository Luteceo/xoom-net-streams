namespace Vlingo.Xoom.Streams
{
  public static class Streams
  {
    public const int DefaultBufferSize = 256;
    public const int DefaultMaxThrottle = -1;

    public enum OverflowPolicy
    {
      DropHead,
      DropTail,
      DropCurrent
    }
  }
}