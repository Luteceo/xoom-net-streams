// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using static Vlingo.Xoom.Streams.Streams;

namespace Vlingo.Xoom.Streams
{
    public class PublisherConfiguration
    {
        public const int DefaultProbeInterval = 15;
        public const int FastProbeInterval = 2;
        public const int FastestProbeInterval = 1;

        public int BufferSize { get; }
        public int MaxThrottle { get; }
        public OverflowPolicy OverflowPolicy { get; }
        public int ProbeInterval { get; }

        public static PublisherConfiguration DefaultDropHead =>
            new PublisherConfiguration(DefaultProbeInterval, DefaultMaxThrottle, DefaultBufferSize,
                OverflowPolicy.DropHead);
        
        public static PublisherConfiguration DefaultDropTail =>
            new PublisherConfiguration(DefaultProbeInterval, DefaultMaxThrottle, DefaultBufferSize,
                OverflowPolicy.DropTail);

        public static PublisherConfiguration DefaultDropCurrent =>
            new PublisherConfiguration(DefaultProbeInterval, DefaultMaxThrottle, DefaultBufferSize,
                OverflowPolicy.DropCurrent);
        
        public static PublisherConfiguration With(int probeInterval, int maxThrottle, int bufferSize, OverflowPolicy overflowPolicy) => 
            new PublisherConfiguration(probeInterval, maxThrottle, bufferSize, overflowPolicy);

        public PublisherConfiguration(int probeInterval, int maxThrottle, int bufferSize, OverflowPolicy overflowPolicy)
        {
            if (probeInterval < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(probeInterval), "Probe interval should be positive");
            }
            
            if (maxThrottle < 0 && maxThrottle != DefaultMaxThrottle)
            {
                throw new ArgumentOutOfRangeException(nameof(maxThrottle), "Max throttle should be positive");
            }
            
            if (bufferSize < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bufferSize), "Buffer size should be positive");
            }
            
            ProbeInterval = probeInterval;
            MaxThrottle = maxThrottle;
            BufferSize = bufferSize;
            OverflowPolicy = overflowPolicy;
        }

        public PublisherConfiguration(int probeInterval, OverflowPolicy overflowPolicy) : this(probeInterval,
            DefaultMaxThrottle, DefaultBufferSize, overflowPolicy)
        {
        }
    }
}