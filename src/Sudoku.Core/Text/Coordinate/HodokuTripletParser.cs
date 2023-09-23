using System.Diagnostics.CodeAnalysis;
using Sudoku.Analytics;
using Sudoku.Concepts;

namespace Sudoku.Text.Coordinate;

/// <summary>
/// Represents a parser type that uses <b>Hodoku Triplet</b> notation rule to parse text,
/// converting into a valid <see cref="ICoordinateObject"/> instance.
/// </summary>
/// <seealso cref="ICoordinateObject"/>
public sealed record HodokuTripletParser : CoordinateParser
{
#nullable disable
	/// <inheritdoc/>
	[Obsolete("This property is not implemented due to the lack of triplet notation rules.", true)]
	public override Func<string, CellMap> CellParser
		=> [DoesNotReturn] static (string _) => throw new NotSupportedException("Triplet notation does not support cell text output.");
#nullable restore

	/// <inheritdoc/>
	public override Func<string, CandidateMap> CandidateParser
		=> str =>
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

#nullable disable
	/// <inheritdoc/>
	[Obsolete("This property is not implemented due to the lack of triplet notation rules.", true)]
	public override Func<string, int> HouseParser
		=> [DoesNotReturn] static (string _) => throw new NotSupportedException("Triplet notation does not support house text output.");

	/// <inheritdoc/>
	[Obsolete("This property is not implemented due to the lack of triplet notation rules.", true)]
	public override Func<string, Conclusion[]> ConclusionParser
		=> [DoesNotReturn] static (string _) => throw new NotSupportedException("Triplet notation does not support conclusion text output.");

	/// <inheritdoc/>
	[Obsolete("This property is not implemented due to the lack of triplet notation rules.", true)]
	public override Func<string, short> DigitParser
		=> [DoesNotReturn] static (string _) => throw new NotSupportedException("Triplet notation does not support digit text output.");

	/// <inheritdoc/>
	[Obsolete("This property is not implemented due to the lack of triplet notation rules.", true)]
	public override Func<string, (IntersectionBase Base, IntersectionResult Result)[]> IntersectionParser
		=> [DoesNotReturn] static (string _) => throw new NotSupportedException("Triplet notation does not support intersection text output.");

	/// <inheritdoc/>
	[Obsolete("This property is not implemented due to the lack of triplet notation rules.", true)]
	public override Func<string, Chute[]> ChuteParser
		=> [DoesNotReturn] static (string _) => throw new NotSupportedException("Triplet notation does not support chute text output.");

	/// <inheritdoc/>
	[Obsolete("This property is not implemented due to the lack of triplet notation rules.", true)]
	public override Func<string, Conjugate[]> ConjuagteParser
		=> [DoesNotReturn] static (string _) => throw new NotSupportedException("Triplet notation does not support conjugate pair text output.");
#nullable restore
}
