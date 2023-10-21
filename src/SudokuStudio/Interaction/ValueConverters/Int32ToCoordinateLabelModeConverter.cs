using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Data;
using SudokuStudio.Rendering;

namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Defines a converter that converts an <see cref="Offset"/> value into a <see cref="CoordinateLabelDisplayMode"/> field.
/// </summary>
public sealed class Int32ToCoordinateLabelModeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language) => (CoordinateLabelDisplayMode)(Offset)value;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
