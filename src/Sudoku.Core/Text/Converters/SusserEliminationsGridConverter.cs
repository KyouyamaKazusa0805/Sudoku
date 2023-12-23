namespace Sudoku.Text.Converters;

/// <summary>
/// Represents with a Susser format, but only extracts for pre-eliminations.
/// </summary>
public sealed partial record SusserEliminationsGridConverter : SusserGridConverter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SusserGridConverter.Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="SusserGridConverter.WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="SusserGridConverter.WithCandidates"/>: <see langword="true"/></item>
	/// <item><see cref="SusserGridConverter.ShortenSusser"/>: <see langword="false"/></item>
	/// <item><see cref="SusserGridConverter.NegateEliminationsTripletRule"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static new readonly SusserEliminationsGridConverter Default = new()
	{
		Placeholder = SusserGridConverter.Default.Placeholder,
		WithModifiables = true,
		WithCandidates = true,
		ShortenSusser = false,
		NegateEliminationsTripletRule = false
	};


	/// <inheritdoc/>
	public override FuncRefReadOnly<Grid, string> Converter
		=> (scoped ref readonly Grid grid) => EliminationPattern().Match(base.Converter(in grid)) is { Success: true, Value: var value } ? value : string.Empty;


	[GeneratedRegex("""(?<=\:)(\d{3}\s+)*\d{3}""", RegexOptions.Compiled, 5000)]
	internal static partial Regex EliminationPattern();
}
