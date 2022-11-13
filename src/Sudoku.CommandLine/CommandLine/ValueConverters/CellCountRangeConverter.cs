namespace Sudoku.CommandLine.ValueConverters;

/// <summary>
/// Defines a value converter that converts a <see cref="string"/> into a range value.
/// </summary>
public sealed class CellCountRangeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(string value)
	{
		if (CellRange.TryParse(value, out var cellRange))
		{
			var (min, max) = cellRange;
			return (min, max);
		}

		throw new CommandConverterException("The text value cannot be parsed as a valid range.");
	}
}
