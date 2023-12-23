namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents for a pencilmarking grid parser.
/// </summary>
public sealed partial record PencilmarkingGridParser : IConceptParser<Grid>
{
	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> static str =>
		{
			// Older regular expression pattern:
			if ((from m in GridPencilmarkingPattern().Matches(str) select m.Value) is not { Length: 81 } matches)
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var cell = 0; cell < 81; cell++)
			{
				if (matches[cell] is not { Length: var length and <= 9 } s)
				{
					// More than 9 characters.
					return Grid.Undefined;
				}

				if (s.Contains('<'))
				{
					// All values will be treated as normal characters: '<digit>', '*digit*' and 'candidates'.

					// Givens.
					if (length == 3)
					{
						if (s[1] is var c and >= '1' and <= '9')
						{
							result.SetDigit(cell, c - '1');
							result.SetState(cell, CellState.Given);
						}
						else
						{
							// Illegal characters found.
							return Grid.Undefined;
						}
					}
					else
					{
						// The length is not 3.
						return Grid.Undefined;
					}
				}
				else if (s.Contains('*'))
				{
					// Modifiables.
					if (length == 3)
					{
						if (s[1] is var c and >= '1' and <= '9')
						{
							result.SetDigit(cell, c - '1');
							result.SetState(cell, CellState.Modifiable);
						}
						else
						{
							// Illegal characters found.
							return Grid.Undefined;
						}
					}
					else
					{
						// The length is not 3.
						return Grid.Undefined;
					}
				}
				else if (s.SatisfyPattern("""[1-9\+\-]{1,9}"""))
				{
					// Candidates.
					// Here don't need to check the length of the string, and also all characters are digit characters.
					var mask = (Mask)0;
					foreach (var c in s)
					{
						if (c is not ('+' or '-'))
						{
							mask |= (Mask)(1 << c - '1');
						}
					}

					if (mask == 0)
					{
						return Grid.Undefined;
					}

					if ((mask & mask - 1) == 0)
					{
						result.SetDigit(cell, TrailingZeroCount(mask));
						result.SetState(cell, CellState.Given);
					}
					else
					{
						for (var digit = 0; digit < 9; digit++)
						{
							result.SetExistence(cell, digit, (mask >> digit & 1) != 0);
						}
					}
				}
				else
				{
					// All conditions can't match.
					return Grid.Undefined;
				}
			}

			return result;
		};


	[GeneratedRegex("""(\<\d\>|\*\d\*|\d*[\+\-]?\d+)""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridPencilmarkingPattern();
}
