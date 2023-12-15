using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Concepts;

/// <summary>
/// Provide with extension methdos on <see cref="Chute"/>.
/// </summary>
/// <seealso cref="Chute"/>
public static class ChuteExtensions
{
	/// <summary>
	/// Try to get the band index (mega-row) of the specified cell.
	/// </summary>
	/// <param name="this">The cell.</param>
	/// <returns>The chute index.</returns>
	public static int ToBandIndex(this Cell @this)
	{
		for (var i = 0; i < 3; i++)
		{
			if (Chutes[i].Cells.Contains(@this))
			{
				return i;
			}
		}

		return -1;
	}

	/// <summary>
	/// Try to get the tower index (mega-column) of the specified cell.
	/// </summary>
	/// <param name="this">The cell.</param>
	/// <returns>The chute index.</returns>
	public static int ToTowerIndex(this Cell @this)
	{
		for (var i = 3; i < 6; i++)
		{
			if (Chutes[i].Cells.Contains(@this))
			{
				return i;
			}
		}

		return -1;
	}
}
