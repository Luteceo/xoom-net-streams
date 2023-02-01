// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Streams.Operator;

/// <summary>
/// Filters <see cref="Sink{T}"/> values and potentially produces <see cref="Source{T}"/> values.
/// </summary>
/// <typeparam name="T">The input and output type</typeparam>
public class Filter<T> : Operator<T, T>
{
    private readonly Predicate<T> _predicate;

    public Filter(Predicate<T> predicate) => _predicate = predicate;

    public override void PerformInto(T value, Action<T> consumer)
    {
        try
        {
            if (_predicate.Invoke(value))
            {
                consumer.Invoke(value);
            }
        }
        catch (Exception e)
        {
            Streams.Logger.Error($"Filter failed because: {e.Message}", e);
        }
    }
}