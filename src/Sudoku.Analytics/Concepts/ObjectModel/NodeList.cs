namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Defines a list of <see cref="ChainNode"/> using doubly linked list as the backing algorithm.
/// </summary>
/// <seealso cref="ChainNode"/>
public sealed class NodeList : LinkedList<ChainNode>
{
	/// <summary>
	/// Creates a <see cref="NodeList"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodeList() : base()
	{
	}

	/// <summary>
	/// Creates a <see cref="NodeList"/> instance via the specified <see cref="ChainNode"/> instances.
	/// </summary>
	/// <param name="potentials">The base <see cref="ChainNode"/> values.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public NodeList(IEnumerable<ChainNode> potentials) : base(potentials)
	{
	}


	/// <summary>
	/// <inheritdoc cref="LinkedList{T}.RemoveFirst" path="/summary"/>
	/// </summary>
	/// <returns>The value of the first node removed.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new ChainNode RemoveFirst()
	{
		var first = First!.Value;
		base.RemoveFirst();

		return first;
	}
}
