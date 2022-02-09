// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams
{
    /// <summary>
    /// The upstream source of a <see cref="Stream"/> that provides <see cref="Elements{T}"/> of
    /// next available value(s), or indicates termination of such values.
    ///
    /// WARNING:
    ///
    /// A <see cref="ISource{T}"/> is polled asynchronously by a <see cref="StreamPublisher{T}"/>.
    /// The <see cref="StreamPublisher{T}"/> uses a <see cref="Vlingo.Xoom.Common.Scheduler"/>
    /// to determine the time interval between polling, and could assume the
    /// rapid answer of the next element(s) or empty. Consider possible flaws
    /// in such a design:
    ///
    /// (1) Latency in <code>Next()</code> and <code>Next(int index)</code> may cause
    /// publish conflicts given that the subsequent <code>IntervalSignal()</code> is
    /// delivered and its <code>Next()</code> or <code>Next(int index)</code> completes
    /// before the previous publish completes. This would no doubt cause races
    /// in any managed subscription instances.
    ///
    /// (2) Latency in <code>Next()</code> and <code>Next(int index)</code> may also cause
    /// eventual <see cref="OutOfMemoryException"/> due to <code>IntervalSignal()</code>
    /// messages growing the <see cref="StreamPublisher{T}"/>'s mailbox faster than
    /// it can process them.
    ///
    /// In order to avoid this situation <code>IsSlow()</code> is provided. If
    /// <code>IsSlow()</code> answers <code>true</code> the <see cref="StreamPublisher{T}"/>
    /// will reschedule its given time interval following the completion
    /// of each <code>Next()</code>/<code>Next(int index)</code> and publish pair.
    /// Although doubtful, this could tax the scheduler while preventing
    /// the above two potential flaws.
    ///
    /// If <code>IsSlow()</code> answers <code>false</code> the <see cref="StreamPublisher{T}"/>
    /// will schedule a recurring time interval, which is done only once.
    /// </summary>
    /// <typeparam name="T">The type produced by the <see cref="ISource{T}"/></typeparam>
    public interface ISource<T>
    {
        /// <summary>
        /// Answer a new empty <see cref="ISource{T}"/>.
        /// </summary>
        /// <returns><see cref="ISource{T}"/></returns>
        public ISource<T> Empty();

        /// <summary>
        /// Answer a new <see cref="ISource{T}"/> with the static number of <paramref name="elements"/>.
        /// </summary>
        /// <param name="elements">The T typed element(s) provided by the new <see cref="ISource{T}"/></param>
        /// <returns><see cref="ISource{T}"/></returns>
        public ISource<T> Only(IEnumerable<T> elements);

        /// <summary>
        /// Answer a new <see cref="ISource{T}"/> with element(s) to be provided between
        /// <paramref name="startInclusive"/> and <paramref name="endExclusive"/>. The <see cref="ISource{T}"/> is non-slow.
        /// </summary>
        /// <param name="startInclusive">The long start of the range, inclusive</param>
        /// <param name="endExclusive">The long end of the range, exclusive</param>
        /// <returns><see cref="ISource{T}"/></returns>
        public ISource<long> RangeOf(long startInclusive, long endExclusive);

        /// <summary>
        /// Answer the number of <paramref name="elements"/>, or if <paramref name="elements"/> is out of bounds,
        /// answer <code>long.MaxValue</code>.
        /// </summary>
        /// <param name="elements">The long number of elements, which may be in bounds or out of bounds</param>
        /// <returns><paramref name="elements"/> or <code>long.MaxValue</code></returns>
        public long OrElseMaximum(long elements);

        /// <summary>
        /// Answer the number of <paramref name="elements"/>, or if <paramref name="elements"/> is out of bounds,
        /// answer <code>0</code>.
        /// </summary>
        /// <param name="elements">The long number of elements, which may be in bounds or out of bounds</param>
        /// <returns><paramref name="elements"/> or <code>long.MaxValue</code></returns>
        public long OrElseMinimum(long elements);

        /// <summary>
        /// Answer a new <see cref="ISource{T}"/> with element(s) to be provided
        /// by the <see cref="IEnumerable{T}"/>. The <see cref="ISource{T}"/> is created
        /// as non-slow.
        /// </summary>
        /// <param name="iterable">The <see cref="IEnumerable{T}"/> providing element(s) for the new <see cref="ISource{T}"/></param>
        /// <returns><see cref="ISource{T}"/></returns>
        public ISource<T> With(IEnumerable<T> iterable);

        /// <summary>
        /// Answer a new <see cref="ISource{T}"/> with element(s) to be provided
        /// by the <see cref="IEnumerable{T}"/>, which may or may not be a <paramref name="slowIterable"/>.
        /// </summary>
        /// <param name="iterable">The <see cref="IEnumerable{T}"/> providing element(s) for the new <see cref="ISource{T}"/></param>
        /// <param name="slowIterable">The boolean indicating whether or not the iterable is slow</param>
        /// <returns><see cref="ISource{T}"/></returns>
        public ISource<T> With(IEnumerable<T> iterable, bool slowIterable);
        
        /// <summary>
        /// Answer a new <see cref="ISource{T}"/> with elements to be provided by the <see cref="Func{TResult}"/>.
        /// The <see cref="ISource{T}"/> is created created as non-slow.
        /// </summary>
        /// <param name="supplier">The <see cref="Func{TResult}"/> providing element(s) for the new <see cref="ISource{T}"/></param>
        /// <returns><see cref="ISource{T}"/></returns>
        public ISource<T> With(Func<T> supplier);
        
        /// <summary>
        /// Answer a new <see cref="ISource{T}"/> with elements to be provided by the <see cref="Func{TResult}"/>,
        /// which may or may not be a <paramref name="slowSupplier"/>.
        /// </summary>
        /// <param name="supplier">The <see cref="Func{TResult}"/> providing element(s) for the new <see cref="ISource{T}"/></param>
        /// <param name="slowSupplier">The boolean indicating whether or not the supplier is slow</param>
        /// <returns><see cref="ISource{T}"/></returns>
        public ISource<T> With(Func<T> supplier, bool slowSupplier);

        /// <summary>
        /// Answers the next element(s) as a <see cref="Elements{T}"/>, which has a
        /// zero length <code>values</code> when the next element is not <strong>immediately</strong> available.
        /// Answering the zero length <see cref="Elements{T}"/>.Value is to prevent blocking.
        /// </summary>
        /// <returns><see cref="ICompletes{T}"/></returns>
        public ICompletes<Elements<T>> Next();
        
        /// <summary>
        /// Answers the next <paramref name="maximumElements"/> as a <see cref="ICompletes{T}"/>, which has a
        /// zero length <code>values</code> when the next element is not <strong>immediately</strong> available.
        /// Answering the zero length <see cref="ICompletes{T}"/>.Value is to prevent blocking.
        /// </summary>
        /// <param name="maximumElements">The int maximum number of elements to answer</param>
        /// <returns><see cref="ICompletes{T}"/></returns>
        public ICompletes<Elements<T>> Next(int maximumElements);
        
        /// <summary>
        /// Answers the next element(s) starting at <paramref name="index"/> as a <see cref="ICompletes{T}"/>,
        /// which has a zero length <code>values</code> when at least the indexed element is not
        /// <strong>immediately</strong> available. Answering the zero length <see cref="ICompletes{T}"/>.Value
        /// is to prevent blocking.
        ///
        /// It is recommended to use this method only when the elements are actually
        /// identified by indexes, such as with an ordered collection or log.
        /// </summary>
        /// <param name="index">The long index of the element at which to start</param>
        /// <returns><see cref="ICompletes{T}"/></returns>
        public ICompletes<Elements<T>> Next(long index);
        
        /// <summary>
        /// Answers the next <paramref name="maximumElements"/> starting at <paramref name="index"/> as a <see cref="ICompletes{T}"/>,
        /// which has a zero length <code>values</code> when at least the indexed element is not
        /// <strong>immediately</strong> available. Answering the zero length <see cref="ICompletes{T}"/>.Value
        /// is to prevent blocking.
        ///
        /// It is recommended to use this method only when the elements are actually
        /// identified by indexes, such as with an ordered collection or log.
        /// </summary>
        /// <param name="index">The long index of the element at which to start</param>
        /// <param name="maximumElements">The int maximum number of elements to answer</param>
        /// <returns><see cref="ICompletes{T}"/></returns>
        public ICompletes<Elements<T>> Next(long index, int maximumElements);
        
        /// <summary>
        /// Answers whether or not the concrete <see cref="ISource{T}"/> is subject to
        /// constantly latency or intermittent latency. See the warning in the
        /// above interface documentation.
        /// </summary>
        /// <returns><see cref="ICompletes{T}"/></returns>
        public ICompletes<bool> IsSlow();
    }
}