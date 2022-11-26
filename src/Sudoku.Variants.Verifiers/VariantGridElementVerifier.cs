namespace Sudoku.Variants.Verifiers;

/// <summary>
/// Defines a verifier that can get all possbible variant elements in a solution grid.
/// </summary>
/// <typeparam name="TNode">The type of the node.</typeparam>
/// <param name="TargetGrid">Indicates the target grid used.</param>
/// <param name="Identifier">Indicates the identifier used.</param>
public abstract record VariantGridElementVerifier<TNode>(scoped in Grid TargetGrid, Identifier Identifier) where TNode : ShapeViewNode
{
	/// <summary>
	/// To verify whether the target puzzle is solved.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the puzzle is not solved.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ThrowIfNotSolved()
	{
		if (!TargetGrid.IsSolved)
		{
			throw new InvalidOperationException("The target grid must be solved.");
		}
	}

	/// <summary>
	/// Try to verify the current grid and get all possible view nodes.
	/// Put <see cref="ThrowIfNotSolved"/> into the first place if you want to verify the validity of the target puzzle.
	/// </summary>
	/// <returns>All view nodes.</returns>
	/// <seealso cref="ThrowIfNotSolved"/>
	public abstract TNode[] Verify();
}
