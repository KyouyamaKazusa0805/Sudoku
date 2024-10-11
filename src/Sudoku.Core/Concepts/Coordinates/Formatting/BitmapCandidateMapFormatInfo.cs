namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a <see cref="CandidateMapFormatInfo"/> type that supports bitmap formatting.
/// </summary>
public sealed partial class BitmapCandidateMapFormatInfo : CandidateMapFormatInfo
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(CandidateMapFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override BitmapCandidateMapFormatInfo Clone() => new();

	/// <inheritdoc/>
	protected internal override string FormatCore(ref readonly CandidateMap obj)
	{
		var result = (stackalloc char[729]);
		result.Fill('0');

		for (var cell = 0; cell < 729; cell++)
		{
			if (obj.Contains(cell))
			{
				result[cell] = '1';
			}
		}
		return result.ToString();
	}

	/// <inheritdoc/>
	protected internal override CandidateMap ParseCore(string str)
	{
		if (str.Length != 729)
		{
			throw new InvalidOperationException(string.Format(SR.ExceptionMessage("LengthMustBeMatched"), 729));
		}

		var result = CandidateMap.Empty;
		for (var cell = 0; cell < 729; cell++)
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

			throw new FormatException(SR.ExceptionMessage("StringValueInvalidToBeParsed"));
		}
		return result;
	}
}
