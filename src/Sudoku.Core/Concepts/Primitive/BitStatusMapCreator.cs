namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents a type that has ability to create <see cref="IBitStatusMap{TSelf, TElement}"/> instances called by compiler.
/// For the users' aspect, we can just use collection expressions.
/// </summary>
/// <seealso cref="IBitStatusMap{TSelf, TElement}"/>
[EditorBrowsable(EditorBrowsableState.Never)]
[DebuggerStepThrough]
public static class BitStatusMapCreator
{
	/// <summary>
	/// Creates a <see cref="IBitStatusMap{TSelf, TElement}"/> instance via the specified offsets.
	/// </summary>
	/// <typeparam name="TSelf"><inheritdoc cref="IBitStatusMap{TSelf, TElement}" path="/typeparam[@name='TSelf']"/></typeparam>
	/// <typeparam name="TElement"><inheritdoc cref="IBitStatusMap{TSelf, TElement}" path="/typeparam[@name='TElement']"/></typeparam>
	/// <param name="offsets">The offsets.</param>
	/// <returns>A <see cref="IBitStatusMap{TSelf, TElement}"/> instance.</returns>
	public static TSelf Create<TSelf, TElement>(scoped ReadOnlySpan<TElement> offsets)
		where TSelf : unmanaged, IBitStatusMap<TSelf, TElement>
		where TElement : unmanaged, IBinaryInteger<TElement>
	{
		var result = TSelf.Empty;
		foreach (var offset in offsets)
		{
			result.Add(offset);
		}
		return result;
	}
}
