namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents a parser type that uses <b>Hodoku Triplet</b> notation rule to parse text,
/// converting into a valid <see cref="ISudokuConcept{TSelf}"/> instance.
/// </summary>
/// <seealso cref="ISudokuConcept{TSelf}"/>
public sealed record HodokuTripletParser : IConceptParser<CandidateMap>
{
	/// <inheritdoc/>
	public Func<string, CandidateMap> Parser
		=> static str =>
		{
			var segments = str.SplitBy(' ');
			if (Array.IndexOf(segments, string.Empty) != -1)
			{
				throw new FormatException(ResourceDictionary.ExceptionMessage("ContainsEmptySegmentOnParsing"));
			}

			var result = CandidateMap.Empty;
			foreach (var segment in segments)
			{
				if (segment is [var d and >= '1' and <= '9', var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
				{
					result.Add(((r - '1') * 9 + c - '1') * 9 + d - '1');
					continue;
				}

				throw new FormatException(ResourceDictionary.ExceptionMessage("StringValueInvalidToBeParsed"));
			}

			return result;
		};
}
