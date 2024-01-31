namespace System;

/// <summary>
/// Represents the builder that can create <see cref="ReadOnlyChunk{T}"/> instances.
/// </summary>
/// <seealso cref="ReadOnlyChunk{T}"/>
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
public static class ReadOnlyChunksBuilder
{
	/// <summary>
	/// Creates a <see cref="ReadOnlyChunk{T}"/> instance.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <param name="values">The values.</param>
	/// <returns>A <see cref="ReadOnlyChunk{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ReadOnlyChunk<T> Create<T>(scoped ReadOnlySpan<T> values) => new([.. values]);
}
