using System.Text.RegularExpressions;
using Sudoku.Concepts;

namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a table parser.
/// </summary>
public sealed partial record MultipleLineGridParser : IConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> static str =>
		{
			var matches = from match in GridSusserDigitPattern().Matches(str) select match.Value;
			if (matches.Length is not (var length and (81 or 85)))
			{
				// Subtle grid outline will bring 2 '.'s on first line of the grid.
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var i = 0; i < 81; i++)
			{
				switch (matches[length - 81 + i])
				{
					case [var match and not ('.' or '0')]:
					{
						result.SetDigit(i, match - '1');
						result.SetState(i, CellState.Given);
						break;
					}
					case { Length: 1 }:
					{
						continue;
					}
					case [_, var match]:
					{
						if (match is '.' or '0')
						{
							// '+0' or '+.'? Invalid combination.
							return Grid.Undefined;
						}

						result.SetDigit(i, match - '1');
						result.SetState(i, CellState.Modifiable);
						break;
					}
					default:
					{
						// The sub-match contains more than 2 characters or empty string,
						// which is invalid.
						return Grid.Undefined;
					}
				}
			}

			return result;
		};


	[GeneratedRegex("""(\+?\d|\.)""", RegexOptions.Compiled, 5000)]
	private static partial Regex GridSusserDigitPattern();
}
