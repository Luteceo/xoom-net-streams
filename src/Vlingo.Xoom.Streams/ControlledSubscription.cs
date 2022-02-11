// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

namespace Vlingo.Xoom.Streams;

public interface IControlledSubscription<T>
{
    public void Cancel(SubscriptionController<T> subscription);
    public void Request(SubscriptionController<T> subscription, long maximum);
}