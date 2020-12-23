using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Sudoku.Drawing.Extensions;
using Sudoku.Solving.Manual;

namespace Sudoku.Windows.Data
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

			return new SolidColorBrush(
				diffColors.TryGetValue(difficultyLevel, out var pair)
				? pair.Background.ToWColor()
				: Colors.White);
		}

		/// <inheritdoc/>
		/// <exception cref="NotImplementedException">Always throws.</exception>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}
