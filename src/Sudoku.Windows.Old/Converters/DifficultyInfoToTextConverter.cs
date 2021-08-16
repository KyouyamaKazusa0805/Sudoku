namespace Sudoku.Windows.Converters;

/// <summary>
/// Defines a converter that converts from a difficulty information to the text.
/// </summary>
public sealed class DifficultyInfoToTextConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var difficultyLevel = (DifficultyLevel)value;
		if (difficultyLevel == DifficultyLevel.Unknown)
		{
			return string.Empty;
		}

		DifficultyLevel min = default, max = default;
		int i = 0;
		foreach (var pos in difficultyLevel)
		{
			switch (i++)
			{
				case 0: min = pos; break;
				case 1: max = pos; break;
				default: goto Returning;
			}
		}

	Returning:
		return min == DifficultyLevel.Unknown ? string.Empty : i == 1 ? min.ToString() : $"{min} - {max}";
	}

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws.</exception>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
		throw new NotImplementedException();
}
