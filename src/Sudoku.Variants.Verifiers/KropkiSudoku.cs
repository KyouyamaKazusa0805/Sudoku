namespace Sudoku.Variants.Verifiers;

/// <summary>
/// Defines a verifier that checks for Kropki dots.
/// </summary>
public sealed class KropkiSudoku : VariantGridElementVerifier<KropkiDotViewNode>
{
	/// <summary>
	/// Initializes a <see cref="KropkiSudoku"/> instance via the target grid and identifier.
	/// </summary>
	/// <param name="targetGrid">The target grid.</param>
	/// <param name="identifier">The identifier.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public KropkiSudoku(scoped in Grid targetGrid, Identifier identifier) : base(targetGrid, identifier)
	{
	}


	/// <inheritdoc/>
	public override KropkiDotViewNode[] Verify()
	{
		var result = new List<KropkiDotViewNode>();
		var random = new Random();

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
		}

		return result.ToArray();
	}
}
