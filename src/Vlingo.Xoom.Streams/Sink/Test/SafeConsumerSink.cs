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
using Vlingo.Xoom.Actors.TestKit;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Sink.Test
{
    public class SafeConsumerSink<T> : Sink<T>
    {
        private AccessSafely _access;
        private readonly AtomicInteger _readyCount = new AtomicInteger(0);
        private readonly AtomicInteger _terminateCount = new AtomicInteger(0);
        private readonly AtomicInteger _valueCount = new AtomicInteger(0);

        private readonly BlockingCollection<T> _values = new BlockingCollection<T>();

        public SafeConsumerSink() => _access = AfterCompleting(0);

        public AccessSafely AfterCompleting(int times)
        {
            _access = AccessSafely.AfterCompleting(times);

            _access.WritingWith("ready", (int value) => _readyCount.AddAndGet(value));
            _access.WritingWith("terminate", (int value) => _terminateCount.AddAndGet(value));
            _access.WritingWith("value", (int value) => _valueCount.AddAndGet(value));

            _access.WritingWith("values", (T value) => _values.Add(value));

            _access.ReadingWith("ready", () => _readyCount.Get());
            _access.ReadingWith("terminate", () => _terminateCount.Get());
            _access.ReadingWith("value", () => _valueCount.Get());

            _access.ReadingWith("values", () => _values.ToList());

            return _access;
        }

        public int AccessValueMustBe(string name, int expected)
        {
            //Console.WriteLine($"{GetType()}: {nameof(AccessValueMustBe)}");
            var current = 0;
            for (var tries = 0; tries < 10; ++tries)
            {
                var value = _access.ReadFromExpecting<int>(name, expected, 1000, false);
                if (value >= expected) return value;
                if (!current.Equals(value)) current = value;
                Thread.Sleep(100);
            }

            return expected == 0 ? -1 : current;
        }

        public override void Ready()
        {
            //Console.WriteLine($"{GetType()}: {nameof(Ready)}");

            _access.WriteUsing("ready", 1);
        }

        public override void Terminate()
        {
            //Console.WriteLine($"{GetType()}: {nameof(Terminate)}");

            _access.WriteUsing("terminate", 1);
        }

        public override void WhenValue(T value)
        {
            //Console.WriteLine($"{GetType()}: {nameof(WhenValue)}");

            _access.WriteUsing("value", 1);
            _access.WriteUsing("values", value);
        }

        public override string ToString() => "SafeConsumerSink";
    }
}