namespace Sudoku.Variants.Verifiers;

/// <summary>
/// Defines a verifier that checks for XV marks.
/// </summary>
/// <param name="TargetGrid"><inheritdoc path="/param[@name='TargetGrid']"/></param>
/// <param name="Identifier"><inheritdoc path="/param[@name='Identifier']"/></param>
public sealed record XvSudoku(scoped in Grid TargetGrid, Identifier Identifier) :
	VariantGridElementVerifier<XvSignViewNode>(TargetGrid, Identifier)
{
	/// <inheritdoc/>
	public override XvSignViewNode[] Verify()
	{
		ThrowIfNotSolved();

		var result = new List<XvSignViewNode>();
		for (var cell = 0; cell < 81; cell++)
		{
			var a = TargetGrid[cell];
			foreach (var adjacent in AdjacentCellPairsTable[cell] ?? Array.Empty<int>())
			{
				var b = TargetGrid[adjacent];
				switch (a + b + 2) // (a + 1) + (b + 1)
				{
					case 5:
					{
						result.Add(new(Identifier, cell, adjacent, false));
						break;
					}
					case 10:
					{
						result.Add(new(Identifier, cell, adjacent, true));
						break;
					}
				}
			}
		}

		return result.ToArray();
	}
}
