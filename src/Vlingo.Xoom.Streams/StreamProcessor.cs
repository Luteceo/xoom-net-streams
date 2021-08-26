// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
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
using Vlingo.Xoom.Common;
using Vlingo.Xoom.Streams.Sink;

namespace Vlingo.Xoom.Streams
{
    /// <summary>
    /// A <see cref="IProcessor{T1,T2}"/> implementation, where as a <see cref="ISubscriber{T}"/> I consume from an
    /// upstream <see cref="IPublisher{T}"/>, perform an operation on those signals using <see cref="Operator{T,TR}"/>,
    /// and emit new signals via my own <see cref="IPublisher{T}"/>.
    ///
    /// My instances reuse <see cref="StreamSubscriberDelegate{T}"/> and <see cref="StreamPublisherDelegate{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type that the Subscriber side consumes</typeparam>
    /// <typeparam name="TR">The type that the Publisher side emits</typeparam>
    public sealed class StreamProcessor<T, TR> : Actor, IProcessor<T, TR>, IControlledSubscription<TR>, IScheduled<object>
    {
        private readonly StreamPublisherDelegate<TR> _publisherDelegate;
        private readonly PublisherSource _publisherSource;
        private readonly long _requestThreshold;
        private readonly StreamSubscriberDelegate<T> _subscriberDelegate;

        /// <summary>
        /// Construct my default state with <paramref name="operator"/>, <paramref name="requestThreshold"/>, and <paramref name="configuration"/>.
        /// </summary>
        /// <param name="operator">The <see cref="Operator{T,TR}"/> that performs on an instance of <typeparamref name="T"/> to yield an instance of <typeparamref name="TR"/></param>
        /// <param name="requestThreshold">The long number of signals accepted by my subscription</param>
        /// <param name="configuration">The <see cref="PublisherConfiguration"/> used by my publisher</param>
        public StreamProcessor(Operator<T, TR> @operator, long requestThreshold, PublisherConfiguration configuration)
        {
            _requestThreshold = requestThreshold;
            _subscriberDelegate = new StreamSubscriberDelegate<T>(new ConsumerSink<T>(ConsumerOperator(@operator)), requestThreshold, Logger);
            _publisherSource = new PublisherSource(this);
            _publisherDelegate = new StreamPublisherDelegate<TR>(_publisherSource, configuration,
                SelfAs<IControlledSubscription<TR>>(), Scheduler, SelfAs<IScheduled<object>>(),
                SelfAs<IStoppable>());
        }

        //===================================
        // Subscriber
        //===================================
        
        public void OnSubscribe(ISubscription subscription) => _subscriberDelegate.OnSubscribe(subscription);

        public void OnNext(T value) => _subscriberDelegate.OnNext(value);

        public void OnError(Exception cause)
        {
            _publisherDelegate.Publish(cause);
            _subscriberDelegate.OnError(cause);

            _publisherSource.Terminate();
        }

        public void OnComplete()
        {
            _subscriberDelegate.OnComplete();
            _publisherSource.Terminate();
        }

        //===================================
        // Publisher
        //===================================
        
        public void Subscribe(ISubscriber<TR> subscriber) => _publisherDelegate.Subscribe(subscriber);

        //===================================
        // ControlledSubscription
        //===================================
        
        public void Cancel(SubscriptionController<TR> controller)
        {
            _subscriberDelegate.CancelSubscription();
            _publisherDelegate.Cancel(controller);
        }

        public void Request(SubscriptionController<TR> subscription, long maximum) => _publisherDelegate.Request(subscription, maximum);

        //===================================
        // Scheduled
        //===================================
        
        public void IntervalSignal(IScheduled<object> scheduled, object data) => _publisherDelegate.ProcessNext();

        //===================================
        // Stoppable
        //===================================
        
        public override void Stop()
        {
            _publisherSource.Terminate();
            base.Stop();
        }

        //===================================
        // PublisherSource
        //===================================
        
        private class PublisherSource : DefaultSource<TR>
        {
            private readonly StreamProcessor<T, TR> _streamProcessor;
            private bool _terminated;
            private readonly Queue<TR> _values;

            public PublisherSource(StreamProcessor<T, TR> streamProcessor)
            {
                _streamProcessor = streamProcessor;
                _values = new Queue<TR>();
                _terminated = false;
            }

            public override ICompletes<Elements<TR>> Next() => Next((int)_streamProcessor._requestThreshold);

            public override ICompletes<Elements<TR>> Next(int maximumElements)
            {
                if (!_values.Any())
                {
                    if (_streamProcessor._subscriberDelegate.IsFinalized() || _terminated)
                    {
                        return Common.Completes.WithSuccess(Elements<TR>.Terminated());
                    }

                    return Common.Completes.WithSuccess(Elements<TR>.Empty());
                }

                return Common.Completes.WithSuccess(Elements<TR>.Of(NextValue(maximumElements)));
            }

            private TR[] NextValue(int maximum)
            {
                var elements = Math.Min(_values.Count, maximum);
                var nextValues = new TR[elements];
                for (var idx = 0; idx < nextValues.Length; ++idx)
                {
                    nextValues[idx] = _values.Dequeue();
                }

                return nextValues.ToArray();
            }

            public override ICompletes<Elements<TR>> Next(long index) => Next((int)_streamProcessor._requestThreshold);

            public override ICompletes<Elements<TR>> Next(long index, int maximumElements) => Next(maximumElements);

            public override ICompletes<bool> IsSlow() => Common.Completes.WithSuccess(false);

            public void Enqueue(TR value) => _values.Enqueue(value);

            public void Terminate() => _terminated = true;
        }

        //===================================
        // ConsumerTransformer
        //===================================
        
        private Action<T> ConsumerOperator(Operator<T, TR> @operator)
        {
            return value =>
            {
                try
                {
                    @operator.PerformInto(value, transformed => _publisherSource.Enqueue(transformed));
                }
                catch (Exception e)
                {
                    _publisherDelegate.Publish(e);
                }
            };
        }
    }
}