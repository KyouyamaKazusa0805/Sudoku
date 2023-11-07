namespace System;

/// <summary>
/// Indicates a callback method that is used by for-each methods, iterating on each reference instead of instance.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
/// <param name="arg">The reference to the element.</param>
public delegate TResult FuncRef<T, out TResult>(scoped ref T arg);
