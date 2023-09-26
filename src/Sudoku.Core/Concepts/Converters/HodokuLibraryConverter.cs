using Sudoku.Concepts;

namespace Sudoku.Text.SudokuGrid;

/// <summary>
/// Represents a type that converts from a <see cref="Grid"/> into an equivalent <see cref="string"/> representation
/// using Hodoku library text rule.
/// </summary>
public sealed record HodokuLibraryConverter : SusserConverter
{
	/// <summary>
	/// Indicates the format prefix.
	/// </summary>
	private const string FormatPrefix = ":0000:x:";


	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static new readonly HodokuLibraryConverter Default = new() { Placeholder = SusserConverter.Default.Placeholder };


	/// <summary>
	/// Indicates the format suffix.
	/// </summary>
	private string FormatSuffix => new(':', WithCandidates ? 2 : 3);


	/// <inheritdoc/>
	public override GridNotationConverter Converter
		=> (scoped ref readonly Grid grid) => $"{FormatPrefix}{base.Converter(in grid)}{FormatSuffix}";
}
