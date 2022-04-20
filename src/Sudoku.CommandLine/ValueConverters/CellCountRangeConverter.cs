namespace Sudoku.CommandLine.ValueConverters;

/// <summary>
/// Defines a value converter that converts a <see cref="string"/> into a range value.
/// </summary>
public sealed class CellCountRangeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object? Convert(string value)
	{
		Unsafe.SkipInit(out int min);
		Unsafe.SkipInit(out int max);
		if (!CellRange.TryParse(value, out var cellRange))
		{
			return null;
		}

		return (min, max) = cellRange;
	}
}
