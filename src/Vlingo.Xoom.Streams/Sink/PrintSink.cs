// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;

namespace Vlingo.Xoom.Streams.Sink;

/// <summary>
/// A <see cref="Sink{T}"/> that prints to the given <see cref="StreamWriter"/>, and may prefix
/// the output with a <code>string</code> value.
/// </summary>
/// <typeparam name="T">The type of the value to be printed</typeparam>
public class PrintSink<T> : Sink<T>, IDisposable
{
    private readonly TextWriter _printStream;
    private readonly string _prefix;
    private bool _terminated;
        
    /// <summary>
    /// Constructs my default state.
    /// </summary>
    /// <param name="printStream">The <see cref="TextWriter"/> through which to print my values</param>
    /// <param name="prefix">The string used to begin each printed line</param>
    public PrintSink(TextWriter printStream, string prefix)
    {
        _printStream = printStream;
        _prefix = prefix;
        _terminated = false;
    }
        
    public override void Ready()
    {
        // ignored
    }

    public override void Terminate() => _terminated = true;

    public override void WhenValue(T value)
    {
        if (!_terminated)
        {
            _printStream.WriteLine(_prefix + value);
        }
    }

    public void Dispose() => _printStream.Dispose();

    public override string ToString() => $"PrintSink[terminated={_terminated}]";
}