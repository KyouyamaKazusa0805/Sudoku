namespace System;

/// <summary>
/// Represents a handler that checks the value comparison of two <typeparamref name="T"/> values.
/// </summary>
/// <typeparam name="T">The type of values to be compared.</typeparam>
/// <param name="left">The left value to be compared.</param>
/// <param name="right">The right value to be compared.</param>
/// <returns>An <see cref="int"/> value indicating the result.</returns>
public delegate int CompareHandler<T>(scoped ref readonly T left, scoped ref readonly T right);
