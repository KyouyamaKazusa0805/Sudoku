namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Lazy{T}"/>.
/// </summary>
/// <seealso cref="Lazy{T}"/>
public static class LazyExtensions
{
	/// <summary>
	/// Operate with inner value if <see cref="Lazy{T}"/> value is created.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="this">The lazy data structure.</param>
	/// <param name="action">The action to handle the value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void DoIfValueCreated<T>(this Lazy<T> @this, Action<T> action)
	{
		if (@this.IsValueCreated)
		{
			action(@this.Value);
		}
	}
}
