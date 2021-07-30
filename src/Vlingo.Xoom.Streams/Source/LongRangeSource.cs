using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Source
{
  public class LongRangeSource : Source<long>
  {
    private readonly long _startInclusive;
    private readonly long _endExclusive;
    private long _current;

    public LongRangeSource(long startInclusive, long endExclusive)
    {
      // Assert(startInclusive <= endExclusive)
      // Assert(startInclusive >= 0 && startInclusive <= long.Max)
      _startInclusive = startInclusive;
      // Assert(endExclusive >= 0 && endExclusive <= long.Max)
      _endExclusive = endExclusive;

      _current = startInclusive;
    }

    public override ICompletes<Elements<long>> Next()
    {
      if (_current >= _endExclusive)
        return Completes.WithSuccess(new Elements<long>(new long[0], true));
      var elements = new long[1];
      elements[0] = _current++;
      return Completes.WithSuccess(new Elements<long>(elements, false));
    }

    public override ICompletes<Elements<long>> Next(int maximumElements)
    {
      return Next();
    }

    public override ICompletes<Elements<long>> Next(long index)
    {
      return Next();
    }

    public override ICompletes<Elements<long>> Next(long index, int maximumElements)
    {
      return Next();
    }

    public override ICompletes<bool> IsSlow()
    {
      return Completes.WithSuccess(false);
    }
  }
}