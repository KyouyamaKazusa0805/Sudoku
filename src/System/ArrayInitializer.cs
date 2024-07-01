namespace System;

/// <summary>
/// Represents a method that initializes for an element of type <typeparamref name="T"/>?,
/// and be <typeparamref name="T"/> after the method invoked.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="value">The value to be initialized.</param>
public delegate void ArrayInitializer<T>([NotNull] ref T? value) where T : allows ref struct;
