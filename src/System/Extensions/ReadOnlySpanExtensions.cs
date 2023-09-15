namespace System;

/// <summary>
/// Provides with extension methods on <see cref="Span{T}"/> and <see cref="ReadOnlySpan{T}"/>.
/// </summary>
/// <seealso cref="Span{T}"/>
/// <seealso cref="ReadOnlySpan{T}"/>
public static class ReadOnlySpanExtensions
{
	/// <summary>
	/// Performs the specified action on each element of the <see cref="Span{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the span.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <param name="action">The <see cref="ActionRef{T}"/> delegate to perform on each element of the <see cref="Span{T}"/>.</param>
	public static void ForEach<T>(this scoped Span<T> @this, ActionRef<T> action)
	{
		foreach (ref var element in @this)
		{
			action(ref element);
		}
	}

	/// <summary>
	/// Performs the specified action on each element of the <see cref="ReadOnlySpan{T}"/>.
	/// </summary>
	/// <typeparam name="T">The type of elements in the span.</typeparam>
	/// <param name="this">The current collection.</param>
	/// <param name="action">
	/// The <see cref="ActionRefReadOnly{T}"/> delegate to perform on each element of the <see cref="ReadOnlySpan{T}"/>.
	/// </param>
	public static void ForEach<T>(this scoped ReadOnlySpan<T> @this, ActionRefReadOnly<T> action)
	{
		foreach (ref readonly var element in @this)
		{
			action(in element);
		}
	}
}
