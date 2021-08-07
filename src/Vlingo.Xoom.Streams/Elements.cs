namespace Vlingo.Xoom.Streams
{
  public class Elements<T>
  {
    private static readonly object[] _empty = new object[0];

    public readonly T[] Values;
    public readonly bool Terminated;

    public static Elements<T> Empty()
    {
      return new Elements<T>(_empty as T[], false);
    }

    public Elements(T[] values, bool terminated)
    {
      Values = values;
      Terminated = terminated;
    }

    public T ElementAt(int index)
    {
      return Values.Length == 0 ? default : Values[index];
    }
  }
}