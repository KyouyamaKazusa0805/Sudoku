using System;
using System.Extensions;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Sudoku.Solving.Manual;
using Sudoku.Windows.Extensions;

namespace Sudoku.Windows.Converters
{
	/// <summary>
	/// Defines a converter that converts from a difficulty information to the background color information.
	/// </summary>
	public sealed class DifficultyInfoToBackgroundColorConverter : IValueConverter
	{
		/// <inheritdoc/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var diffColors = ((WindowsSettings)parameter).DiffColors;
			var difficultyLevel = (DifficultyLevel)value;

			if (difficultyLevel == DifficultyLevel.Unknown)
			{
				return Brushes.White;
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
				? Brushes.White
				: i == 1
				? diffColors.TryGetValue(min, out var pair)
				? new SolidColorBrush(pair.Background.ToWColor())
				: Brushes.White
				: diffColors.TryGetValue(min, out var minPair) && diffColors.TryGetValue(max, out var maxPair)
				? new LinearGradientBrush(minPair.Background.ToWColor(), maxPair.Background.ToWColor(), 0)
				: Brushes.White;
		}

		/// <inheritdoc/>
		/// <exception cref="NotImplementedException">Always throws.</exception>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}
