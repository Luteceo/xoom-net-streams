// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams;

/// <summary>
/// Transform a value of type <typeparamref name="T"/> to a value of type <typeparamref name="TR"/>.
/// </summary>
/// <typeparam name="T">The type of the original value</typeparam>
/// <typeparam name="TR">The type of the resulting value</typeparam>
public interface ITransformer<in T, TR>
{
    /// <summary>
    /// Answer the <see cref="ICompletes"/> value after transforming it from the <paramref name="value"/> of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value">The T typed original value</param>
    /// <returns><see cref="ICompletes"/></returns>
    ICompletes<TR> Transform(T value);
}