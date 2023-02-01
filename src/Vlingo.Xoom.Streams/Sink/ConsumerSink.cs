// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Streams.Sink;

/// <summary>
/// A <see cref="Sink{T}"/> that provides values to a given <see cref="Action{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the value to be provided to the <see cref="Action{T}"/>.</typeparam>
public class ConsumerSink<T> : Sink<T>
{
    private readonly Action<T> _consumer;
    private bool _terminated;

    public ConsumerSink(Action<T> consumer)
    {
        _consumer = consumer;
        _terminated = false;
    }

    public override void Ready()
    {
        // IGNORED
    }

    public override void Terminate() => _terminated = true;

    public override void WhenValue(T value)
    {
        if (!_terminated)
        {
            _consumer.Invoke(value);
        }
    }

    public override string ToString() => $"ConsumerSink[terminated={_terminated}]";
}