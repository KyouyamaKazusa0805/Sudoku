namespace Sudoku.Text.SudokuGrid;

/// <summary>
/// Indicates the factory that creates the grid formatter.
/// </summary>
internal static class GridFormatterFactory
{
	/// <summary>
	/// Get a built-in <see cref="GridConverter"/> instance according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	public static GridConverter? GetBuiltInConverter(string? format)
		=> format switch
		{
			null or "." => SusserConverter.Default,
			"0" => SusserConverter.Default with { Placeholder = '0' },
			"0+" or "+0" => SusserConverter.Default with { Placeholder = '0', WithModifiables = true },
			"+" or ".+" or "+." => SusserConverter.Default with { WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#." => SusserConverter.Full,
			"0+:" or "+0:" or "#0" => SusserConverter.FullZero,
			":" or ".:" => SusserEliminationsConverter.Default,
			"0:" => SusserEliminationsConverter.Default with { Placeholder = '0' },
			"!" or ".!" or "!." => SusserConverterTreatingValuesAsGivens.Default,
			"0!" or "!0" => SusserConverterTreatingValuesAsGivens.Default with { Placeholder = '0' },
			".!:" or "!.:" => SusserConverterTreatingValuesAsGivens.Default with { WithCandidates = true },
			"0!:" or "!0:" => SusserConverterTreatingValuesAsGivens.Default with { Placeholder = '0', WithCandidates = true },
			".*" or "*." => SusserConverter.Default with { Placeholder = '.', ShortenSusser = true },
			"0*" or "*0" => SusserConverter.Default with { Placeholder = '0', ShortenSusser = true },
			"@" or "@." => MultipleLineConverter.Default,
			"@*" or "@.*" or "@*." => MultipleLineConverter.Default with { SubtleGridLines = false },
			"@0" => MultipleLineConverter.Default with { Placeholder = '0' },
			"@0!" or "@!0" => MultipleLineConverter.Default with { Placeholder = '0', TreatValueAsGiven = true },
			"@0*" or "@*0" => MultipleLineConverter.Default with { Placeholder = '0', SubtleGridLines = false },
			"@!" or "@.!" or "@!." => MultipleLineConverter.Default with { TreatValueAsGiven = true },
			"@!*" or "@*!" => MultipleLineConverter.Default with { TreatValueAsGiven = true, SubtleGridLines = false },
			"@:" => new PencilmarkingConverter(),
			"@*:" or "@:*" => new PencilmarkingConverter() with { SubtleGridLines = false },
			"@:!" or "@!:" => new PencilmarkingConverter() with { TreatValueAsGiven = true },
			"@!*:" or "@*!:" or "@!:*" or "@*:!" or "@:!*" or "@:*!" => new PencilmarkingConverter() with { TreatValueAsGiven = true, SubtleGridLines = false },
			"~." => SukakuConverter.Default,
			"~" or "~0" => SukakuConverter.Default with { Placeholder = '0' },
			"@~" or "~@" or "@~." or "@.~" or "~@." or "~.@" => SukakuConverter.Default with { Multiline = true },
			"@~0" or "@0~" or "~@0" or "~0@" => SukakuConverter.Default with { Multiline = true, Placeholder = '0' },
			_ => null
		};
}
