namespace Sudoku.CommandLine.ValueConverters;

/// <summary>
/// Defines a converter that can convert a <see cref="string"/> value into the target result.
/// </summary>
public sealed class NumericConverter<TNumber> : IValueConverter where TNumber : unmanaged, INumber<TNumber>
{
	/// <inheritdoc/>
	public object Convert(string value)
	{
		var type = typeof(TNumber);
		try
		{
			if (type == typeof(sbyte)) { return sbyte.Parse(value); }
			else if (type == typeof(byte)) { return byte.Parse(value); }
			else if (type == typeof(ushort)) { return ushort.Parse(value); }
			else if (type == typeof(short)) { return short.Parse(value); }
			else if (type == typeof(int)) { return int.Parse(value); }
			else if (type == typeof(uint)) { return uint.Parse(value); }
			else if (type == typeof(long)) { return long.Parse(value); }
			else if (type == typeof(ulong)) { return ulong.Parse(value); }
			else if (type == typeof(float)) { return float.Parse(value); }
			else if (type == typeof(double)) { return double.Parse(value); }
			else if (type == typeof(decimal)) { return decimal.Parse(value); }
			else { throw new NotSupportedException("The specified format is not supported."); }
		}
		catch when (type.Name is var name)
		{
			throw new CommandConverterException($"The specified text cannot be parsed as a valid number of type '{name}'.");
		}
	}
}
