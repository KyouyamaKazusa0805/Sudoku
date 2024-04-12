namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents for a pencilmarking grid parser.
/// </summary>
/// <example>
/// For example:
/// <code><![CDATA[
/// +-------------------+-----------------+--------------------+
/// | <8>   12459  249  | *6*  <7>   259  | 1245    245   *3*  |
/// | 1457  1245   <6>  | 58   125   <3>  | 124578  2458  <9>  |
/// | 1579  <3>    279  | <4>  1259  2589 | 12578   *6*   78   |
/// +-------------------+-----------------+--------------------+
/// | *2*   <7>    *3*  | <1>  <8>   45   | 456     <9>   46   |
/// | <6>   458    48   | <2>  59    459  | <3>     *7*   *1*  |
/// | 1459  1459   49   | *3*  *6*   <7>  | 458     458   <2>  |
/// +-------------------+-----------------+--------------------+
/// | <3>   2489   <1>  | 578  25    <6>  | 24789   248   478  |
/// | 79    <6>    2789 | 78   <4>   *1*  | 289     *3*   <5>  |
/// | 47    248    <5>  | <9>  *3*   28   | 2678    <1>   4678 |
/// +-------------------+-----------------+--------------------+
/// ]]></code>
/// </example>
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
						// Compatibility:
						// If the cell has only one candidate left, we should treat this as given also.
						// This may ignore Sukaku checking, which causes a bug in logic.
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
