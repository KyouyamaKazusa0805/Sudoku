using System;

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
			var formatter = new GridFormatter(gridOutputOption.HasFlag(GridOutputOptions.Multiline));
			if (gridOutputOption.HasFlag(GridOutputOptions.WithModifiers))
			{
				formatter.WithModifiables = true;
			}
			if (gridOutputOption.HasFlag(GridOutputOptions.WithCandidates))
			{
				formatter.WithCandidates = true;
			}
			if (gridOutputOption.HasFlag(GridOutputOptions.TreatValueAsGiven))
			{
				formatter.TreatValueAsGiven = true;
			}
			if (gridOutputOption.HasFlag(GridOutputOptions.SubtleGridLines))
			{
				formatter.SubtleGridLines = true;
			}
			if (gridOutputOption.HasFlag(GridOutputOptions.HodokuCompatible))
			{
				formatter.HodokuCompatible = true;
			}
			formatter.Placeholder = gridOutputOption.HasFlag(GridOutputOptions.DotPlaceholder) ? '.' : '0';

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
		public static GridFormatter Create(string? format)
		{
			switch (format)
			{
				case null:
				case ".":
				{
					return new GridFormatter(false);
				}
				case "+":
				case ".+":
				case "+.":
				{
					return new GridFormatter(false) { WithModifiables = true };
				}
				case "0":
				{
					return new GridFormatter(false) { Placeholder = '0' };
				}
				case ":":
				{
					return new GridFormatter(false) { WithCandidates = true };
				}
				case "!":
				case ".!":
				case "!.":
				{
					return new GridFormatter(false) { WithModifiables = true };
				}
				case "0!":
				case "!0":
				{
					return new GridFormatter(false)
					{
						Placeholder = '0',
						WithModifiables = true
					};
				}
				case ".:":
				{
					return new GridFormatter(false) { WithCandidates = true };
				}
				case "0:":
				{
					return new GridFormatter(false)
					{
						Placeholder = '0',
						WithCandidates = true
					};
				}
				case "0+":
				case "+0":
				{
					return new GridFormatter(false)
					{
						Placeholder = '0',
						WithModifiables = true
					};
				}
				case "+:":
				case "+.:":
				case ".+:":
				{
					return new GridFormatter(false)
					{
						WithModifiables = true,
						WithCandidates = true
					};
				}
				case "0+:":
				case "+0:":
				{
					return new GridFormatter(false)
					{
						Placeholder = '0',
						WithModifiables = true,
						WithCandidates = true
					};
				}
				case ".!:":
				case "!.:":
				{
					return new GridFormatter(false) { WithModifiables = true };
				}
				case "0!:":
				case "!0:":
				{
					return new GridFormatter(false)
					{
						Placeholder = '0',
						WithModifiables = true
					};
				}
				case "#":
				case "#.":
				{
					// Formats representing 'intelligence processor' is equal to
					// format '.+:' and '0+:'.
					goto case ".+:";
				}
				case "#0":
				{
					goto case "0+:";
				}
				case "@":
				case "@.":
				{
					return new GridFormatter(true)
					{
						SubtleGridLines = true
					};
				}
				case "@0":
				{
					return new GridFormatter(true)
					{
						Placeholder = '0',
						SubtleGridLines = true
					};
				}
				case "@!":
				case "@.!":
				case "@!.":
				{
					return new GridFormatter(true)
					{
						TreatValueAsGiven = true,
						SubtleGridLines = true
					};
				}
				case "@0!":
				case "@!0":
				{
					return new GridFormatter(true)
					{
						Placeholder = '0',
						TreatValueAsGiven = true,
						SubtleGridLines = true
					};
				}
				case "@*":
				case "@.*":
				case "@*.":
				{
					return new GridFormatter(true);
				}
				case "@0*":
				case "@*0":
				{
					return new GridFormatter(true) { Placeholder = '0' };
				}
				case "@!*":
				case "@*!":
				{
					return new GridFormatter(true) { TreatValueAsGiven = true };
				}
				case "@:":
				{
					return new GridFormatter(true)
					{
						WithCandidates = true,
						SubtleGridLines = true
					};
				}
				case "@:!":
				case "@!:":
				{
					return new GridFormatter(true)
					{
						WithCandidates = true,
						TreatValueAsGiven = true,
						SubtleGridLines = true
					};
				}
				case "@*:":
				case "@:*":
				{
					return new GridFormatter(true) { WithCandidates = true };
				}
				case "@!*:":
				case "@*!:":
				case "@!:*":
				case "@*:!":
				case "@:!*":
				case "@:*!":
				{
					return new GridFormatter(true)
					{
						WithCandidates = true,
						TreatValueAsGiven = true
					};
				}
				default:
				{
					throw Throwing.FormatError;
				}
			}
		}
	}
}
