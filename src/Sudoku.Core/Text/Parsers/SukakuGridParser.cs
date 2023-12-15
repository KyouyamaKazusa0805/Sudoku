using System.Text.RegularExpressions;
using Sudoku.Concepts;

namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a Sukaku grid parser.
/// </summary>
/// <param name="SingleLine">
/// Indicates whether the parser use single-line logic to parse the string text.
/// The single-line text uses 729 characters to describe all possible existences for all 9 digits in all 81 cells.
/// </param>
public sealed partial record SukakuGridParser(bool SingleLine = false) : ISpecifiedConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> str =>
		{
			if (SingleLine)
			{
				if (str.Length < 729)
				{
					return Grid.Undefined;
				}

				var result = Grid.Empty;
				for (var i = 0; i < 729; i++)
				{
					var c = str[i];
					if (c is not (>= '0' and <= '9' or '.'))
					{
						return Grid.Undefined;
					}

					if (c is '0' or '.')
					{
						result.SetExistence(i / 9, i % 9, false);
					}
				}

				return result;
			}
			else
			{
				var matches = from m in GridSukakuSegmentPattern().Matches(str) select m.Value;
				if (matches is { Length: not 81 })
				{
					return Grid.Undefined;
				}

				var result = Grid.Empty;
				for (var offset = 0; offset < 81; offset++)
				{
					var s = matches[offset].Reserve(@"\d");
					if (s.Length > 9)
					{
						// More than 9 characters.
						return Grid.Undefined;
					}

					var mask = (Mask)0;
					foreach (var c in s)
					{
						mask |= (Mask)(1 << c - '1');
					}

					if (mask == 0)
					{
						return Grid.Undefined;
					}

					for (var digit = 0; digit < 9; digit++)
					{
						result.SetExistence(offset, digit, (mask >> digit & 1) != 0);
					}
				}

				return result;
			}
		};


	[GeneratedRegex("""\d*[\-\+]?\d+""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSukakuSegmentPattern();
}
