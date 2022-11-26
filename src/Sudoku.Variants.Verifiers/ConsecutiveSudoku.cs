namespace Sudoku.Variants.Verifiers;

/// <summary>
/// Defines a verifier that checks for consecutive border bars.
/// </summary>
public sealed class ConsecutiveSudoku : VariantGridElementVerifier<BorderBarViewNode>
{
	/// <summary>
	/// Initializes a <see cref="ConsecutiveSudoku"/> instance via the target grid and identifier.
	/// </summary>
	/// <param name="targetGrid">The target grid.</param>
	/// <param name="identifier">The identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ConsecutiveSudoku(scoped in Grid targetGrid, Identifier identifier) : base(targetGrid, identifier)
	{
	}


	/// <inheritdoc/>
	public override BorderBarViewNode[] Verify()
	{
		var result = new List<BorderBarViewNode>();

		for (var row = 0; row < 8; row++)
		{
			for (var column = 0; column < 8; column++)
			{
				var cell = row * 9 + column;
				var adjacent1 = row * 9 + column + 1;
				var adjacent2 = (row + 1) * 9 + column;

				var a = TargetGrid[cell];
				foreach (var (adjacent, b) in stackalloc[] { (adjacent1, TargetGrid[adjacent1]), (adjacent2, TargetGrid[adjacent2]) })
				{
					if (Abs(a - b) == 1)
					{
						result.Add(new(Identifier, cell, adjacent));
					}
				}
			}
		}

		return result.ToArray();
	}
}
