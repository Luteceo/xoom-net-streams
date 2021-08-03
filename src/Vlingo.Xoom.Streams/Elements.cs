namespace Vlingo.Xoom.Streams
{
  public class Elements<T>
  {
    private static readonly object[] _empty = new object[0];

    public readonly T[] Values;
    public readonly bool IsTerminated;

    public static Elements<T> Empty()
    {
      return new Elements<T>(_empty as T[], false);
    }
    public static Elements<T> Terminated()
    {
      return new Elements<T>(_empty as T[], true);
    }

    public Elements(T[] values, bool terminated)
    {
      Values = values;
      IsTerminated = terminated;
    }

    public T ElementAt(int index)
    {
      return Values.Length == 0 ? default : Values[index];
    }

    public static Elements<T> Of(T[] values)
    {
      return new Elements<T>(values, false);
    }
  }
}