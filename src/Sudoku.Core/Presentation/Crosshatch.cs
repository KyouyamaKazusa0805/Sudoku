namespace Sudoku.Presentation;

/// <summary>
/// Defines a crosshatching line that used for displaying in a picture.
/// </summary>
/// <param name="Start">The start position.</param>
/// <param name="End">The end position.</param>
public readonly partial record struct Crosshatch(in Cells Start, in Cells End)
{
	/// <inheritdoc/>
	public bool Equals(in Crosshatch other) => Start == other.Start && End == other.End;
}
