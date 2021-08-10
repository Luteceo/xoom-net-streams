// Copyright © 2012-2021 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Operator
{
    public abstract class QueueSource<T> : Source<T>
    {
        private readonly Queue<T> _queue;
        private bool _slow;
        private bool _terminated;

        public QueueSource(bool slow)
        {
            _slow = slow;
            _queue = new Queue<T>(1);
            _terminated = false;
        }

        public override ICompletes<Elements<T>> Next() => Next(0, 1);

        public override ICompletes<Elements<T>> Next(int maximumElements) => Next(0, maximumElements);

        public override ICompletes<Elements<T>> Next(long index) => Next(0, 1);

        public override ICompletes<Elements<T>> Next(long index, int maximumElements)
        {
            if (_queue.Count > 0)
            {
                var total = Math.Min(_queue.Count, maximumElements);
                var elements = new T[total];
                for (var idx = 0; idx < total; ++idx)
                {
                    elements[idx] = _queue.Dequeue();
                }
                
                return Completes.WithSuccess(Elements<T>.Of(elements));
            }
            
            return Completes.WithSuccess(_terminated ? Elements<T>.Terminated() : Elements<T>.Empty());
        }

        public override ICompletes<bool> IsSlow() => Completes.WithSuccess(_slow);
        
        protected void Add(T value) => _queue.Enqueue(value);

        protected void Terminated() => _terminated = true;

        protected bool IsTerminated => _terminated;
    }
}