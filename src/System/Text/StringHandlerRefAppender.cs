namespace System.Text;

/// <summary>
/// Defines an appender method that converts a <typeparamref name="T"/> instance into a <see cref="string"/> result.
/// </summary>
/// <typeparam name="T">The type of each element in a collection, specified as a read-only reference.</typeparam>
/// <param name="refFirstElement">A reference that reference to the collection.</param>
/// <returns>The converted string result.</returns>
public delegate string? StringHandlerRefAppender<T>(scoped in T refFirstElement);
