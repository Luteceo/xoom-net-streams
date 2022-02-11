// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;
using System.Text;
using Vlingo.Xoom.Streams.Sink;
using Xunit;

namespace Vlingo.Xoom.Streams.Tests.Sink;

public class PrintSinkTest : IDisposable
{
    private readonly MemoryStream _byteStream;
    private readonly StreamWriter _printStream;

    public PrintSinkTest()
    {
        _byteStream = new MemoryStream();
        _printStream = new StreamWriter(_byteStream);
    }

    [Fact]
    public void TestThatSinkPrintsToStream()
    {
        var sink = new PrintSink<string>(_printStream, "");

        sink.WhenValue("A");
        sink.WhenValue("B");
        sink.WhenValue("C");

        Assert.Equal($"A{Environment.NewLine}B{Environment.NewLine}C{Environment.NewLine}", PrintString());
    }

    [Fact]
    public void TestThatSinkPrintsWithPrefixToStream()
    {
        var sink = Sink<string>.PrintTo(_printStream, "-");

        sink.WhenValue("A");
        sink.WhenValue("B");
        sink.WhenValue("C");

        Assert.Equal($"-A{Environment.NewLine}-B{Environment.NewLine}-C{Environment.NewLine}", PrintString());
    }

    [Fact]
    public void TestThatTerminatedSinkDoesNotPrint()
    {
        var sink = new PrintSink<string>(_printStream, "");

        sink.WhenValue("A");
        sink.WhenValue("B");
        sink.WhenValue("C");

        sink.Terminate();

        sink.WhenValue("D");

        Assert.Equal($"A{Environment.NewLine}B{Environment.NewLine}C{Environment.NewLine}", PrintString());
    }

    public void Dispose() => _byteStream?.Dispose();

    private string PrintString()
    {
        _printStream.Flush();
        return Encoding.UTF8.GetString(_byteStream.ToArray());
    }
}