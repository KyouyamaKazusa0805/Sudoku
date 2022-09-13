namespace Sudoku.UI.DataConversion.ValueConverters;

/// <summary>
/// Defines a value converter that allows the one-way binding from item source property
/// to <see cref="Visibility"/> result.
/// </summary>
public sealed class ItemSourceNullabilityToVisibilityConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, string? language)
		=> value is null ? Visibility.Collapsed : Visibility.Visible;

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException();
}
