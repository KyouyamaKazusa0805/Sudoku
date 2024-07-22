namespace Sudoku.Concepts.Formatting;

/// <summary>
/// Represents a <see cref="CellMapFormatInfo"/> type that supports bitmap formatting.
/// </summary>
public sealed class BitmapCellMapFormatInfo : CellMapFormatInfo
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(CellMapFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] CellMapFormatInfo? other) => other is BitmapCellMapFormatInfo;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(typeof(BitmapCellMapFormatInfo));

	/// <inheritdoc/>
	public override BitmapCellMapFormatInfo Clone() => new();

	/// <inheritdoc/>
	protected internal override string FormatMap(ref readonly CellMap map)
	{
		var result = (stackalloc char[81]);
		result.Fill('0');

		for (var cell = 0; cell < 81; cell++)
		{
			if (map.Contains(cell))
			{
				result[cell] = '1';
			}
		}
		return result.ToString();
	}

	/// <inheritdoc/>
	protected internal override CellMap ParseMap(string str)
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
