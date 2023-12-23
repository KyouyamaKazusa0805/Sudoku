namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a type that converts from a <see cref="Grid"/> into an equivalent <see cref="string"/> representation
/// using Hodoku library text rule.
/// </summary>
public sealed record HodokuLibraryGridConverter : SusserGridConverter
{
	/// <summary>
	/// Indicates the format prefix.
	/// </summary>
	private const string FormatPrefix = ":0000:x:";


	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static new readonly HodokuLibraryGridConverter Default = new() { Placeholder = SusserGridConverter.Default.Placeholder };


	/// <summary>
	/// Indicates the format suffix.
	/// </summary>
	private string FormatSuffix => new(':', WithCandidates ? 2 : 3);


	/// <inheritdoc/>
	public override FuncRefReadOnly<Grid, string> Converter
		=> (scoped ref readonly Grid grid) => $"{FormatPrefix}{base.Converter(in grid)}{FormatSuffix}";
}
