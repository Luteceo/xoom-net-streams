using System;
using System.Collections.Generic;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Source
{
  public class IterableSource<T> : Source<T> 
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
      Console.WriteLine($"G{GetType()} : {nameof(Next)}");
      if (!_iterator.MoveNext()) return Completes.WithSuccess(new Elements<T>(new T[0], false));

      var elements = new T[1];
      elements[0] = _iterator.Current;
      return Completes.WithSuccess(new Elements<T>(elements, false));
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