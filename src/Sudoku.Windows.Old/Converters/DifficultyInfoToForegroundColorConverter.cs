namespace Sudoku.Windows.Converters;

/// <summary>
/// Defines a converter that converts from a difficulty information to the foreground color information.
/// </summary>
public sealed class DifficultyInfoToForegroundColorConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		var diffColors = WColorPalette.DifficultyLevelColors;
		var difficultyLevel = (DifficultyLevel)value;

		if (difficultyLevel == DifficultyLevel.Unknown)
		{
			return WBrushes.White;
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
		return min == DifficultyLevel.Unknown
			? WBrushes.White
			: i == 1
				? diffColors.TryGetValue(min, out var pair)
					? new WSolidColorBrush(pair.Foreground.ToWColor())
					: WBrushes.White
				: diffColors.TryGetValue(min, out var minPair) && diffColors.TryGetValue(max, out var maxPair)
					? new WLinearGradientBrush(minPair.Foreground.ToWColor(), maxPair.Foreground.ToWColor(), 0)
					: WBrushes.White;
	}

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws.</exception>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
		throw new NotImplementedException();
}
