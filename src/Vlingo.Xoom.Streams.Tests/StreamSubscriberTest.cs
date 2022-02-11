// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Vlingo.Xoom.Streams.Sink.Test;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests;

public class StreamSubscriberTest : StreamPubSubTest
{
    private readonly SafeConsumerSink<string> _sink;

    public StreamSubscriberTest() => _sink = new SafeConsumerSink<string>();

    [Fact]
    public void TestThatSubscriberSubscribes()
    {
        CreatePublisherWith(SourceOfABC);
        CreateSubscriberWith(_sink, 2);

        var access = _sink.AfterCompleting(1);

        Publisher.Subscribe(Subscriber);

        var subscriberCount = access.ReadFrom<int>("ready");
        Assert.Equal(1, subscriberCount);
    }

    [Fact]
    public void TestThatSubscriberFeedsSink()
    {
        CreatePublisherWith(SourceOfABC);
        CreateSubscriberWith(_sink, 2);

        var access = _sink.AfterCompleting(5);

        Publisher.Subscribe(Subscriber);

        var subscriberCount = access.ReadFrom<int>("ready");
        Assert.Equal(1, subscriberCount);

        var valueCount = _sink.AccessValueMustBe("value", 3);
        Assert.Equal(3, valueCount);

        var terminateCount = _sink.AccessValueMustBe("terminate", 1);
        Assert.Equal(1, terminateCount);

        var values = access.ReadFrom<List<string>>("values");
        Assert.Equal(3, values.Count);
        Assert.Equal("A", values[0]);
        Assert.Equal("B", values[1]);
        Assert.Equal("C", values[2]);
    }

    [Fact]
    public void TestThatSubscriberReceivesTotalRandomNumberOfElements()
    {
        CreatePublisherWith(SourceRandomNumberOfElements);
        CreateSubscriberWith(_sink, 5);

        var access = _sink.AfterCompleting(102);

        Publisher.Subscribe(Subscriber);

        var subscriberCount = access.ReadFrom<int>("ready");
        Assert.Equal(1, subscriberCount);

        var valueCount = _sink.AccessValueMustBe("value", 100);
        Assert.Equal(100, valueCount);

        var terminateCount = _sink.AccessValueMustBe("terminate", 1);
        Assert.Equal(1, terminateCount);

        var values = access.ReadFrom<List<string>>("values");
        var expected = StringListOf1To(100);
        Assert.Equal(expected, values);
    }

    [Fact]
    public void TestThatSubscriberReceiversUpToCancel()
    {
        CreatePublisherWith(SourceRandomNumberOfElements);
        var subscriber = new TestSubscriber<string>(_sink, 100, 50);

        var access = _sink.AfterCompleting(50);

        Publisher.Subscribe(subscriber);

        var subscriberCount = access.ReadFrom<int>("ready");
        Assert.Equal(1, subscriberCount);

        var valueCount = _sink.AccessValueMustBe("value", 50);
        Assert.True(50 <= valueCount);

        var terminateCount = _sink.AccessValueMustBe("terminate", 1);
        Assert.Equal(1, terminateCount);

        var values = access.ReadFrom<List<string>>("values");
        var expected = StringListOf1To(valueCount);
        Assert.Equal(expected, values);
    }
}