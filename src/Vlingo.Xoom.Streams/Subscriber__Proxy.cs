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
    public class Subscriber__Proxy<T> : ISubscriber<T>
    {
        private const string OnSubscribeRepresentation1 = "OnSubscribe(ISubscription)";
        private const string OnCompleteRepresentation2 = "OnComplete()";
        private const string OnErrorRepresentation3 = "OnError(Exception)";
        private const string OnNextRepresentation4 = "OnNext(T)";
        private readonly Actor _actor;
        private readonly IMailbox _mailbox;

        public Subscriber__Proxy(Actor actor, IMailbox mailbox)
        {
            _actor = actor;
            _mailbox = mailbox;
        }

        public void OnSubscribe(ISubscription subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException("Subscription must not be null");
            if (_actor.IsStopped)
            {
                _actor.DeadLetters.FailedDelivery(new DeadLetter(_actor, OnSubscribeRepresentation1));
            }
            else
            {
                Action<ISubscriber<T>> consumer = (actor) => actor.OnSubscribe(subscription);
                if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, OnSubscribeRepresentation1);
                else _mailbox.Send(new LocalMessage<ISubscriber<T>>(_actor, consumer, OnSubscribeRepresentation1));
            }
        }

        public void OnNext(T element)
        {
            if (element == null)
                throw new ArgumentNullException("Element must not be null");
            if (_actor.IsStopped)
            {
                _actor.DeadLetters?.FailedDelivery(new DeadLetter(_actor, OnNextRepresentation4));
            }
            else
            {
                Action<ISubscriber<T>> consumer = (actor) => actor.OnNext(element);
                if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, OnNextRepresentation4);
                else _mailbox.Send(new LocalMessage<ISubscriber<T>>(_actor, consumer, OnNextRepresentation4));
            }
        }

        public void OnError(Exception cause)
        {
            if (cause == null)
                throw new ArgumentNullException("Exception must not be null");
            if (_actor.IsStopped)
            {
                _actor.DeadLetters?.FailedDelivery(new DeadLetter(_actor, OnErrorRepresentation3));
            }
            else
            {
                Action<ISubscriber<T>> consumer = (actor) => actor.OnError(cause);
                if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, OnErrorRepresentation3);
                else _mailbox.Send(new LocalMessage<ISubscriber<T>>(_actor, consumer, OnErrorRepresentation3));
            }
        }

        public void OnComplete()
        {
            if (_actor.IsStopped)
            {
                _actor.DeadLetters?.FailedDelivery(new DeadLetter(_actor, OnCompleteRepresentation2));
            }
            else
            {
                Action<ISubscriber<T>> consumer = (actor) => actor.OnComplete();
                if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, OnCompleteRepresentation2);
                else _mailbox.Send(new LocalMessage<ISubscriber<T>>(_actor, consumer, OnCompleteRepresentation2));
            }
        }
    }
}