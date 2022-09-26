namespace Sudoku.Presentation;

/// <summary>
/// Provides with extension methods on <see cref="View"/>.
/// </summary>
/// <seealso cref="View"/>
public static class ViewExtensions
{
	/// <summary>
	/// Determines whether the specified <see cref="View"/> stores several <see cref="UnknownViewNode"/>s,
	/// and at least one of it overlaps the specified cell.
	/// </summary>
	/// <param name="this">The view instance.</param>
	/// <param name="cell">The cell.</param>
	/// <returns>A <see cref="bool"/> value indicating whether being overlapped.</returns>
	public static bool UnknownOverlaps(this View? @this, int cell)
	{
		if (@this is null)
		{
			return false;
		}

		foreach (var unknownNode in @this.UnknownNodes)
		{
			if (unknownNode.Cell == cell)
			{
				return true;
			}
		}

		return false;
	}
}
