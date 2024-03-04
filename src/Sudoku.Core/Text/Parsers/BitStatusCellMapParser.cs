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
				throw new InvalidOperationException(string.Format(ResourceDictionary.ExceptionMessage("LengthMustBeMatched"), 81));
			}

			var result = (CellMap)[];
			for (var cell = 0; cell < 81; cell++)
			{
				var character = str[cell];
				if (character is '.' or '0')
				{
					continue;
				}

				if (str[cell] - '0' == 1)
				{
					result.Add(cell);
					continue;
				}

				throw new FormatException(ResourceDictionary.ExceptionMessage("StringValueInvalidToBeParsed"));
			}

			return result;
		};
}
