namespace Sudoku.Text.Formatting;

/// <summary>
/// Indicates the factory that creates the grid formatter.
/// </summary>
internal static class GridFormatterFactory
{
	/// <summary>
	/// Get a built-in <see cref="IGridFormatter"/> instance according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	public static IGridFormatter? GetBuiltInFormatter(string? format)
		=> format switch
		{
			null or "." => SusserFormat.Default,
			"0" => SusserFormat.Default with { Placeholder = '0' },
			"0+" or "+0" => SusserFormat.Default with { Placeholder = '0', WithModifiables = true },
			"+" or ".+" or "+." => SusserFormat.Default with { WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#." => SusserFormat.Full,
			"0+:" or "+0:" or "#0" => SusserFormat.FullZero,
			":" or ".:" => SusserFormatEliminationsOnly.Default,
			"0:" => SusserFormatEliminationsOnly.Default with { Placeholder = '0' },
			"!" or ".!" or "!." => SusserFormatTreatingValuesAsGivens.Default,
			"0!" or "!0" => SusserFormatTreatingValuesAsGivens.Default with { Placeholder = '0' },
			".!:" or "!.:" => SusserFormatTreatingValuesAsGivens.Default with { WithCandidates = true },
			"0!:" or "!0:" => SusserFormatTreatingValuesAsGivens.Default with { Placeholder = '0', WithCandidates = true },
			".*" or "*." => SusserFormat.Default with { Placeholder = '.', ShortenSusser = true },
			"0*" or "*0" => SusserFormat.Default with { Placeholder = '0', ShortenSusser = true },
			"@" or "@." => MultipleLineFormat.Default,
			"@*" or "@.*" or "@*." => MultipleLineFormat.Default with { SubtleGridLines = false },
			"@0" => MultipleLineFormat.Default with { Placeholder = '0' },
			"@0!" or "@!0" => MultipleLineFormat.Default with { Placeholder = '0', TreatValueAsGiven = true },
			"@0*" or "@*0" => MultipleLineFormat.Default with { Placeholder = '0', SubtleGridLines = false },
			"@!" or "@.!" or "@!." => MultipleLineFormat.Default with { TreatValueAsGiven = true },
			"@!*" or "@*!" => MultipleLineFormat.Default with { TreatValueAsGiven = true, SubtleGridLines = false },
			"@:" => PencilMarkFormat.Default,
			"@*:" or "@:*" => PencilMarkFormat.Default with { SubtleGridLines = false },
			"@:!" or "@!:" => PencilMarkFormat.Default with { TreatValueAsGiven = true },
			"@!*:" or "@*!:" or "@!:*" or "@*:!" or "@:!*" or "@:*!" => PencilMarkFormat.Default with { TreatValueAsGiven = true, SubtleGridLines = false },
			"~." => SukakuFormat.Default,
			"~" or "~0" => SukakuFormat.Default with { Placeholder = '0' },
			"@~" or "~@" or "@~." or "@.~" or "~@." or "~.@" => SukakuFormat.Default with { Multiline = true },
			"@~0" or "@0~" or "~@0" or "~0@" => SukakuFormat.Default with { Multiline = true, Placeholder = '0' },
			_ => null
		};
}
