// Copyright © 2012-2022 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;

namespace Vlingo.Xoom.Streams.Sink;

public class TerminalOperationConsumerSink<T, TO> : ConsumerSink<T>
{
    private readonly Action<TO> _terminalOperation;
    private readonly TO _terminalValue;
        
    public TerminalOperationConsumerSink(Action<T> consumer, TO terminalValue, Action<TO> terminalOperation) : base(consumer)
    {
        _terminalOperation = terminalOperation;
        _terminalValue = terminalValue;
    }

    public override void Terminate()
    {
        base.Terminate();
        _terminalOperation(_terminalValue);
    }
}