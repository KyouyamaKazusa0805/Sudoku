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


	/// <inheritdoc/>
	public Func<string, Grid> Parser
		=> str =>
		{
			var match = GridSusserPattern().Match(str);
			if (match is not { Success: true, Captures: { Count: 81 } captures })
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
						var candidates = new HodokuTripletParser().Parser(digitsStr);
						if (!NegateEliminationsTripletRule)
						{
							// This applies for normal rule - removing candidates marked.
							foreach (var candidate in candidates)
							{
								// Set the candidate with false to eliminate the candidate.
								result.SetExistence(candidate / 9, candidate % 9, false);
							}
						}
						else
						{
							// If negate candidates, we should remove all possible candidates from all empty cells, making the grid invalid firstly.
							// Then we should add candidates onto the grid to make the grid valid.
							result[cell] = (Mask)(Grid.EmptyMask | candidates.GetDigitsFor(cell));
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


	[GeneratedRegex("""(\+?[\d\.]|\[[1-9]{1,9}\]){81}""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridSusserPattern();
}
