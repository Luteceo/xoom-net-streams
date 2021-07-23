namespace Vlingo.Xoom.Streams
{
  public class Streams
  {
    public static readonly int DEFAULT_BUFFER_SIZE = 256;
    public static readonly int DEFAULT_MAX_THROTTLE = -1;

    public enum OverflowPolicy
    {
      DROP_HEAD,
      DROP_TAIL,
      DROP_CURRENT
    }
  }
}