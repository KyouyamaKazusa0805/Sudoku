namespace Sudoku.UI.Data.ValueConverters;

/// <summary>
/// Defines a converter that can convert an <see cref="int"/> value to a <see cref="string"/>.
/// </summary>
public sealed class Int32ToStringConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object? parameter, string language)
		=> (value, parameter) switch
		{
			(double d, string s) => d.ToString(s),
			(double d, _) => d.ToString(),
			_ => string.Empty
		};

	/// <inheritdoc/>
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException();
}
