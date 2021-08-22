// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Reactive.Streams;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams
{
    /// <summary>
    /// The standard <see cref="ISubscriber{T}"/> of streams.
    /// </summary>
    /// <typeparam name="T">The type od value consumed</typeparam>
    public class StreamSubscriber<T> : Actor, ISubscriber<T>
    {
        private readonly ISubscriber<T> _subscriber;

        public StreamSubscriber(Sink<T> sink, long requestThreshold) => 
            _subscriber = new StreamSubscriberDelegate<T>(sink, requestThreshold, Logger);

        public virtual void OnComplete() => _subscriber.OnComplete();

        public virtual void OnError(Exception cause) => _subscriber.OnError(cause);

        public virtual void OnNext(T element) => _subscriber.OnNext(element);

        public virtual void OnSubscribe(ISubscription? subscription) => _subscriber.OnSubscribe(subscription);
    }
}