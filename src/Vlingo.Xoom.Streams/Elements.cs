namespace Vlingo.Xoom.Streams
{
  public class Elements<T> where T : class
  {
    private static readonly object[] _empty = new object[0];

    public readonly T[] Values;
    public readonly bool Terminated;

    public static Elements<T> Empty()
    {
      return new Elements<T>((T[]) _empty, false);
    }

    public Elements(T[] values, bool terminated)
    {
      Values = values;
      Terminated = terminated;
    }
  }
}