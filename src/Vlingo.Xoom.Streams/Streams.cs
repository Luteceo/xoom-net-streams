// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Reactive.Streams;
using Vlingo.Xoom.Actors;

namespace Vlingo.Xoom.Streams
{
    public static class Streams
    {
        private static ILogger? _logger;

        /// <summary>
        /// The default total number of elements to buffer
        /// internally before applying the <see cref="OverflowPolicy"/>.
        /// </summary>
        public const int DefaultBufferSize = 256;
        
        /// <summary>
        /// The default maximum number of elements to deliver
        /// to the <see cref="Sink{T}"/> before internally buffering.
        /// The request maximum will be honored if the throttle
        /// is higher than the remaining number requested. The
        /// default of <code>-1</code> indicates to deliver up to the
        /// remaining number of elements, up to the request maximum.
        /// </summary>
        public const int DefaultMaxThrottle = -1;

        /// <summary>
        /// Declares what the <see cref="StreamPublisher{T}"/> should do in the case
        /// of the internal buffer reaching overflow of elements.
        /// </summary>
        public enum OverflowPolicy
        {
            /// <summary>
            /// Drops the head (first) element to make room for appending current as the tail.
            /// </summary>
            DropHead,
            
            /// <summary>
            /// Drops the tail (last) element to make room for appending current as the tail.
            /// </summary>
            DropTail,
            
            /// <summary>
            /// Drops the current element in favor of delivering the total number of previously buffered elements.
            /// </summary>
            DropCurrent
        }

        /// <summary>
        /// Sets a <see cref="ILogger"/> for use by streams.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the logger is already set.</exception>
        /// <exception cref="ArgumentNullException">Thrown if trying to get when the logger is null.</exception>
        public static ILogger Logger
        {
            set
            {
                if (_logger != null)
                {
                    throw new ArgumentException("Logger is already set.");
                }

                _logger = value;
            }
            get => _logger ?? throw new ArgumentNullException($"Logger is null.");
        }
        
        /// <summary>
        /// Answer a new <see cref="IPublisher{T}"/> created within <see cref="Stage"/> using <paramref name="source"/> from
        /// which to read elements. The default <code>PublisherConfiguration.DefaultDropHead</code> is used.
        /// </summary>
        /// <param name="stage">The <see cref="Stage"/> within with the <see cref="IPublisher{T}"/> actor is created</param>
        /// <param name="source">The <see cref="Source{T}"/> used by the <see cref="IPublisher{T}"/> to read elements</param>
        /// <typeparam name="T">The T typed elements to be published</typeparam>
        /// <returns><see cref="IPublisher{T}"/></returns>
        public static IPublisher<T> PublisherWith<T>(Stage stage, ISource<T> source) => 
            PublisherWith(stage, source, PublisherConfiguration.DefaultDropHead);

        /// <summary>
        /// Answer a new <see cref="IPublisher{T}"/> created within <see cref="Stage"/>, reading from <paramref name="source"/>,
        /// and configured by the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="stage">The <see cref="Stage"/> within with the <see cref="IPublisher{T}"/> actor is created</param>
        /// <param name="source">The <see cref="Source{T}"/> used by the <see cref="IPublisher{T}"/> to read elements</param>
        /// <param name="configuration">The PublisherConfiguration used to configure the <see cref="IPublisher{T}"/></param>
        /// <typeparam name="T">The T typed elements to be published</typeparam>
        /// <returns><see cref="IPublisher{T}"/></returns>
        public static IPublisher<T> PublisherWith<T>(Stage stage, ISource<T> source, PublisherConfiguration configuration) => 
            stage.ActorFor<IPublisher<T>>(() => new StreamPublisher<T>(source, configuration));

        /// <summary>
        /// Answer a new <see cref="ISubscriber{T}"/> created within <paramref name="stage"/> and pushing to <paramref name="sink"/>,
        /// with an unbounded request threshold.
        /// </summary>
        /// <param name="stage">The Stage within with the <see cref="ISubscriber{T}"/> actor is created</param>
        /// <param name="sink">The <see cref="Sink{T}"/> to which elements are pushed</param>
        /// <typeparam name="T">The T typed elements of the subscription</typeparam>
        /// <returns><see cref="ISubscriber{T}"/></returns>
        public static ISubscriber<T> SubscriberWith<T>(Stage stage, Sink<T> sink) => 
            SubscriberWith(stage, sink, long.MaxValue);

        /// <summary>
        /// Answer a new <see cref="ISubscriber{T}"/> created within <paramref name="stage"/> and pushing to <paramref name="sink"/>,
        /// with the <paramref name="requestThreshold"/>. If the <paramref name="requestThreshold"/> is less than the
        /// unbounded <code>long.MaxValue</code>, the same amount is requested each time <paramref name="requestThreshold"/>
        /// elements are streamed to the <see cref="ISubscriber{T}"/>.
        /// </summary>
        /// <param name="stage">The Stage within with the <see cref="ISubscriber{T}"/> actor is created</param>
        /// <param name="sink">The <see cref="Sink{T}"/> to which elements are pushed</param>
        /// <param name="requestThreshold">The long request limit to be used until the subscription is completed</param>
        /// <typeparam name="T">The T typed elements of the subscription</typeparam>
        /// <returns><see cref="ISubscriber{T}"/></returns>
        public static ISubscriber<T> SubscriberWith<T>(Stage stage, Sink<T> sink, long requestThreshold) => 
            stage.ActorFor<ISubscriber<T>>(() => new StreamSubscriber<T>(sink, requestThreshold));
    }
}