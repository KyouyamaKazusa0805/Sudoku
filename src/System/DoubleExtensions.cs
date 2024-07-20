namespace System;

/// <summary>
/// Provides extension methods on <see cref="double"/>.
/// </summary>
/// <seealso cref="double"/>
public static class DoubleExtensions
{
	/// <summary>
	/// Indicates whether the specified value is nearly equals to the current value.
	/// </summary>
	/// <param name="this">The value.</param>
	/// <param name="other">The other value.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool NearlyEquals(this double @this, double other) => @this.NearlyEquals(other, double.Epsilon);

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
	public static bool NearlyEquals(this double @this, double other, double epsilon) => Math.Abs(@this - other) < epsilon;

	/// <summary>
	/// Creates a <see cref="TimeSpan"/> instane using the specified value as seconds.
	/// </summary>
	/// <param name="this">The seconds.</param>
	/// <returns>A <see cref="TimeSpan"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TimeSpan Seconds(this double @this) => TimeSpan.FromSeconds(@this);

	/// <summary>
	/// Creates a <see cref="TimeSpan"/> instane using the specified value as milliseconds.
	/// </summary>
	/// <param name="this">The milliseconds.</param>
	/// <returns>A <see cref="TimeSpan"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TimeSpan Milliseconds(this double @this) => TimeSpan.FromMicroseconds(@this);
}
