using System;
using static Sudoku.Data.GridOutputOptions;

namespace Sudoku.Data
{
	/// <summary>
	/// Provides a factory to create a <see cref="GridFormatter"/>.
	/// </summary>
	internal static class GridFormatFactory
	{
		/// <summary>
		/// Create a <see cref="GridFormatter"/> according to the specified grid output options.
		/// </summary>
		/// <param name="gridOutputOption">The grid output options.</param>
		/// <returns>The grid formatter.</returns>
		public static GridFormatter Create(GridOutputOptions gridOutputOption)
		{
			// Special cases.
			if (gridOutputOption == Excel)
			{
				return new(true) { Excel = true };
			}

			var formatter = new GridFormatter(gridOutputOption.HasFlag(Multiline));
			if (gridOutputOption.HasFlag(WithModifiers))
			{
				formatter.WithModifiables = true;
			}
			if (gridOutputOption.HasFlag(WithCandidates))
			{
				formatter.WithCandidates = true;
			}
			if (gridOutputOption.HasFlag(TreatValueAsGiven))
			{
				formatter.TreatValueAsGiven = true;
			}
			if (gridOutputOption.HasFlag(SubtleGridLines))
			{
				formatter.SubtleGridLines = true;
			}
			if (gridOutputOption.HasFlag(HodokuCompatible))
			{
				formatter.HodokuCompatible = true;
			}
			if (gridOutputOption == Sukaku)
			{
				formatter.Sukaku = true;
			}

			formatter.Placeholder = gridOutputOption.HasFlag(DotPlaceholder) ? '.' : '0';

			return formatter;
		}

		/// <summary>
		/// Create a <see cref="GridFormatter"/> according to the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <returns>The grid formatter.</returns>
		/// <exception cref="FormatException">
		/// Throws when the format string is invalid.
		/// </exception>
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
				"~." => new(false) { Sukaku = true, Placeholder = '.' },
				"@~" or "~@" => new(true) { Sukaku = true },
				"@~0" or "@0~" or "~@0" or "~0@" => new(true) { Sukaku = true, Placeholder = '0' },
				"@~." or "@.~" or "~@." or "~.@" => new(true) { Sukaku = true, Placeholder = '.' },
				"%" => new(true) { Excel = true },
				_ => throw new FormatException("The specified format is invalid.")
			};
	}
}
