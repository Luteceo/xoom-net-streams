// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Streams;
using Vlingo.Xoom.Streams.Sink.Test;
using Xunit;
using Xunit.Abstractions;

namespace Vlingo.Xoom.Streams.Tests;

public class StreamProcessorTest : StreamPubSubTest
{
    private readonly SafeConsumerSink<int> _sink;
    private readonly StringToIntegerMapper _transformer;

    [Fact]
    public void TestThatProcessorPipesTransformation()
    {
        CreatePublisherWith(SourceOf123);
        var access = _transformer.AfterCompleting(6);
        var accessSink = _sink.AfterCompleting(12);
        var subscriber = CreateSubscriberWithoutSubscribing(_sink, 3);
        var processor = World.ActorFor(new[] { typeof(IProcessor<string, int>) },
                typeof(StreamProcessor<string, int>),
                _transformer, 3L, PublisherConfiguration.DefaultDropHead)
            .Get<IProcessor<string, int>>(0);
            
        processor.Subscribe(subscriber);
        Publisher.Subscribe(processor);
            
        var sinkValue = _sink.AccessValueMustBe("value", 3);
        Assert.Equal(3, sinkValue);

        var transformCount = access.ReadFrom<int>("transformCount");
        Assert.Equal(3, transformCount);

        var values = access.ReadFrom<List<int>>("values");
        var expected = IntegerListOf1To(transformCount);
        Assert.Equal(expected, values);
    }
        
    [Fact]
    public void TestThatProcessorPipesTransformationOfMany()
    {
        CreatePublisherWith(SourceRandomNumberOfElements);

        var accessTransformer = _transformer.AfterCompleting(200);

        var accessSink = _sink.AfterCompleting(102);

        var subscriber = CreateSubscriberWithoutSubscribing(_sink, 10);

        var processor = World.ActorFor<IProcessor<string, int>>(() => new StreamProcessor<string, int>(_transformer, 10, PublisherConfiguration.DefaultDropHead));

        processor.Subscribe(subscriber);
        Publisher.Subscribe(processor);

        var sinkValue = _sink.AccessValueMustBe("value", 100);

        Assert.Equal(100, sinkValue);

        var transformCount = accessTransformer.ReadFrom<int>("transformCount");

        Assert.Equal(100, transformCount);

        var values = accessTransformer.ReadFrom<IEnumerable<int>>("values").ToList();
        var expected = IntegerListOf1To(transformCount).ToList();

        for (var i = 0; i < values.Count; i++)
        {
            Assert.Equal(expected[i], values[i]);
        }
    }

    public StreamProcessorTest(ITestOutputHelper output)
    {
        var converter = new Converter(output);
        Console.SetOut(converter);
            
        _sink = new SafeConsumerSink<int>();
        _transformer = new StringToIntegerMapper();
    }
}