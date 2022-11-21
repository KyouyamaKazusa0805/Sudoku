namespace System;

/// <summary>
/// Provides with extension methods on <see cref="int"/>.
/// </summary>
/// <seealso cref="int"/>
public static class Int32Extensions
{
	/// <summary>
	/// Gets the equivalent seconds of type <see cref="TimeSpan"/> from current integer value.
	/// </summary>
	/// <param name="this">The integer value.</param>
	/// <returns>The equivalent <see cref="TimeSpan"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TimeSpan Seconds(this int @this) => TimeSpan.FromSeconds(@this);
}
