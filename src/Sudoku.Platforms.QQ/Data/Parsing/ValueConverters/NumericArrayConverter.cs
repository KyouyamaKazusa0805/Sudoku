namespace Sudoku.Platforms.QQ.Data.Parsing.ValueConverters;

/// <summary>
/// Defines a converter that converts a <see cref="string"/> value into <typeparamref name="T"/>[] value.
/// </summary>
/// <typeparam name="T">The type of the parsable target value.</typeparam>
public sealed class NumericArrayConverter<T> : IValueConverter where T : unmanaged, INumber<T>
{
	/// <inheritdoc/>
	public object Convert(string value)
	{
		var split = value.Split(new[] { ',', '，', '、' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		var result = new T[split.Length];
		for (var i = 0; i < split.Length; i++)
		{
			var element = split[i];
			if (!T.TryParse(element, null, out var target))
			{
				throw new CommandConverterException();
			}

			result[i] = target;
		}

		return result;
	}
}
