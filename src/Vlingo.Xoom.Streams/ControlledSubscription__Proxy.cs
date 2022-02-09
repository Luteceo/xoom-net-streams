// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams
{
    public class ControlledSubscription__Proxy<T> : IControlledSubscription<T>
    {
        private const string RequestRepresentation1 = "Request(SubscriptionController<T>, long)";
        private const string CancelRepresentation2 = "Cancel(SubscriptionController<T>)";
        private readonly Actor _actor;
        private readonly IMailbox _mailbox;

        public ControlledSubscription__Proxy(Actor actor, IMailbox mailbox)
        {
            _actor = actor;
            _mailbox = mailbox;
        }

        public void Cancel(SubscriptionController<T> subscription)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription), "Subscription must not be null");
            if (_actor.IsStopped)
            {
                _actor.DeadLetters?.FailedDelivery(new DeadLetter(_actor, CancelRepresentation2));
            }
            else
            {
                Action<IControlledSubscription<T>> consumer = (actor) => actor.Cancel(subscription);
                if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, CancelRepresentation2);
                else
                    _mailbox.Send(
                        new LocalMessage<IControlledSubscription<T>>(_actor, consumer, CancelRepresentation2));
            }
        }

        public void Request(SubscriptionController<T> subscription, long maximum)
        {
            if (subscription == null)
                throw new ArgumentNullException(nameof(subscription), "Subscription must not be null");
            if (_actor.IsStopped)
            {
                _actor.DeadLetters?.FailedDelivery(new DeadLetter(_actor, RequestRepresentation1));
            }
            else
            {
                Action<IControlledSubscription<T>> consumer = (actor) => actor.Request(subscription, maximum);
                if (_mailbox.IsPreallocated) _mailbox.Send(_actor, consumer, null, RequestRepresentation1);
                else
                    _mailbox.Send(
                        new LocalMessage<IControlledSubscription<T>>(_actor, consumer, RequestRepresentation1));
            }
        }
    }
}