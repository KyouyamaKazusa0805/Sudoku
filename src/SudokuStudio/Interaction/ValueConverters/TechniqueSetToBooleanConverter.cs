using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Data;
using Sudoku.Analytics.Categorization;

namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Defines a value converter that converts the value from <see cref="TechniqueSet"/> to a <see cref="bool"/> value, or vice versa.
/// </summary>
/// <seealso cref="TechniqueSet"/>
internal sealed class TechniqueSetToBooleanConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language)
		=> value is TechniqueSet set && parameter is Technique technique
			? (bool?)set.Contains(technique)
			: throw new ArgumentException("The target value is invalid.");

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}
