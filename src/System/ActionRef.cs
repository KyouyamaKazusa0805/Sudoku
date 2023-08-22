namespace System;

/// <summary>
/// Indicates a callback method that is used by for-each methods, iterating on each reference instead of instance.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="element">The reference to the element.</param>
public delegate void ActionRef<T>(scoped ref T element);
