namespace Sudoku.Variants.Verifiers;

/// <summary>
/// Defines a verifier that can get all possbible variant elements in a solution grid.
/// </summary>
/// <typeparam name="TNode">The type of the node.</typeparam>
public abstract class VariantGridElementVerifier<TNode> where TNode : ShapeViewNode
{
	/// <summary>
	/// Assigns <see cref="Grid"/> instance to the target propety <see cref="TargetGrid"/> and identifier.
	/// </summary>
	/// <param name="targetGrid">The target grid.</param>
	/// <param name="identifier">The identifier.</param>
	/// <exception cref="ArgumentException">Throws when the argument <paramref name="targetGrid"/> must be solved.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	protected VariantGridElementVerifier(scoped in Grid targetGrid, Identifier identifier)
		=> (TargetGrid, Identifier) = (targetGrid.IsSolved ? targetGrid : throw new ArgumentException("The argument must be solved.", nameof(targetGrid)), identifier);


	/// <summary>
	/// Indicates the target grid used.
	/// </summary>
	public Grid TargetGrid { get; }

	/// <summary>
	/// Indicates the identifier used.
	/// </summary>
	public Identifier Identifier { get; }


	/// <summary>
	/// Try to verify the current grid and get all possible view nodes.
	/// </summary>
	/// <returns>All view nodes.</returns>
	public abstract TNode[] Verify();
}
