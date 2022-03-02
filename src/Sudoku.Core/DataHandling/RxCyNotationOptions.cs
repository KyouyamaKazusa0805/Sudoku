namespace Sudoku.DataHandling;

/// <summary>
/// Provides with a type that is used for displaying a cell list, using RxCy notation.
/// </summary>
/// <param name="UpperCasing">
/// Indicates whether we should use upper-casing to handle the result notation of cells.
/// For example, if <see langword="true"/>, the cell at row 3 and column 3 will be displayed
/// as <c>R3C3</c>; otherwise, <c>r3c3</c>.
/// </param>
public readonly record struct RxCyNotationOptions(bool UpperCasing)
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly RxCyNotationOptions Default = new(false);
}
