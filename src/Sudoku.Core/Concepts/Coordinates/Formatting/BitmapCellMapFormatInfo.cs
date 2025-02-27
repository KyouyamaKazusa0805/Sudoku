namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a <see cref="CellMapFormatInfo"/> type that supports bitmap formatting.
/// </summary>
public sealed class BitmapCellMapFormatInfo : CellMapFormatInfo
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override IFormatProvider? GetFormat(Type? formatType) => formatType == typeof(CellMapFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override BitmapCellMapFormatInfo Clone() => new();

	/// <inheritdoc/>
	protected internal override string FormatCore(in CellMap obj)
	{
		var result = (stackalloc char[81]);
		result.Fill('0');

		for (var cell = 0; cell < 81; cell++)
		{
			if (obj.Contains(cell))
			{
				result[cell] = '1';
			}
		}
		return result.ToString();
	}

	/// <inheritdoc/>
	protected internal override CellMap ParseCore(string str)
	{
		if (str.Length != 81)
		{
			throw new InvalidOperationException(string.Format(SR.ExceptionMessage("LengthMustBeMatched"), 81));
		}

		var result = CellMap.Empty;
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

			throw new FormatException(SR.ExceptionMessage("StringValueInvalidToBeParsed"));
		}
		return result;
	}
}
