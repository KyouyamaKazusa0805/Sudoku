using System;
using System.Globalization;
using System.Windows.Data;
using Sudoku.Solving.Manual;

namespace Sudoku.Windows.Data
{
	/// <summary>
	/// Defines a converter that converts from a difficulty information to the text.
	/// </summary>
	public sealed class DifficultyInfoToTextConverter : IValueConverter
	{
		/// <inheritdoc/>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var difficultyLevel = (DifficultyLevel)value;
			return difficultyLevel == DifficultyLevel.Unknown ? string.Empty : difficultyLevel.ToString();
		}

		/// <inheritdoc/>
		/// <exception cref="NotImplementedException">Always throws.</exception>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
			throw new NotImplementedException();
	}
}
