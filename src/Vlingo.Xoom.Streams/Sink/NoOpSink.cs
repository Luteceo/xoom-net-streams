// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

namespace Vlingo.Xoom.Streams.Sink;

/// <summary>
/// A <see cref="Sink{T}"/> that does nothing.
/// </summary>
/// <typeparam name="T">The type of the value to be provided</typeparam>
public class NoOpSink<T> : Sink<T>
{
    public NoOpSink()
    {
    }

    public override void Ready()
    {
        // ignored
    }

    public override void Terminate()
    {
        // ignored
    }

    public override void WhenValue(T value)
    {
        // ignored
    }

    public override string ToString() => "NoOpConsumerSink[nothing=nothing]";
}