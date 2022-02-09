// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using Vlingo.Xoom.Common;

namespace Vlingo.Xoom.Streams.Source
{
    /// <summary>
    /// A Source that uses a <see cref="Func{TResult}"/> to provide next <see cref="Elements{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The T type of Element being supplied</typeparam>
    public class SupplierSource<T> : DefaultSource<T>
    {
        private readonly bool _slowSupplier;
        private readonly Func<T> _supplier;
        
        public SupplierSource(Func<T> supplier, bool slowSupplier) 
        {
            _supplier = supplier;
            _slowSupplier = slowSupplier;
        }
        
        public override ICompletes<Elements<T>> Next()
        {
            var any = _supplier();

            if (any != null)
            {
                var elements = new T[1];
                elements[0] = any;
                return Completes.WithSuccess(new Elements<T>(elements, false));
            }

            return Completes.WithSuccess(new Elements<T>(Array.Empty<T>(), true));
        }

        public override ICompletes<Elements<T>> Next(int maximumElements) => Next();

        public override ICompletes<Elements<T>> Next(long index) => Next();

        public override ICompletes<Elements<T>> Next(long index, int maximumElements) => Next();

        public override ICompletes<bool> IsSlow() => Completes.WithSuccess(_slowSupplier);
    }
}