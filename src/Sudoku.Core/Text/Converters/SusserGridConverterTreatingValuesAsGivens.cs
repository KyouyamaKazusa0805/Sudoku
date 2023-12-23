namespace Sudoku.Text.Converters;

/// <summary>
/// Represents with a Susser formatter, removing all plus mark <c>'+'</c> as modifiable distinction tokens.
/// </summary>
public sealed record SusserGridConverterTreatingValuesAsGivens : SusserGridConverter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SusserGridConverter.Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="SusserGridConverter.WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="SusserGridConverter.WithCandidates"/>: <see langword="false"/></item>
	/// <item><see cref="SusserGridConverter.ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static new readonly SusserGridConverterTreatingValuesAsGivens Default = new()
	{
		Placeholder = SusserGridConverter.Default.Placeholder,
		WithModifiables = true
	};


	/// <inheritdoc/>
	public override FuncRefReadOnly<Grid, string> Converter
		=> (scoped ref readonly Grid grid) => base.Converter(in grid).RemoveAll(ModifiablePrefix);
}
