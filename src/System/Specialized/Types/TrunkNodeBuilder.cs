namespace System;

/// <summary>
/// Internal builder type for creating <see cref="TrunkNode{T}"/>.
/// </summary>
/// <seealso cref="TrunkNode{T}"/>
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
public static class TrunkNodeBuilder
{
	/// <summary>
	/// The create method.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	/// <param name="values">The values.</param>
	/// <returns>A <see cref="TrunkNode{T}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static TrunkNode<T> Create<T>(scoped ReadOnlySpan<T> values) => values.ToArray();
}
