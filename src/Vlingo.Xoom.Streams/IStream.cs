// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

namespace Vlingo.Xoom.Streams
{
    public interface IStream
    {
        /// <summary>
        /// Potentially changes the underlying <see cref="StreamSubscriber{T}"/>'s <paramref name="flowElementsRate"/>
        /// if its current is not equal to the <paramref name="flowElementsRate"/>.
        /// </summary>
        /// <param name="flowElementsRate">The long rate at which new elements will flow into the <see cref="Sink{T}"/></param>
        void Request(long flowElementsRate);
        
        /// <summary>
        /// Starts the flow from <see cref="StreamPublisher{T}"/> to <see cref="StreamSubscriber{T}"/>, which in turn flows the
        /// elements to the <paramref name="sink"/>. The <code>Stream.DefaultFlowRate</code> and <code>Stream.DefaultProbeInterval</code>
        /// are used.
        /// </summary>
        /// <param name="sink">The <see cref="Sink{T}"/> to which the <see cref="StreamSubscriber{T}"/> pushes elements</param>
        /// <typeparam name="T">The type of the elements that the Sink intakes</typeparam>
        void FlowInto<T>(Sink<T> sink);
        
        /// <summary>
        /// Starts the flow from <see cref="StreamPublisher{T}"/> to <see cref="StreamSubscriber{T}"/> at the rate of
        /// <paramref name="flowElementsRate"/>, which in turn flows the elements to the <paramref name="sink"/>.
        /// The <code>Stream.DefaultProbeInterval</code> is used.
        /// </summary>
        /// <param name="sink">The <see cref="Sink{T}"/> to which the <see cref="StreamSubscriber{T}"/> pushes elements</param>
        /// <param name="flowElementsRate">The long limit of elements to push to the Sink at any one time</param>
        /// <typeparam name="T">The type of the elements that the Sink intakes</typeparam>
        void FlowInto<T>(Sink<T> sink, long flowElementsRate);

        /// <summary>
        /// Starts the flow from <see cref="StreamPublisher{T}"/> to <see cref="StreamSubscriber{T}"/> at the rate of
        /// <paramref name="flowElementsRate"/>, which in turn flows the elements to the <paramref name="sink"/>.
        /// The <see cref="StreamPublisher{T}"/> will poll for new elements on every <paramref name="probeInterval"/>.
        /// </summary>
        /// <param name="sink">The <see cref="Sink{T}"/> to which the <see cref="StreamSubscriber{T}"/> pushes elements</param>
        /// <param name="flowElementsRate">The long limit of elements to push to the Sink at any one time</param>
        /// <param name="probeInterval">The int indicating how often the <see cref="StreamPublisher{T}"/> should poll its <see cref="Source{T}"/></param>
        /// <typeparam name="T">The type of the elements that the Sink intakes</typeparam>
        void FlowInto<T>(Sink<T> sink, long flowElementsRate, int probeInterval);
        
        /// <summary>
        /// Sends a message to the <see cref="StreamPublisher{T}"/> to stop polling elements from its
        /// <see cref="Source{T}"/> and pushing them to the <see cref="Sink{T}"/> by way of the <see cref="StreamSubscriber{T}"/>.
        /// Note that some elements may have already been pushed to the <see cref="StreamSubscriber{T}"/> but
        /// have not yet fully flowed to the <see cref="Sink{T}"/>. This means you cannot assume that
        /// flow will stop immediately.
        /// </summary>
        void Stop();
    }
}