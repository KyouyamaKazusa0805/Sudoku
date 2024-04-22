namespace System;

/// <summary>
/// The specialized get hash code handler.
/// </summary>
/// <typeparam name="T">The type of the object.</typeparam>
/// <param name="obj">The object to be calculated.</param>
/// <returns>An <see cref="int"/> value indicating the result.</returns>
public delegate int GetHashCodeHandler<T>([DisallowNull] ref readonly T obj);
