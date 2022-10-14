#pragma warning disable IDE1006

namespace System.Text;

/// <summary>
/// Defines an appender method that converts a <typeparamref name="TUnmanaged"/> instance into a <see cref="string"/> result.
/// </summary>
/// <typeparam name="TUnmanaged">The type of each element in a collection, specified as a read-only reference.</typeparam>
/// <param name="refFirstElement">A reference that reference to the collection.</param>
/// <returns>The converted string result.</returns>
public delegate string? StringHandlerRefAppender<TUnmanaged>(scoped in TUnmanaged refFirstElement) where TUnmanaged : unmanaged;
