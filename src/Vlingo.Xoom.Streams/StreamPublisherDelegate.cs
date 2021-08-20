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

namespace Vlingo.Xoom.Streams
{
    public class StreamPublisherDelegate<T> : IPublisher<T>, IControlledSubscription<T>
    {
        private readonly ISource<T> _source;
        private readonly PublisherConfiguration _configuration;
        private readonly IControlledSubscription<T> _controlledSubscription;
        private readonly Scheduler _scheduler;
        private readonly IScheduled<object> _scheduled;
        private readonly IStoppable _stoppable;

        private readonly Dictionary<int, SubscriptionController<T>> _subscriptions;

        private bool _slow;
        private ICancellable? _cancellable;

        public StreamPublisherDelegate(ISource<T> source, PublisherConfiguration configuration,
            IControlledSubscription<T> controlledSubscription, Scheduler scheduler,
            IScheduled<object> scheduled, IStoppable stoppable)
        {
            _source = source;
            _configuration = configuration;
            _controlledSubscription = controlledSubscription;
            _scheduler = scheduler;
            _scheduled = scheduled;
            _stoppable = stoppable;

            _subscriptions = new Dictionary<int, SubscriptionController<T>>(2);

            _cancellable = null;

            DetermineIfSlow();
        }

        //===================================
        // Publisher
        //===================================
        
        public void Subscribe(ISubscriber<T> subscriber)
        {
            //Console.WriteLine($"3: {GetType()} : {nameof(Subscribe)}");
            Schedule(true);

            var controller = new SubscriptionController<T>(subscriber, _controlledSubscription, _configuration);

            if (!_subscriptions.ContainsKey(controller.Id))
            {
                _subscriptions.Add(controller.Id, controller);
            }

            subscriber.OnSubscribe(controller);
        }
        
        //===================================
        // ControlledSubscription
        //===================================

        public void Cancel(SubscriptionController<T> controller)
        {
            controller.CancelSubscription();
            _subscriptions.Remove(controller.Id);
        }

        public void Request(SubscriptionController<T> controller, long maximum)
        {
            //Console.WriteLine($"StreamPublisherDelegate.{nameof(Request)} | SubscriptionController: {controller}");
            controller.RequestFlow(controller.Accumulate(maximum));

            Publish(controller, Optional.Empty<T>());
        }

        //===================================
        // Internal implementation
        //===================================
        
        internal void ProcessNext()
        {
            if (!_subscriptions.Any())
            {
                return;
            }

            try
            {
                _source
                    .Next()
                    .AndThenConsume(maybeElements =>
                    {
                        if (!maybeElements.IsTerminated)
                        {
                            //Console.WriteLine($"StreamPublisherDelegate PROCESS NEXT: ELEMENTS: {maybeElements}");
                            Publish(maybeElements.Values);
                            Schedule(false);
                        }
                        else if (Flush())
                        {
                            // nothing to do
                        }
                        else
                        {
                            //Console.WriteLine("StreamPublisherDelegate COMPLETING ALL");
                            CompleteAll();
                            _stoppable.Stop();
                        }
                    });
            }
            catch (Exception e)
            {
                Publish(e);
            }
        }

        public void Publish(Exception cause)
        {
            foreach (var controller in _subscriptions.Values)
            {
                controller.OnError(cause);
            }
        }

        private T[] Publish(T[] maybeElements)
        {
            //Console.WriteLine($"StreamPublisherDelegate T[] {nameof(Publish)} | maybeElements: {string.Join(", ", maybeElements)}");
            if (maybeElements.Any())
            {
                for (var idx = 0; idx < maybeElements.Length; ++idx)
                {
                    Publish(maybeElements[idx]);
                }
            }

            return maybeElements;
        }

        private void Publish(T elementOrNull)
        {
            foreach (var controller in _subscriptions.Values)
            {
                controller.OnNext(Optional.Of(elementOrNull));
            }
        }

        private void Publish(SubscriptionController<T> controller, Optional<T> elementOrNull)
        {
            //Console.WriteLine($"StreamPublisherDelegate.{nameof(Publish)} | SubscriptionController: {controller}");
            controller.OnNext(elementOrNull);
        }

        private bool _flushed;

        private bool Flush()
        {
            //Console.WriteLine($"StreamPublisherDelegate.{nameof(Flush)}");
            _flushed = false;
            foreach (var controller in _subscriptions.Values.Where(controller => controller.HasBufferedElements()))
            {
                controller.OnNext(Optional.Empty<T>());
                _flushed = true;
            }

            return _flushed;
        }

        public void Stop()
        {
            _cancellable?.Cancel();
            CompleteAll();
        }

        private void CompleteAll()
        {
            //Console.WriteLine($"StreamPublisherDelegate.{nameof(CompleteAll)}");
            foreach (var controller in _subscriptions.Values)
            {
                controller.Subscriber.OnComplete();
            }

            _subscriptions.Clear();
        }

        // pure evil; don't try this at home.
        // BTW, it is most likely not a slow
        // operation to determine whether the
        // Source is slow, like intsy tinsy
        // blocking.
        private void DetermineIfSlow() => _slow = _source.IsSlow().Await();

        private void Schedule(bool isSubscribing)
        {
            if (_slow)
            {
                _cancellable = _scheduler.ScheduleOnce(_scheduled, default!, TimeSpan.Zero,
                    TimeSpan.FromMilliseconds(_configuration.ProbeInterval));
            }
            else
            {
                if (isSubscribing && _cancellable == null)
                    _cancellable = _scheduler.Schedule(_scheduled, default!, TimeSpan.Zero,
                        TimeSpan.FromMilliseconds(_configuration.ProbeInterval));
            }
        }
    }
}