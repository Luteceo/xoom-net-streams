// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

namespace Vlingo.Xoom.Streams
{
    public static class Stream
    {
        /// <summary>
        /// The default rate of flow bursts
        /// </summary>
        public const long DefaultFlowRate = 100;
        
        /// <summary>
        /// The default interval to poll the <see cref="Source{T}"/>.
        /// </summary>
        public const int DefaultProbeInterval = PublisherConfiguration.DefaultProbeInterval;
        
        /// <summary>
        /// The fast interval to poll the <see cref="Source{T}"/>.
        /// </summary>
        public const int FastProbeInterval = PublisherConfiguration.FastProbeInterval;
        
        /// <summary>
        /// The fastest interval to poll the <see cref="Source{T}"/>.
        /// </summary>
        public const int FastestProbeInterval = PublisherConfiguration.FastestProbeInterval;
    }
}