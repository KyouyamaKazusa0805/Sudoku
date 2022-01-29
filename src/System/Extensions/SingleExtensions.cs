using static System.Math;

namespace System;

/// <summary>
/// Provides extension methods on <see cref="float"/>.
/// </summary>
/// <seealso cref="float"/>
public static class SingleExtensions
{
	/// <summary>
	/// Indicates whether the specified value is nearly equals to the current value.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="other">The other value.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool NearlyEquals(this float @this, float other) => @this.NearlyEquals(other, float.Epsilon);

	/// <summary>
	/// Indicates whether the specified value is nearly equals to the current value.
	/// If the differ of two values to compare is lower than the specified epsilon value,
	/// the method will return <see langword="true"/>.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="other">The other value to compare.</param>
	/// <param name="epsilon">The epsilon value (the minimal differ).</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool NearlyEquals(this float @this, float other, float epsilon) =>
		Abs(@this - other) < epsilon;
}
