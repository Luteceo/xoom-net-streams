// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using Vlingo.Xoom.Actors;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams;

public class StreamTransformer<T, TR> : Actor, ITransformer<T, TR>
{
    private readonly ITransformer<T, TR> _transformer;

    public StreamTransformer(ITransformer<T, TR> transformer) => _transformer = transformer;

    public ICompletes<TR> Transform(T value) => _transformer.Transform(value);
}