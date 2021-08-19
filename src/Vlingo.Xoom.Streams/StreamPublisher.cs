// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using Reactive.Streams;
using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
    public sealed class StreamPublisher<T> : Actor, IPublisher<T>, IControlledSubscription<T>, IScheduled<object>
    {
        private readonly StreamPublisherDelegate<T> _delegate;

        public StreamPublisher(Source<T> source, PublisherConfiguration configuration) =>
            _delegate = new StreamPublisherDelegate<T>(source, configuration, SelfAs<IControlledSubscription<T>>(),
                Scheduler, SelfAs<IScheduled<object>>(), SelfAs<IStoppable>());
        
        //===================================
        // Scheduled
        //===================================
        
        public void IntervalSignal(IScheduled<object> scheduled, object data) => _delegate.ProcessNext();

        //===================================
        // ControlledSubscription
        //===================================
        
        public void Request(SubscriptionController<T> subscription, long maximum)
        {
            //Console.WriteLine($"{GetType()} : {nameof(Request)}");
            //Console.WriteLine($"StreamPublisher.Request | SubscriptionController: {subscription}, maximum: {maximum}");
            _delegate.Request(subscription, maximum);
        }
        
        public void Cancel(SubscriptionController<T> subscription) => _delegate.Cancel(subscription);

        //===================================
        // Publisher
        //===================================
        
        public void Subscribe(ISubscriber<T>? subscriber)
        {
            //Console.WriteLine($"{GetType()} : {nameof(Subscribe)}");
            if (subscriber != null)
            {
                _delegate.Subscribe(subscriber);
            }
        }

        //===================================
        // Internal implementation
        //===================================
        
        public override void Stop()
        {
            _delegate.Stop();

            base.Stop();
        }
    }
}