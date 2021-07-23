using System.Collections.Generic;
using Vlingo.Xoom.Common;
using Vlingo.Xoom.Streams.Source;

namespace Vlingo.Xoom.Streams
{
  public abstract class Source<T> where T : class
  {
    static Source<T> Empty()
    {
      return new IterableSource<T>(new List<T>(0), false);
    }

    public static Source<T> Only(IEnumerable<T> elements)
    {
      return new IterableSource<T>(new List<T>(elements), false);
    }

    public abstract ICompletes<Elements<T>> Next();
    public abstract ICompletes<Elements<T>> Next(int maximumElements);
    public abstract ICompletes<Elements<T>> Next(long index);
    public abstract ICompletes<Elements<T>> Next(long index, int maximumElements);
    public abstract ICompletes<bool> IsSlow();
  }
}