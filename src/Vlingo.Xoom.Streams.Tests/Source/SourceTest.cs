// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System.Text;

namespace Vlingo.Xoom.Streams.Tests.Source
{
    public abstract class SourceTest
    {
        private readonly StringBuilder _builder = new StringBuilder();

        protected string StringFromSource(ISource<string> source)
        {
            var current = "";
            while (current != null)
            {
                var elements = source
                    .Next()
                    .Await();

                current = elements.ElementAt(0);

                if (current != null)
                    _builder.Append(current);
            }

            var result = _builder.ToString();
            return result;
        }
    }
}