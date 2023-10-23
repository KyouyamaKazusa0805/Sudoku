using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Data;
using Sudoku.Concepts;

namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Defines a converter that converts an <see cref="int"/> value into a <see cref="CoordinateType"/> field.
/// </summary>
public sealed class Int32ToCoordinateLabelKindConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language) => (CoordinateType)(int)value;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
