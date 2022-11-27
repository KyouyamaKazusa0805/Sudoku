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
	/// The table of adjacent cells data.
	/// </summary>
	protected static readonly int[]?[] AdjacentCellPairsTable;


	/// <summary>
	/// Initializes for field <see cref="AdjacentCellPairsTable"/>.
	/// </summary>
	static VariantGridElementVerifier()
	{
		using scoped var list = new ValueList<int>(2);
		var currentCellGroup = new int[]?[81];
		for (var row = 0; row < 9; row++)
		{
			for (var column = 0; column < 9; column++)
			{
				var cell = row * 9 + column;
				var adjacent1 = column + 1 >= 9 ? -1 : row * 9 + column + 1;
				var adjacent2 = row + 1 >= 9 ? -1 : (row + 1) * 9 + column;
				currentCellGroup[cell] = (adjacent1, adjacent2) switch
				{
					(-1, -1) => null,
					(_, -1) => new[] { adjacent1 },
					(-1, _) => new[] { adjacent2 },
					_ => new[] { adjacent1, adjacent2 }
				};
			}
		}

		AdjacentCellPairsTable = currentCellGroup;
	}


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
