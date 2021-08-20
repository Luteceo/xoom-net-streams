// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Xoom.Streams.Operator;

namespace Vlingo.Xoom.Streams
{
    /// <summary>
    /// An operator that takes <typeparamref name="T"/> input and produces <typeparamref name="TR"/> output
    /// into a given <see cref="Action{TR}"/>.
    /// </summary>
    /// <typeparam name="T">The input parameter type</typeparam>
    /// <typeparam name="TR">The result type</typeparam>
    public abstract class Operator<T, TR>
    {
        /// <summary>
        /// Answer a new <see cref="Operator{T,TR}"/> that filters using <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The <see cref="Predicate{T}"/> that filters</param>
        /// <returns><see cref="Operator{T,TR}"/></returns>
        public static Operator<T, T> FilterWith(Predicate<T> filter) => new Filter<T>(filter);

        /// <summary>
        /// Answer a new <see cref="Operator{T,TR}"/> that maps from values of <typeparamref name="T"/>
        /// to values of <typeparamref name="TR"/> by means of the <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">The <see cref="Func{T, TR}"/> that maps from values of <typeparamref name="T"/> to values of <typeparamref name="TR"/></param>
        /// <returns><see cref="Operator{T,TR}"/></returns>
        public static Operator<T, TR> MapWith(Func<T, TR> mapper) => new Mapper<T, TR>(mapper);

        /// <summary>
        /// Answer a new <see cref="Operator{T,TR}"/> that flat-maps from values of <typeparamref name="T"/>
        /// to values of <typeparamref name="TR"/> by means of the <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">The <code>Func{T, Source{TR}}</code> that maps from values of <typeparamref name="T"/> to values of <typeparamref name="TR"/></param>
        /// <returns><see cref="Operator{T,TR}"/></returns>
        public static Operator<T, TR> FlatMapWith(Func<T, ISource<TR>> mapper) => new FlatMapper<T, TR>(mapper);

        /// <summary>
        /// Accept the <typeparamref name="T"/> value and potentially give an
        /// <typeparamref name="TR"/> outcome to the <paramref name="consumer"/>.
        /// </summary>
        /// <param name="value">The T value to accept</param>
        /// <param name="consumer">The <see cref="Action{TR}"/> that may receive the outcome.</param>
        public abstract void PerformInto(T value, Action<TR> consumer);
    }
}