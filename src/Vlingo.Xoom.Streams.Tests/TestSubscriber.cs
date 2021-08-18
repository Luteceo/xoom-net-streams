// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Reactive.Streams;
using Vlingo.Xoom.Actors.TestKit;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Tests
{
    public class TestSubscriber<T> : ISubscriber<T>
    {
        private AccessSafely _access;

        private readonly AtomicInteger _onSubscribeCount = new AtomicInteger(0);
        private readonly AtomicInteger _onNextCount = new AtomicInteger(0);
        private readonly AtomicInteger _onErrorCount = new AtomicInteger(0);
        private readonly AtomicInteger _onCompleteCount = new AtomicInteger(0);

        private readonly BlockingCollection<T> _values = new BlockingCollection<T>();

        private readonly Sink<T> _sink;

        private bool _cancelled;
        private readonly int _cancelAfterElements;
        private readonly int _total;
        private ISubscription _subscription;

        public TestSubscriber(int total) : this(total, total * total)
        {
        }

        public TestSubscriber(int total, int cancelAfterElements) : this(null, total, cancelAfterElements)
        {
        }

        public TestSubscriber(Sink<T> sink, int total, int cancelAfterElements)
        {
            _sink = sink;
            _total = total;
            _cancelAfterElements = cancelAfterElements;

            _access = AfterCompleting(0);
        }

        public void OnSubscribe(ISubscription subscription)
        {
            if (_subscription != null)
            {
                subscription.Cancel();
                return;
            }

            _subscription = subscription;
            _access.WriteUsing("onSubscribe", 1);
            subscription.Request(_total);

            _sink?.Ready();
        }

        public void OnNext(T value)
        {
            _access.WriteUsing("onNext", 1);
            _access.WriteUsing("values", value);

            if (_onNextCount.Get() >= _cancelAfterElements && !_cancelled)
            {
                _subscription?.Cancel();
                _sink?.Terminate();
                _cancelled = true;
            }

            _sink?.WhenValue(value);
        }

        public void OnError(Exception cause)
        {
            _access.WriteUsing("onError", 1);
            _sink?.Terminate();
        }

        public void OnComplete()
        {
            _access.WriteUsing("onComplete", 1);
            _sink?.Terminate();
        }

        public AccessSafely AfterCompleting(int times)
        {
            _access = AccessSafely.AfterCompleting(times);

            _access.WritingWith("onSubscribe", (int value) => _onSubscribeCount.AddAndGet(value));
            _access.WritingWith("onNext", (int value) => _onNextCount.AddAndGet(value));
            _access.WritingWith("onError", (int value) => _onErrorCount.AddAndGet(value));
            _access.WritingWith("onComplete", (int value) => _onCompleteCount.AddAndGet(value));

            _access.WritingWith("values", (T value) => _values.Add(value));

            _access.ReadingWith("onSubscribe", () => _onSubscribeCount.Get());
            _access.ReadingWith("onNext", () => _onNextCount.Get());
            _access.ReadingWith("onError", () => _onErrorCount.Get());
            _access.ReadingWith("onComplete", () => _onCompleteCount.Get());

            _access.ReadingWith("values", () => _values.ToList());

            return _access;
        }

        public int AccessValueMustBe(string name, int expected)
        {
            var current = 0;
            for (var tries = 0; tries < 10; ++tries)
            {
                var value = _access.ReadFrom<int>(name);
                if (value >= expected) return value;
                if (!current.Equals(value)) current = value;

                Thread.Sleep(100);
            }

            return expected == 0 ? -1 : 0;
        }
    }
}