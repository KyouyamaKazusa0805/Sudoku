using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Sudoku.Concepts;

namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents an Open-Sudoku format parser.
/// </summary>
public sealed partial record OpenSudokuGridParser : IConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> static str =>
		{
			if (GridOpenSudokuPattern().Match(str) is not { Success: true, Value: var match })
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var i = 0; i < 81; i++)
			{
				switch (match[i * 6])
				{
					case '0' when whenClause(i * 6, match, "|0|1", "|0|1|"):
					{
						continue;
					}
					case not '0' and var ch when whenClause(i * 6, match, "|0|0", "|0|0|"):
					{
						result.SetDigit(i, ch - '1');
						result.SetState(i, CellState.Given);

						break;
					}
					default:
					{
						// Invalid string state.
						return Grid.Undefined;
					}
				}
			}

			return result;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static bool whenClause(Cell i, string match, string pattern1, string pattern2)
				=> i == 80 * 6 ? match[(i + 1)..(i + 5)] == pattern1 : match[(i + 1)..(i + 6)] == pattern2;
		};


	[GeneratedRegex("""\d(\|\d){242}""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridOpenSudokuPattern();
}
