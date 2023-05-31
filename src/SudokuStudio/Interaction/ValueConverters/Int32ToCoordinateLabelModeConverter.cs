﻿namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Defines a converter that converts an <see cref="int"/> value into a <see cref="CoordinateLabelDisplayMode"/> field.
/// </summary>
public sealed class Int32ToCoordinateLabelModeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language) => (CoordinateLabelDisplayMode)(int)value;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
