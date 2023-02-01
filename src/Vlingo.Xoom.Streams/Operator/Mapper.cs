// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Streams.Operator;

/// <summary>
/// Maps from <see cref="Sink{T}"/> to <see cref="Source{T}"/>
/// </summary>
/// <typeparam name="T">The input parameter type</typeparam>
/// <typeparam name="TR">The result type</typeparam>
public class Mapper<T, TR> : Operator<T, TR>
{
    private readonly Func<T, TR> _mapper;

    public Mapper(Func<T, TR> mapper) => _mapper = mapper;

    public override void PerformInto(T value, Action<TR> consumer)
    {
        try
        {
            consumer.Invoke(_mapper.Invoke(value));
        }
        catch (Exception e)
        {
            Streams.Logger.Error($"Mapper failed because: {e.Message}", e);
        }
    }
}