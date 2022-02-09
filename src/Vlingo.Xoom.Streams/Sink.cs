// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using Vlingo.Xoom.Streams.Sink;

namespace Vlingo.Xoom.Streams
{
    /// <summary>
    /// The downstream receiver of values from a <see cref="Source{T}"/> with possible transformation to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the streamed values</typeparam>
    public abstract class Sink<T>
    {
        /// <summary>
        /// Answer a new <see cref="Sink{T}"/> that relays to the <paramref name="consumer"/>.
        /// </summary>
        /// <param name="consumer">The <see cref="Action{T}"/> that will consume values</param>
        /// <returns><see cref="Sink{T}"/></returns>
        public static Sink<T> ConsumeWith(Action<T> consumer) => new ConsumerSink<T>(consumer);

        /// <summary>
        /// Answer a new <see cref="Sink{T}"/> that prints to the standard output
        /// stream where each line starts with <paramref name="prefix"/>.
        /// </summary>
        /// <param name="prefix">The string that begins each line printed</param>
        /// <returns><see cref="Sink{T}"/></returns>
        public static Sink<T> PrintToConsoleOut(string prefix) => PrintTo(Console.Out, prefix);
        
        /// <summary>
        /// Answer a new <see cref="Sink{T}"/> that prints to the standard error
        /// stream where each line starts with <paramref name="prefix"/>.
        /// </summary>
        /// <param name="prefix">The string that begins each line printed</param>
        /// <returns><see cref="Sink{T}"/></returns>
        public static Sink<T> PrintToConsoleError(string prefix) => PrintTo(Console.Error, prefix);

        /// <summary>
        /// Answer a new <see cref="Sink{T}"/> that prints to the given <paramref name="printStream"/>
        /// where each line starts with <paramref name="prefix"/>.
        /// </summary>
        /// <param name="printStream">The <see cref="TextWriter"/> where output values are printed</param>
        /// <param name="prefix">The string that begins each line printed</param>
        /// <returns><see cref="Sink{T}"/></returns>
        public static Sink<T> PrintTo(TextWriter printStream, string prefix) => new PrintSink<T>(printStream, prefix);

        /// <summary>
        /// Indicates that the receiver should become ready to receive values.
        /// </summary>
        public abstract void Ready();
        
        /// <summary>
        /// Indicates that the receiver has been terminated.
        /// </summary>
        public abstract void Terminate();
        
        /// <summary>
        /// Receives the new <paramref name="value"/> from the stream.
        /// </summary>
        /// <param name="value">The next T value from the stream</param>
        public abstract void WhenValue(T value);
    }
}