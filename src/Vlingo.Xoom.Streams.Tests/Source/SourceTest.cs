using System.Text;

namespace Vlingo.Xoom.Streams.Tests.Source
{
  public class SourceTest
  {
    private readonly StringBuilder _builder = new StringBuilder();

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
  }
}