// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Xoom.Actors;

namespace Reactive.Streams
{
    public class Publisher__Proxy<T> : IPublisher<T>
    {
        private const string SubscribeRepresentation1 = "Subscribe(ISubscriber<T>)";
        private readonly Actor _actor;
        private readonly IMailbox _mailbox;

        public Publisher__Proxy(Actor actor, IMailbox mailbox)
        {
            _actor = actor;
            _mailbox = mailbox;
        }

        public void Subscribe(ISubscriber<T> subscriber)
        {
            if (subscriber == null)
                throw new ArgumentNullException("Subscriber must not be null");
            if (_actor.IsStopped)
            {
                _actor.DeadLetters?.FailedDelivery(new DeadLetter(_actor, SubscribeRepresentation1));
            }
            else
            {
                Action<IPublisher<T>> consumer = (actor) => actor.Subscribe(subscriber);
                if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, SubscribeRepresentation1);
                else _mailbox.Send(new LocalMessage<IPublisher<T>>(_actor, consumer, SubscribeRepresentation1));
            }
        }
    }
}