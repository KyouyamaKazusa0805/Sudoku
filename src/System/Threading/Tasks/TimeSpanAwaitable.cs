namespace System.Threading.Tasks;

/// <summary>
/// Provides with extension methods on <see cref="TimeSpan"/>.
/// </summary>
/// <seealso cref="TimeSpan"/>
public static class TimeSpanAwaitable
{
	/// <summary>
	/// Creates a <see cref="TimeSpanAwaiter"/> instance used for <see langword="await"/> expressions.
	/// </summary>
	/// <param name="this">The time span.</param>
	/// <returns>A <see cref="TimeSpanAwaiter"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TimeSpanAwaiter GetAwaiter(this TimeSpan @this) => new(Task.Delay(@this).GetAwaiter());
}
