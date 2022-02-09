// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests
{
    public class StreamPublisherTest : StreamPubSubTest
    {
        [Fact]
        public void TestThatSubscriberSubscribes()
        {
            CreatePublisherWith(SourceOfABC);
            var subscriber = new TestSubscriber<string>(3);
            var access = subscriber.AfterCompleting(1);

            Publisher.Subscribe(subscriber);

            var subscriberCount = access.ReadFrom<int>("onSubscribe");
            Assert.Equal(1, subscriberCount);
        }

        [Fact]
        public void TestThatSubscriberReceives()
        {
            CreatePublisherWith(SourceOfABC);
            var subscriber = new TestSubscriber<string>(3);
            var access = subscriber.AfterCompleting(8);

            Publisher.Subscribe(subscriber);

            var subscriberCount = access.ReadFrom<int>("onSubscribe");
            Assert.Equal(1, subscriberCount);

            var completeCount = access.ReadFrom<int>("onComplete");
            Assert.Equal(1, completeCount);

            var valueCount = subscriber.AccessValueMustBe("onNext", 3);
            Assert.Equal(3, valueCount);

            var values = access.ReadFrom<List<string>>("values");
            var expected = new[] { "A", "B", "C" };
            Assert.Equal(expected, values);
        }

        [Fact]
        public void TestThatSubscriberReceivesTotalRandomNumberOfElements()
        {
            CreatePublisherWith(SourceRandomNumberOfElements);
            var subscriber = new TestSubscriber<string>(100);
            var access = subscriber.AfterCompleting(102);

            Publisher.Subscribe(subscriber);

            var subscriberCount = access.ReadFrom<int>("onSubscribe");
            Assert.Equal(1, subscriberCount);

            var completeCount = subscriber.AccessValueMustBe("onComplete", 1);
            Assert.Equal(1, completeCount);

            var valueCount = subscriber.AccessValueMustBe("onNext", 100);
            Assert.Equal(100, valueCount);

            var values = access.ReadFrom<List<string>>("values");
            var expected = StringListOf1To(100);
            Assert.Equal(expected, values);
        }

        [Fact]
        public void TestThatSubscriberReceivesUpToCancel()
        {
            CreatePublisherWith(SourceRandomNumberOfElements);
            var subscriber = new TestSubscriber<string>(100, 50);
            var access = subscriber.AfterCompleting(50);

            Publisher.Subscribe(subscriber);

            var subscriberCount = access.ReadFrom<int>("onSubscribe");
            Assert.Equal(1, subscriberCount);

            var valueCount = subscriber.AccessValueMustBe("onNext", 50);
            Assert.True(50 <= valueCount);

            var values = access.ReadFrom<List<string>>("values");
            var expected = StringListOf1To(valueCount);
            Assert.Equal(expected, values);
        }

        [Fact]
        public void TestThatSubscriberReceiversFromSlowSource()
        {
            SourceRandomNumberOfElements = new RandomNumberOfElementsSource(100, true);
            CreatePublisherWith(SourceRandomNumberOfElements);
            var subscriber = new TestSubscriber<string>(100);
            var access = subscriber.AfterCompleting(102);

            Publisher.Subscribe(subscriber);

            var subscriberCount = access.ReadFrom<int>("onSubscribe");
            Assert.Equal(1, subscriberCount);

            var completeCount = subscriber.AccessValueMustBe("onComplete", 1);
            Assert.Equal(1, completeCount);

            var valueCount = subscriber.AccessValueMustBe("onNext", 100);
            Assert.Equal(100, valueCount);

            var values = access.ReadFrom<List<string>>("values");
            var expected = StringListOf1To(100);
            Assert.Equal(expected, values);
        }
    }
}