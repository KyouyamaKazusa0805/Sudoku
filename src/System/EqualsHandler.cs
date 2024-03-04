namespace System;

/// <summary>
/// The specialized equality comparing handler.
/// </summary>
/// <typeparam name="T">The type of the object.</typeparam>
/// <param name="left">The left value to be compared.</param>
/// <param name="right">The right value to be compared.</param>
/// <returns>A <see cref="bool"/> value indicating the result.</returns>
public delegate bool EqualsHandler<T>(scoped ref readonly T left, scoped ref readonly T right);
