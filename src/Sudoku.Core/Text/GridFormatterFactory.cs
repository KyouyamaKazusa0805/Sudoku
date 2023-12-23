namespace Sudoku.Text;

/// <summary>
/// Indicates the factory that creates the grid formatter.
/// </summary>
internal static class GridFormatterFactory
{
	/// <summary>
	/// Get a built-in converter instance according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	public static IConceptConverter<Grid>? GetBuiltInConverter(string? format)
		=> format switch
		{
			null or "." => SusserGridConverter.Default,
			"0" => SusserGridConverter.Default with { Placeholder = '0' },
			"0+" or "+0" => SusserGridConverter.Default with { Placeholder = '0', WithModifiables = true },
			"+" or ".+" or "+." => SusserGridConverter.Default with { WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#." => SusserGridConverter.Full,
			"^+:" or "^:+" or "^.+:" or "^#" or "^#." => SusserGridConverter.Full with { NegateEliminationsTripletRule = true },
			"0+:" or "+0:" or "#0" => SusserGridConverter.FullZero,
			"^0+:" or "^+0:" or "^#0" => SusserGridConverter.FullZero with { NegateEliminationsTripletRule = true },
			":" or ".:" => SusserEliminationsGridConverter.Default,
			"0:" => SusserEliminationsGridConverter.Default with { Placeholder = '0' },
			"!" or ".!" or "!." => SusserGridConverterTreatingValuesAsGivens.Default,
			"0!" or "!0" => SusserGridConverterTreatingValuesAsGivens.Default with { Placeholder = '0' },
			".!:" or "!.:" => SusserGridConverterTreatingValuesAsGivens.Default with { WithCandidates = true },
			"^.!:" or "^!.:" => SusserGridConverterTreatingValuesAsGivens.Default with { WithCandidates = true, NegateEliminationsTripletRule = true },
			"0!:" or "!0:" => SusserGridConverterTreatingValuesAsGivens.Default with { Placeholder = '0', WithCandidates = true },
			"^0!:" or "^!0:" => SusserGridConverterTreatingValuesAsGivens.Default with { Placeholder = '0', WithCandidates = true, NegateEliminationsTripletRule = true },
			".*" or "*." => SusserGridConverter.Default with { Placeholder = '.', ShortenSusser = true },
			"0*" or "*0" => SusserGridConverter.Default with { Placeholder = '0', ShortenSusser = true },
			"@" or "@." => MultipleLineGridConverter.Default,
			"@*" or "@.*" or "@*." => MultipleLineGridConverter.Default with { SubtleGridLines = false },
			"@0" => MultipleLineGridConverter.Default with { Placeholder = '0' },
			"@0!" or "@!0" => MultipleLineGridConverter.Default with { Placeholder = '0', TreatValueAsGiven = true },
			"@0*" or "@*0" => MultipleLineGridConverter.Default with { Placeholder = '0', SubtleGridLines = false },
			"@!" or "@.!" or "@!." => MultipleLineGridConverter.Default with { TreatValueAsGiven = true },
			"@!*" or "@*!" => MultipleLineGridConverter.Default with { TreatValueAsGiven = true, SubtleGridLines = false },
			"@:" => new PencilmarkingGridConverter(),
			"@*:" or "@:*" => new PencilmarkingGridConverter() with { SubtleGridLines = false },
			"@:!" or "@!:" => new PencilmarkingGridConverter() with { TreatValueAsGiven = true },
			"@!*:" or "@*!:" or "@!:*" or "@*:!" or "@:!*" or "@:*!" => new PencilmarkingGridConverter() with { TreatValueAsGiven = true, SubtleGridLines = false },
			"~." => SukakuGridConverter.Default,
			"~" or "~0" => SukakuGridConverter.Default with { Placeholder = '0' },
			"@~" or "~@" or "@~." or "@.~" or "~@." or "~.@" => SukakuGridConverter.Default with { Multiline = true },
			"@~0" or "@0~" or "~@0" or "~0@" => SukakuGridConverter.Default with { Multiline = true, Placeholder = '0' },
			_ => null
		};
}
