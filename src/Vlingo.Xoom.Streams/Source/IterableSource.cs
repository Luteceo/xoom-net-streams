using System.Collections.Generic;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Source
{
  public class IterableSource<T> : Source<T> where T : class
  {
    private readonly IEnumerator<T> _iterator;
    private readonly bool _slowIterable;

    public IterableSource(IEnumerable<T> iterator, bool slowIterable)
    {
      _iterator = iterator.GetEnumerator();
      _slowIterable = slowIterable;
    }

    public override ICompletes<Elements<T>> Next()
    {
      if (_iterator.MoveNext())
      {
        var elements = new T[1];
        elements[0] = _iterator.Current;
        return Completes.WithSuccess(new Elements<T>(elements, false));
      }

      return Completes.WithSuccess(new Elements<T>(new T[0], false));
    }

    public override ICompletes<Elements<T>> Next(int maximumElements)
    {
      return Next();
    }

    public override ICompletes<Elements<T>> Next(long index)
    {
      return Next();
    }

    public override ICompletes<Elements<T>> Next(long index, int maximumElements)
    {
      return Next();
    }

    public override ICompletes<bool> IsSlow()
    {
      return Completes.WithSuccess(_slowIterable);
    }
  }
}