using System;
using System.Globalization;
using System.Windows.Data;

namespace Sudoku.Windows.Converters;

/// <summary>
/// Defines a width converter.
/// </summary>
public sealed class WidthConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		double actualWidth = (double)value;
		double widthScale = double.Parse((string)parameter);
		return actualWidth * widthScale is var result and >= 0 ? result : 0;
	}

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws.</exception>
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
		throw new NotImplementedException();
}
