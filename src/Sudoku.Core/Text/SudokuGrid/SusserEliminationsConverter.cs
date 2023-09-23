using System.Text.RegularExpressions;
using Sudoku.Concepts;

namespace Sudoku.Text.SudokuGrid;

/// <summary>
/// Represents with a Susser format, but only extracts for pre-eliminations.
/// </summary>
public sealed partial record SusserEliminationsConverter : SusserConverter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SusserConverter.Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="SusserConverter.WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="SusserConverter.WithCandidates"/>: <see langword="true"/></item>
	/// <item><see cref="SusserConverter.ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static new readonly SusserEliminationsConverter Default = new()
	{
		Placeholder = SusserConverter.Default.Placeholder,
		WithModifiables = true,
		WithCandidates = true
	};


	/// <inheritdoc/>
	public override GridNotationConverter TargetConverter
		=> (scoped ref readonly Grid grid) => EliminationPattern().Match(base.TargetConverter(in grid)) is { Success: true, Value: var value } ? value : string.Empty;


	[GeneratedRegex("""(?<=\:)(\d{3}\s+)*\d{3}""", RegexOptions.Compiled, 5000)]
	internal static partial Regex EliminationPattern();
}
