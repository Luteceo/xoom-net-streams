// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Streams.Operator
{
    public class FlatMapper<T, TR> : Operator<T, TR>
    {
        private readonly Func<T, Source<TR>> _mapper;
        private const int MaximumBuffer = 32;

        public FlatMapper(Func<T, Source<TR>> mapper) => _mapper = mapper;

        public override void PerformInto(T value, Action<TR> consumer)
        {
            try
            {
                var result = _mapper.Invoke(value);
                PropagateSource(result, consumer);
            }
            catch (Exception e)
            {
                Streams.Logger.Error($"FlatMapper failed because: {e.Message}", e);
            }
        }

        private static void PropagateSource(Source<TR> source, Action<TR> consumer)
        {
            source
                .Next(MaximumBuffer)
                .AndThenConsume(elements =>
                {
                    foreach (var element in elements.Values)
                    {
                        consumer.Invoke(element);
                    }

                    if (!elements.IsTerminated)
                    {
                        PropagateSource(source, consumer);
                    }
                });
        }
    }
}