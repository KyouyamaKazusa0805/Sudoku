namespace Sudoku.UI.DataConversion.ValueConverters;

/// <summary>
/// Defines a value converter that allows the one-way binding from <see cref="CollectionViewSource.Source"/>
/// property to <see cref="Control.IsEnabled"/> property.
/// </summary>
/// <seealso cref="CollectionViewSource.Source"/>
/// <seealso cref="Control.IsEnabled"/>
public sealed class SourceToIsEnabledConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object parameter, string language) => value is not null;

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException();
}
