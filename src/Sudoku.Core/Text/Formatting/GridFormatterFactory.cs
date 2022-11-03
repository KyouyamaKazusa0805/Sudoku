namespace Sudoku.Text.Formatting;

/// <summary>
/// Indicates the factory that creates the grid formatter.
/// </summary>
internal static class GridFormatterFactory
{
	/// <summary>
	/// Create a <see cref="GridFormatter"/> according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	[Obsolete($"Use type '{nameof(IGridFormatter)}' and method '{nameof(Grid)}.{nameof(Grid.ToString)}({nameof(IGridFormatter)})' instead.", false)]
	public static GridFormatter Create(string? format)
		=> format switch
		{
			null or "." => new(false),
			"+" or ".+" or "+." => new(false) { WithModifiables = true },
			"0" => new(false) { Placeholder = '0' },
			":" => new(false) { WithCandidates = true },
			"!" or ".!" or "!." => new(false) { WithModifiables = true, TreatValueAsGiven = true },
			"0!" or "!0" => new(false) { Placeholder = '0', WithModifiables = true, TreatValueAsGiven = true },
			".:" => new(false) { WithCandidates = true },
			"0:" => new(false) { Placeholder = '0', WithCandidates = true },
			"0+" or "+0" => new(false) { Placeholder = '0', WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#." => new(false) { WithModifiables = true, WithCandidates = true },
			"0+:" or "+0:" or "#0" => new(false) { Placeholder = '0', WithModifiables = true, WithCandidates = true },
			".!:" or "!.:" => new(false) { WithModifiables = true, TreatValueAsGiven = true },
			"0!:" or "!0:" => new(false) { Placeholder = '0', WithModifiables = true, TreatValueAsGiven = true },
			".*" or "*." => new(false) { Placeholder = '.', ShortenSusser = true },
			"0*" or "*0" => new(false) { Placeholder = '0', ShortenSusser = true },
			"@" or "@." => new(true) { SubtleGridLines = true },
			"@0" => new(true) { Placeholder = '0', SubtleGridLines = true },
			"@!" or "@.!" or "@!." => new(true) { TreatValueAsGiven = true, SubtleGridLines = true },
			"@0!" or "@!0" => new(true) { Placeholder = '0', TreatValueAsGiven = true, SubtleGridLines = true },
			"@*" or "@.*" or "@*." => new(true),
			"@0*" or "@*0" => new(true) { Placeholder = '0' },
			"@!*" or "@*!" => new(true) { TreatValueAsGiven = true },
			"@:" => new(true) { WithCandidates = true, SubtleGridLines = true },
			"@:!" or "@!:" => new(true) { WithCandidates = true, TreatValueAsGiven = true, SubtleGridLines = true },
			"@*:" or "@:*" => new(true) { WithCandidates = true },
			"@!*:" or "@*!:" or "@!:*" or "@*:!" or "@:!*" or "@:*!" => new(true) { WithCandidates = true, TreatValueAsGiven = true },
			"~." => new(false) { Sukaku = true },
			"@~" or "~@" or "@~." or "@.~" or "~@." or "~.@" => new(true) { Sukaku = true },
			"~" or "~0" => new(false) { Sukaku = true, Placeholder = '0' },
			"@~0" or "@0~" or "~@0" or "~0@" => new(true) { Sukaku = true, Placeholder = '0' },
			"%" => new(true) { Excel = true },
			"^" => new(false) { OpenSudoku = true },
			_ => throw new FormatException("The specified format is invalid.")
		};

#pragma warning disable format
	/// <summary>
	/// Create a <see cref="IGridFormatter"/> according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	public static IGridFormatter CreateFormatter(string? format)
		=> format switch
		{
			null or "."													=> SusserFormat.Default,
			"0"															=> SusserFormat.Default with { Placeholder = '0' },
			"0+" or "+0"												=> SusserFormat.Default with { Placeholder = '0', WithModifiables = true },
			"0+:" or "+0:" or "#0"										=> SusserFormat.Default with { Placeholder = '0', WithModifiables = true, WithCandidates = true },
			"+" or ".+" or "+."											=> SusserFormat.Default with { WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#."						=> SusserFormat.Default with { WithModifiables = true, WithCandidates = true },
			":" or ".:"													=> SusserFormatEliminationsOnly.Default,
			"0:"														=> SusserFormatEliminationsOnly.Default with { Placeholder = '0' },
			"!" or ".!" or "!."											=> SusserFormatTreatingValuesAsGivens.Default,
			"0!" or "!0"												=> SusserFormatTreatingValuesAsGivens.Default with { Placeholder = '0' },
			".!:" or "!.:"												=> SusserFormatTreatingValuesAsGivens.Default with { WithCandidates = true },
			"0!:" or "!0:"												=> SusserFormatTreatingValuesAsGivens.Default with { Placeholder = '0', WithCandidates = true },
			".*" or "*."												=> SusserFormat.Default with { Placeholder = '.', ShortenSusser = true },
			"0*" or "*0"												=> SusserFormat.Default with { Placeholder = '0', ShortenSusser = true },
			"@*" or "@.*" or "@*."										=> MultipleLineFormat.Default,
			"@" or "@."													=> MultipleLineFormat.Default with { SubtleGridLines = true },
			"@0"														=> MultipleLineFormat.Default with { Placeholder = '0', SubtleGridLines = true },
			"@!" or "@.!" or "@!."										=> MultipleLineFormat.Default with { TreatValueAsGiven = true, SubtleGridLines = true },
			"@0!" or "@!0"												=> MultipleLineFormat.Default with { Placeholder = '0', TreatValueAsGiven = true, SubtleGridLines = true },
			"@0*" or "@*0"												=> MultipleLineFormat.Default with { Placeholder = '0' },
			"@!*" or "@*!"												=> MultipleLineFormat.Default with { TreatValueAsGiven = true },
			"@*:" or "@:*"												=> PencilMarkFormat.Default,
			"@:"														=> PencilMarkFormat.Default with { SubtleGridLines = true },
			"@:!" or "@!:"												=> PencilMarkFormat.Default with { TreatValueAsGiven = true, SubtleGridLines = true },
			"@!*:" or "@*!:" or "@!:*" or "@*:!" or "@:!*" or "@:*!"	=> PencilMarkFormat.Default with { TreatValueAsGiven = true },
			"~."														=> SukakuFormat.Default,
			"~" or "~0"													=> SukakuFormat.Default with { Placeholder = '0' },
			"@~" or "~@" or "@~." or "@.~" or "~@." or "~.@"			=> SukakuFormat.Default with { Multiline = true },
			"@~0" or "@0~" or "~@0" or "~0@"							=> SukakuFormat.Default with { Multiline = true, Placeholder = '0' },
			"%"															=> ExcelFormat.Default,
			"^"															=> OpenSudokuFormat.Default,
			_															=> throw new FormatException("The specified format is invalid.")
		};
#pragma warning restore format
}
