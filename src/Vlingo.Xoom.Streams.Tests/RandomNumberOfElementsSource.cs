using System;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Tests
{
  class RandomNumberOfElementsSource : Source<string>
  {
    private readonly AtomicInteger _element = new AtomicInteger(0);
    private readonly Random _count = new Random();
    private readonly int _total;
    private readonly bool _slow;

    public RandomNumberOfElementsSource(int total) : this(total, false)
    {
    }

    public RandomNumberOfElementsSource(int total, bool slow)
    {
      _total = total;
      _slow = slow;
    }

    public override ICompletes<bool> IsSlow() => Completes.WithSuccess(_slow);

    public override ICompletes<Elements<string>> Next()
    {
      throw new NotImplementedException();
    }

    public override ICompletes<Elements<string>> Next(int maximumElements)
    {
      throw new NotImplementedException();
    }

    public override ICompletes<Elements<string>> Next(long index)
    {
      throw new NotImplementedException();
    }

    public override ICompletes<Elements<string>> Next(long index, int maximumElements)
    {
      throw new NotImplementedException();
    }
  }
}