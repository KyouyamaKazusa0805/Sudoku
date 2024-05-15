namespace System;

/// <summary>
/// Represents a method that will return itself. The method can also handle some other logic inside method block.
/// </summary>
/// <typeparam name="T">The type of an instance to be returned.</typeparam>
/// <param name="value">The value to be returned.</param>
/// <returns>The instance to be returned.</returns>
public delegate ref readonly T SelfReturner<T>(ref readonly T value);
