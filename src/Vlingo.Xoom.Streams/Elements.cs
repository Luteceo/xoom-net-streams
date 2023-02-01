// Copyright © 2012-2023 VLINGO LABS. All rights reserved.
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL
// was not distributed with this file, You can obtain
// one at https://mozilla.org/MPL/2.0/.

using System;
using System.Linq;

namespace Vlingo.Xoom.Streams;

/// <summary>
/// A container for the element(s) that may be available on each
/// <code>Source{T}.Next()</code> or <code>Source{T}.Next(int index)</code>.
/// </summary>
/// <typeparam name="T">The type of element value</typeparam>
public class Elements<T>
{
    private static readonly object[] _empty = Array.Empty<object>();

    /// <summary>
    /// Zero or more element values.
    /// </summary>
    public T[] Values { get; }
        
    /// <summary>
    /// Bool <code>true</code> when the <see cref="Source{T}"/> has no more elements otherwise <code>false</code>
    /// </summary>
    public bool IsTerminated { get; }

    /// <summary>
    /// Answer a new <see cref="Elements{T}"/> with no <code>values</code> but that is not <code>terminated</code>.
    /// </summary>
    /// <returns><see cref="Elements{T}"/></returns>
    public static Elements<T> Empty() => new Elements<T>(_empty.Cast<T>().ToArray(), false);
        
    /// <summary>
    /// Answer a new <see cref="Elements{T}"/> with the single <code>values</code> of <code>elements</code>.
    /// </summary>
    /// <param name="value">Type type of the <see cref="Elements{T}"/></param>
    /// <returns><see cref="Elements{T}"/></returns>
    public static Elements<T> Of(T value) => new Elements<T>(new [] { value }, false);
        
    /// <summary>
    /// Answer a new <see cref="Elements{T}"/> with <code>values</code> of <code>elements</code>.
    /// </summary>
    /// <param name="values">The T typed value instances of the new <see cref="Elements{T}"/></param>
    /// <returns><see cref="Elements{T}"/></returns>
    public static Elements<T> Of(params T[] values) => new Elements<T>(values, false);

    /// <summary>
    /// Answer a new <see cref="Elements{T}"/> with no <code>values</code> and that is <code>terminated</code>.
    /// </summary>
    /// <returns><see cref="Elements{T}"/></returns>
    public static Elements<T> Terminated() => new Elements<T>(_empty.Cast<T>().ToArray(), true);

    /// <summary>
    /// Constructs my state.
    /// </summary>
    /// <param name="values">The T[] of zero or more element values</param>
    /// <param name="terminated">The bool indicating whether or not the Source is terminated</param>
    public Elements(T[] values, bool terminated)
    {
        Values = values;
        IsTerminated = terminated;
    }

    /// <summary>
    /// Answer the <code>T</code> value at the <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The int index of the value to answer</param>
    /// <returns>T</returns>
    public T ElementAt(int index) => (Values.Length == 0 ? default : Values[index])!;
        
    /// <summary>
    /// Answer whether or not there is an element value at <paramref name="index"/>.
    /// </summary>
    /// <param name="index">The int index of the value to check for existence</param>
    /// <returns><code>true</code> if the element exists at index otherwise false.</returns>
    public bool HasElementAt(int index) => Values.Length != 0 && index < Values.Length;
        
    public string ElementsAsString() => string.Join(",", Values);
        
    public override string ToString() => $"Elements[values={ElementsAsString()} terminated={IsTerminated}]";
}