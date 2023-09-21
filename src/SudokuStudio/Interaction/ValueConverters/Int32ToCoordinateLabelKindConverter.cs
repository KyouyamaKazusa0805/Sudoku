using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Data;
using Sudoku.Text.Coordinate;

namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Defines a converter that converts an <see cref="int"/> value into a <see cref="ConceptNotationBased"/> field.
/// </summary>
public sealed class Int32ToCoordinateLabelKindConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language) => (ConceptNotationBased)(int)value;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
