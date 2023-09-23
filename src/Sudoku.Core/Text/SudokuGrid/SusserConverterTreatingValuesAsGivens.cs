using Sudoku.Concepts;

namespace Sudoku.Text.SudokuGrid;

/// <summary>
/// Represents with a Susser formatter, removing all plus mark <c>'+'</c> as modifiable distinction tokens.
/// </summary>
public sealed record SusserConverterTreatingValuesAsGivens : SusserConverter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SusserConverter.Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="SusserConverter.WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="SusserConverter.WithCandidates"/>: <see langword="false"/></item>
	/// <item><see cref="SusserConverter.ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static new readonly SusserConverterTreatingValuesAsGivens Default = new()
	{
		Placeholder = SusserConverter.Default.Placeholder,
		WithModifiables = true
	};


	/// <inheritdoc/>
	public override GridNotationConverter TargetConverter
		=> (scoped ref readonly Grid grid) => base.TargetConverter(in grid).RemoveAll(ModifiablePrefix);
}
