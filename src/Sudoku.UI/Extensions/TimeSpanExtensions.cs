namespace System;

/// <summary>
/// Provides extension methods on <see cref="TimeSpan"/>.
/// </summary>
/// <seealso cref="TimeSpan"/>
public static class TimeSpanExtensions
{
	/// <summary>
	/// Creates an awaiter that is used for the <see langword="await"/> statements
	/// via the <see cref="int"/> instance.
	/// </summary>
	/// <param name="this">The delay of milliseconds.</param>
	/// <returns>The awaiter instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TaskAwaiter GetAwaiter(this int @this) => Task.Delay(@this).GetAwaiter();

	/// <summary>
	/// Creates an awaiter that is used for the <see langword="await"/> statements
	/// via the <see cref="TimeSpan"/> instance.
	/// </summary>
	/// <param name="this">The delay of milliseconds.</param>
	/// <returns>The awaiter instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TaskAwaiter GetAwaiter(this TimeSpan @this) => Task.Delay((int)@this.Ticks).GetAwaiter();
}
