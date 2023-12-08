using Sudoku.Text.Converters;
using Sudoku.Text.SudokuGrid;

namespace Sudoku.Text;

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
			null or "." => SusserGridConverter.Default,
			"0" => SusserGridConverter.Default with { Placeholder = '0' },
			"0+" or "+0" => SusserGridConverter.Default with { Placeholder = '0', WithModifiables = true },
			"+" or ".+" or "+." => SusserGridConverter.Default with { WithModifiables = true },
			"+:" or "+.:" or ".+:" or "#" or "#." => SusserGridConverter.Full,
			"0+:" or "+0:" or "#0" => SusserGridConverter.FullZero,
			":" or ".:" => SusserEliminationsGridConverter.Default,
			"0:" => SusserEliminationsGridConverter.Default with { Placeholder = '0' },
			"!" or ".!" or "!." => SusserGridConverterTreatingValuesAsGivens.Default,
			"0!" or "!0" => SusserGridConverterTreatingValuesAsGivens.Default with { Placeholder = '0' },
			".!:" or "!.:" => SusserGridConverterTreatingValuesAsGivens.Default with { WithCandidates = true },
			"0!:" or "!0:" => SusserGridConverterTreatingValuesAsGivens.Default with { Placeholder = '0', WithCandidates = true },
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