namespace System;

/// <summary>
/// Represents the builder that can create <see cref="ReadOnlyTrunks{T}"/> instances.
/// </summary>
/// <seealso cref="ReadOnlyTrunks{T}"/>
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
public static class ReadOnlyTrunksBuilder
{
	/// <summary>
	/// Creates a <see cref="ReadOnlyTrunks{T}"/> instance.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="values">The values.</param>
	/// <returns>A <see cref="ReadOnlyTrunks{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlyTrunks<T> Create<T>(scoped ReadOnlySpan<T> values) => new([.. values]);
}
