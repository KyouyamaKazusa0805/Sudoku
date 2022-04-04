namespace Sudoku.Concepts.Formatting;

/// <summary>
/// Indicates the factory that creates the grid formatter.
/// </summary>
public static class GridFormatterFactory
{
	/// <summary>
	/// Create a <see cref="GridFormatter"/> according to the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The grid formatter.</returns>
	/// <exception cref="FormatException">Throws when the format string is invalid.</exception>
	public static GridFormatter Create(string? format) =>
		format switch
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
			"~" or "~0" => new(false) { Sukaku = true, Placeholder = '0' },
			"~." => new(false) { Sukaku = true },
			"@~" or "~@" => new(true) { Sukaku = true },
			"@~0" or "@0~" or "~@0" or "~0@" => new(true) { Sukaku = true, Placeholder = '0' },
			"@~." or "@.~" or "~@." or "~.@" => new(true) { Sukaku = true },
			"%" => new(true) { Excel = true },
			"^" => new(false) { OpenSudoku = true },
			_ => throw new FormatException("The specified format is invalid.")
		};
}
