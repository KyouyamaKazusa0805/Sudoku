namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a <see cref="CandidateMapFormatInfo"/> type that supports Hodoku elimination candidates formatting.
/// </summary>
public sealed class HodokuTripletCandidateMapFormatInfo : CandidateMapFormatInfo
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(CandidateMapFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override HodokuTripletCandidateMapFormatInfo Clone() => new();

	/// <inheritdoc/>
	protected internal override string FormatCore(ref readonly CandidateMap obj)
	{
		return obj switch { [] => string.Empty, [var p] => $"{p % 9 + 1}{p / 9 / 9 + 1}{p / 9 % 9 + 1}", _ => f(in obj) };


		static string f(ref readonly CandidateMap map)
		{
			var sb = new StringBuilder();
			foreach (var candidate in map)
			{
				var (cell, digit) = (candidate / 9, candidate % 9);
				sb.Append($"{digit + 1}{cell / 9 + 1}{cell % 9 + 1} ");
			}

			return sb.RemoveFrom(^1).ToString();
		}
	}

	/// <inheritdoc/>
	protected internal override CandidateMap ParseCore(string str)
	{
		var segments = str.SplitBy(' ');
		if (Array.IndexOf(segments, string.Empty) != -1)
		{
			throw new FormatException(SR.ExceptionMessage("ContainsEmptySegmentOnParsing"));
		}

		var result = CandidateMap.Empty;
		foreach (var segment in segments)
		{
			if (segment is [var d and >= '1' and <= '9', var r and >= '1' and <= '9', var c and >= '1' and <= '9'])
			{
				result.Add(((r - '1') * 9 + c - '1') * 9 + d - '1');
				continue;
			}

			throw new FormatException(SR.ExceptionMessage("StringValueInvalidToBeParsed"));
		}
		return result;
	}
}
