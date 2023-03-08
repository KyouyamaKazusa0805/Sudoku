namespace Sudoku.Platforms.QQ.Data.Parsing.ValueConverters;

/// <summary>
/// Defines a converter that converts a <see cref="string"/> value into <typeparamref name="T"/> value.
/// </summary>
/// <typeparam name="T">The type of the parsable target value.</typeparam>
public sealed class NumericConverter<T> : IValueConverter where T : unmanaged, INumber<T>
{
	/// <inheritdoc/>
	public object Convert(string value)
	{
		if (!T.TryParse(value, null, out var result))
		{
			throw new CommandConverterException();
		}

		return result;
	}
}
