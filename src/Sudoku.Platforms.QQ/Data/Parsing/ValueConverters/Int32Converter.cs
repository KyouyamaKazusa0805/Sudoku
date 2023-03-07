namespace Sudoku.Platforms.QQ.Data.Parsing.ValueConverters;

/// <summary>
/// Defines a converter that converts a <see cref="string"/> value into <see cref="int"/> value.
/// </summary>
public sealed class Int32Converter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(string value)
	{
		if (!int.TryParse(value, out var result))
		{
			throw new CommandConverterException();
		}

		return result;
	}
}
