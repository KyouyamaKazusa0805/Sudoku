namespace Sudoku.Variants.Verifiers;

/// <summary>
/// Defines a verifier that checks for consecutive border bars.
/// </summary>
/// <param name="TargetGrid"><inheritdoc path="/param[@name='TargetGrid']"/></param>
/// <param name="Identifier"><inheritdoc path="/param[@name='Identifier']"/></param>
public sealed record ConsecutiveSudoku(scoped in Grid TargetGrid, Identifier Identifier) :
	VariantGridElementVerifier<BorderBarViewNode>(TargetGrid, Identifier)
{
	/// <inheritdoc/>
	public override BorderBarViewNode[] Verify()
	{
		ThrowIfNotSolved();

		var result = new List<BorderBarViewNode>();
		for (var cell = 0; cell < 81; cell++)
		{
			var a = TargetGrid[cell];
			foreach (var adjacent in AdjacentCellPairsTable[cell] ?? Array.Empty<int>())
			{
				var b = TargetGrid[adjacent];
				if (Abs(a - b) == 1)
				{
					result.Add(new(Identifier, cell, adjacent));
				}
			}
		}

		return result.ToArray();
	}
}
