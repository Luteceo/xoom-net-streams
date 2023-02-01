// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Linq;
using Reactive.Streams;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams.Tests;

public abstract class StreamPubSubTest : IDisposable
{
    protected readonly PublisherConfiguration Configuration;
    protected IControlledSubscription<string> ControlledSubscription;
    protected readonly ISource<string> SourceOf123;
    protected readonly ISource<string> SourceOfABC;
    protected ISource<string> SourceRandomNumberOfElements;
    protected IPublisher<string> Publisher;
    protected ISubscriber<string> Subscriber;
    protected readonly World World;

    public StreamPubSubTest()
    {
        World = World.StartWithDefaults("streams");

        Configuration = new PublisherConfiguration(5, Streams.OverflowPolicy.DropHead);

        SourceOf123 = Source<string>.Only(new[] { "1", "2", "3" });

        SourceOfABC = Source<string>.Only(new[] { "A", "B", "C" });

        SourceRandomNumberOfElements = new RandomNumberOfElementsSource(100);
    }

    protected void CreatePublisherWith(ISource<string> source)
    {
        var definition = Definition.Has<StreamPublisher<string>>(Definition.Parameters(source, Configuration));

        var protocols = World.ActorFor(new[] { typeof(IPublisher<string>), typeof(IControlledSubscription<string>) },
            definition);

        Publisher = protocols.Get<IPublisher<string>>(0);

        ControlledSubscription = protocols.Get<IControlledSubscription<string>>(1);
    }

    protected void CreateSubscriberWith(Sink<string> sink, long requestThreshold)
    {
        var protocols = World.ActorFor(new[] { typeof(ISubscriber<string>) }, typeof(StreamSubscriber<string>), sink,
            requestThreshold);

        Subscriber = protocols.Get<ISubscriber<string>>(0);
        Publisher.Subscribe(Subscriber);
    }

    protected ISubscriber<T> CreateSubscriberWithoutSubscribing<T>(Sink<T> sink, long requestThreshold)
    {
        var subscriber = World.ActorFor<ISubscriber<T>>(() => new StreamSubscriber<T>(sink, requestThreshold));
        return subscriber;
    }

    protected IEnumerable<string> StringListOf1To(int n) =>
        Enumerable.Range(1, n)
            .Select(idx => $"{idx}");

    protected IEnumerable<int> IntegerListOf1To(int n) => Enumerable.Range(1, n);

    public void Dispose() => World.Terminate();
}