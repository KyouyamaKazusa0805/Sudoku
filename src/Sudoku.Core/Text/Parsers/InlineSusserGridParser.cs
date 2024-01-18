namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a grid parser for linline Susser format.
/// </summary>
/// <param name="NegateEliminationsTripletRule">
/// <inheritdoc cref="SusserGridParser" path="/param[@name='NegateEliminationsTripletRule']"/>
/// </param>
public sealed partial record InlineSusserGridParser(bool NegateEliminationsTripletRule = false) : IConceptParser<Grid>
{
	/// <summary>
	/// Indicates the plus token that describes for modifiable values.
	/// </summary>
	private const string PlusToken = "+";

	/// <summary>
	/// Indicates the empty string.
	/// </summary>
	private const string EmptyString = "";


	/// <summary>
	/// Indicates the internal digits parser.
	/// </summary>
	private static readonly Func<string, Mask> DigitParser = new RxCyParser().DigitParser;


	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> str =>
		{
			var match = GridSusserPattern().Matches(str);
			if (match is not { Count: 81 } captures)
			{
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var cell = 0; cell < 81; cell++)
			{
				switch (captures[cell].Value)
				{
					case [.. var token, var digitChar and >= '1' and <= '9']:
					{
						var state = token switch { PlusToken => CellState.Modifiable, EmptyString => CellState.Given, _ => default };
						if (state is not (CellState.Given or CellState.Modifiable))
						{
							return Grid.Undefined;
						}

						var digit = digitChar - '1';
						result.SetDigit(cell, digit);
						result.SetState(cell, state);
						break;
					}
					case ['0' or '.']:
					{
						continue;
					}
					case ['[', .. var digitsStr, ']']:
					{
						var digits = DigitParser(digitsStr);
						if (!NegateEliminationsTripletRule)
						{
							// This applies for normal rule - removing candidates marked.
							foreach (var digit in digits)
							{
								// Set the candidate with false to eliminate the candidate.
								result.SetExistence(cell, digit, false);
							}
						}
						else
						{
							// If negate candidates, we should remove all possible candidates from all empty cells, making the grid invalid firstly.
							// Then we should add candidates onto the grid to make the grid valid.
							result[cell] = (Mask)(Grid.EmptyMask | digits);
						}
						break;
					}
					default:
					{
						return Grid.Undefined;
					}
				}
			}

			return result;
		};


	[GeneratedRegex("""(\+?[\d\.]|\[[1-9]{1,9}\])""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSusserPattern();
}
