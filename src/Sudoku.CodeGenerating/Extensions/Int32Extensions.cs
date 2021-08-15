namespace System;

/// <summary>
/// Provides extension methods on <see cref="int"/>.
/// </summary>
/// <seealso cref="int"/>
internal static class Int32Extensions
{
	/// <summary>
	/// Get the order suffix.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <returns>The order suffix.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string GetOrderSuffix(this int @this) => @this switch
	{
		1 when @this / 10 is 0 or 1 => "st",
		2 when @this / 10 is 0 or 1 => "nd",
		3 when @this / 10 is 0 or 1 => "rd",
		_ => "th"
	};
}
