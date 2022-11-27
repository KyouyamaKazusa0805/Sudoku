namespace Sudoku.Variants.Verifiers;

/// <summary>
/// Defines a verifier that checks for Kropki dots.
/// </summary>
/// <param name="TargetGrid"><inheritdoc path="/param[@name='TargetGrid']"/></param>
/// <param name="Identifier"><inheritdoc path="/param[@name='Identifier']"/></param>
public sealed record KropkiSudoku(scoped in Grid TargetGrid, Identifier Identifier) :
	VariantGridElementVerifier<KropkiDotViewNode>(TargetGrid, Identifier)
{
	/// <inheritdoc/>
	public override KropkiDotViewNode[] Verify()
	{
		ThrowIfNotSolved();

		var result = new List<KropkiDotViewNode>();
		var random = new Random();
		for (var cell = 0; cell < 81; cell++)
		{
			var a = TargetGrid[cell];
			foreach (var adjacent in AdjacentCellPairsTable[cell] ?? Array.Empty<int>())
			{
				var b = TargetGrid[adjacent];
				switch (a, b)
				{
					case (1, 2) or (2, 1):
					{
						// This case will use randomized algorithm to decide whether displaying as solid circles.
						result.Add(new(Identifier, cell, adjacent, random.Next(1, 100) >= 50));
						break;
					}
					case var _ when a - b == 1 || b - a == 1:
					{
						result.Add(new(Identifier, cell, adjacent, false));
						break;
					}
					case var _ when a << 1 == b || a == b << 1:
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
