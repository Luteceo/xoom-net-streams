using System;
using System.Text;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams.Tests.Source
{
  public class SourceTest : IDisposable
  {
    private readonly StringBuilder _builder = new();

    protected string StringFromSource(Source<string> source)
    {
      string current = "";
      while (current != null)
      {
        var elements = source
          .Next()
          .Await();

        current = elements.ElementAt(0);

        if (current != null)
          _builder.Append(current);
      }

      var result = _builder.ToString();
      return result;
    }

    public void Dispose()
    {
    }
  }
}