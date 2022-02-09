// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
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
    public class StreamSubscriberDelegate<T> : ISubscriber<T>
    {
        private readonly Sink<T> _sink;
        private readonly long _requestThreshold;
        private readonly ILogger _logger;

        private long _count;
        private bool _completed;
        private bool _errored;
        private ISubscription? _subscription;

        public StreamSubscriberDelegate(Sink<T> sink, long requestThreshold, ILogger logger)
        {
            _sink = sink;
            _requestThreshold = requestThreshold;
            _logger = logger;
            _count = 0;
            _completed = false;
            _errored = false;
            _subscription = null;
        }

        public void OnSubscribe(ISubscription subscription)
        {
            if (_subscription != null)
            {
                subscription.Cancel();
            }
            else
            {
                _subscription = subscription;
                _sink.Ready();
                _subscription.Request(_requestThreshold);
            }
        }

        public void OnComplete()
        {
            _completed = true;
            Terminate();

            _logger.Info($"Subscriber with {_sink} is completed.");
        }

        public void OnError(Exception cause)
        {
            _errored = true;
            Terminate();

            _logger.Error($"Subscriber with {_sink} is terminating because: {cause.Message}, {cause}");
        }

        public void OnNext(T value)
        {
            if (!IsFinalized())
            {
                _sink.WhenValue(value);
            
                if (++_count >= _requestThreshold)
                {
                    _subscription?.Request(_requestThreshold);
                }
            }
        }

        public bool IsFinalized() => _completed || _errored;

        private void Terminate() => _sink.Terminate();

        public void CancelSubscription() => _subscription?.Cancel();
    }
}