using System.Collections.Generic;
using Reactive.Streams;
using Vlingo.Xoom.Streams.Sink.Test;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests
{
  public class StreamProcessorTest : StreamPubSubTest
  {
    private readonly SafeConsumerSink<int> _sink;
    private readonly StringToIntegerMapper _transformer;

    [Fact]
    public void TestThatProcessorPipesTransformation()
    {
      // GIVEN
      CreatePublisherWith(SourceOf123);
      var access = _transformer.AfterCompleting(6);
      var subscriber = CreateSubscriberWithoutSubscribing(_sink, 3);
      var processor = World.ActorFor(new[] {typeof(IProcessor<string, int>)}, typeof(StreamProcessor<string, int>),
        _transformer, 3L, PublisherConfiguration.DefaultDropHead)
        .Get<IProcessor<string, int>>(0);

      // WHEN
      processor.Subscribe(subscriber);
      Publisher.Subscribe(processor);

      // THEN
      var transformCount = access.ReadFrom<int>("transformCount");
      Assert.Equal(3, transformCount);

      var values = access.ReadFrom<List<int>>("values");
      var expected = IntegerListOf1To(transformCount);
      Assert.Equal(expected, values);
    }

    public StreamProcessorTest()
    {
      _sink = new SafeConsumerSink<int>();
      _transformer = new StringToIntegerMapper();
    }
  }
}