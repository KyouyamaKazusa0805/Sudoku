using Sudoku.Concepts;
using Sudoku.Text.Coordinate;

namespace Sudoku.Text;

/// <summary>
/// Represents a parser type that uses <b>Hodoku Triplet</b> notation rule to parse text,
/// converting into a valid <see cref="ICoordinateObject"/> instance.
/// </summary>
/// <seealso cref="ICoordinateObject"/>
public sealed record HodokuTripletParser : ISpecifiedConceptParser<CandidateMap>
{
	/// <inheritdoc/>
	public Func<string, CandidateMap> Parser
		=> static str =>
		{
			var segments = str.Split((char[])[' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
			if (Array.IndexOf(segments, string.Empty) != -1)
			{
				throw new FormatException("The string contains empty segment.");
			}

			var result = CandidateMap.Empty;
			foreach (var segment in segments)
			{
				if (segment is [var d and >= '1' and <= '9', var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
				{
					result.Add(((r - '1') * 9 + c - '1') * 9 + d - '1');
					continue;
				}

				throw new FormatException("Each candidate segment contains invalid character.");
			}

			return result;
		};
}
