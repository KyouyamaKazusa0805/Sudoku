﻿namespace Sudoku.Text.Notations;

/// <summary>
/// Provides with a type that is used for displaying a cell list, using RxCy notation.
/// </summary>
/// <param name="UpperCasing">
/// Indicates whether we should use upper-casing to handle the result notation of cells.
/// For example, if <see langword="true"/>, the cell at row 3 and column 3 will be displayed
/// as <c>R3C3</c>; otherwise, <c>r3c3</c>.
/// </param>
/// <param name="Separator">
/// Indicates the separator string value that inserts two coordinate elements, to combine them.
/// For example, cells <c>R3C1</c> and <c>R4C2</c> can be combined to <c>R3C1, R4C2</c> if the separator
/// is <c>", "</c>.
/// </param>
public readonly record struct RxCyNotationOptions(bool UpperCasing = false, string Separator = ", ") :
	INotationOptions<RxCyNotationOptions>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static RxCyNotationOptions Default => new(false);
}
