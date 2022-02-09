using Microsoft.UI.Xaml.Data;
using Sudoku.Diagnostics.CodeAnalysis;

namespace Sudoku.UI.Data.Converters;

/// <summary>
/// Defines a converter that allows the conversions between
/// sudoku digit (between 0 and 9, and 0 is for the empty cell) and a <see cref="string"/> value.
/// </summary>
public sealed class SudokuDigitConverter : IValueConverter
{
	/// <inheritdoc/>
	public object? Convert(
		object? value, Type targetType, [IsDiscard] object? parameter, [IsDiscard] string? language) =>
		targetType == typeof(int) && value is string str && int.TryParse(str, out int result)
			? result
			: null;

	/// <inheritdoc/>
	public object? ConvertBack(
		object value, [IsDiscard] Type targetType, [IsDiscard] object parameter, [IsDiscard] string language) =>
		value is int i and not 0 ? i.ToString() : string.Empty;
}
