namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents a parser that uses bit operation rule to parse <see cref="CellMap"/> instances.
/// </summary>
/// <seealso cref="CellMap"/>
public sealed class BitStatusCellMapParser : IConceptParser<CellMap>
{
	/// <inheritdoc/>
	public Func<string, CellMap> Parser
		=> static str =>
		{
			if (str.Length != 81)
			{
				throw new InvalidOperationException("The length of the string must be 81.");
			}

			var result = CellMap.Empty;
			for (var cell = 0; cell < 81; cell++)
			{
				switch (str[cell] - '0')
				{
					case 1:
					{
						result.Add(cell);
						break;
					}
					case not 0:
					{
						throw new FormatException("The specified format contains invalid characters.");
					}
				}
			}

			return result;
		};
}
