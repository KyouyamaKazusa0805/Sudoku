namespace System;

/// <summary>
/// Internal builder type for creating <see cref="ChunkNode{T}"/>.
/// </summary>
/// <seealso cref="ChunkNode{T}"/>
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
public static class ChunkNodeBuilder
{
	/// <summary>
	/// The create method.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="values">The values.</param>
	/// <returns>A <see cref="ChunkNode{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ChunkNode<T> Create<T>(scoped ReadOnlySpan<T> values) => values.ToArray();
}
