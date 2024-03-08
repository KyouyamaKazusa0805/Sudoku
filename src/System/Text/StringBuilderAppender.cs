namespace System.Text;

/// <summary>
/// Represents an appender method, adding the specified instance into the <see cref="StringBuilder"/> instance.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
/// <param name="this">The string builder instance.</param>
/// <param name="value">The value to be added.</param>
/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
public delegate StringBuilder StringBuilderAppender<T>(StringBuilder @this, T value) where T : notnull;
