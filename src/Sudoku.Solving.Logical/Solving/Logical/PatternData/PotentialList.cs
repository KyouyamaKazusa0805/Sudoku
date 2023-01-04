namespace Sudoku.Solving.Logical.PatternData;

/// <summary>
/// Defines a list of <see cref="Potential"/> using doubly linked list as the backing algorithm.
/// </summary>
/// <seealso cref="Potential"/>
internal sealed class PotentialList : LinkedList<Potential>
{
	/// <summary>
	/// Creates a <see cref="PotentialList"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PotentialList() : base()
	{
	}

	/// <summary>
	/// Creates a <see cref="PotentialList"/> instance via the specified <see cref="Potential"/> instances.
	/// </summary>
	/// <param name="potentials">The base <see cref="Potential"/> values.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public PotentialList(IEnumerable<Potential> potentials) : base(potentials)
	{
	}


	/// <summary>
	/// <inheritdoc cref="LinkedList{T}.RemoveFirst" path="/summary"/>
	/// </summary>
	/// <returns>The value of the first node removed.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new Potential RemoveFirst()
	{
		var first = First!.Value;
		base.RemoveFirst();

		return first;
	}
}
