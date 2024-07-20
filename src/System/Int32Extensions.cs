namespace System;

/// <summary>
/// Provides with extension methods on <see cref="int"/>.
/// </summary>
/// <seealso cref="int"/>
public static class Int32Extensions
{
	/// <inheritdoc cref="DoubleExtensions.Seconds(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TimeSpan Seconds(this int @this) => ((double)@this).Seconds();

	/// <inheritdoc cref="DoubleExtensions.Milliseconds(double)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TimeSpan Milliseconds(this int @this) => ((double)@this).Milliseconds();
}
