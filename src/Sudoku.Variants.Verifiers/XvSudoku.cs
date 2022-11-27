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
		for (var row = 0; row < 9; row++)
		{
			for (var column = 0; column < 9; column++)
			{
				var cell = row * 9 + column;
				var adjacent1 = column + 1 >= 9 ? -1 : row * 9 + column + 1;
				var adjacent2 = row + 1 >= 9 ? -1 : (row + 1) * 9 + column;

				var a = TargetGrid[cell];
				foreach (var adjacent in stackalloc[] { adjacent1, adjacent2 })
				{
					if (adjacent == -1)
					{
						continue;
					}

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
		}

		return result.ToArray();
	}
}
