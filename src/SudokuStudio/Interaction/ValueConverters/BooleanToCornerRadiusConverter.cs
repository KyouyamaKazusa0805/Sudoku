namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Converts a <see cref="bool"/> value into a <see cref="double"/> value of corner radius.
/// </summary>
public sealed class BooleanToCornerRadiusConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language)
		=> value switch
		{
			true => new CornerRadius(12D),
			false => default,
			_ => throw new ArgumentOutOfRangeException(nameof(value))
		};

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotSupportedException();
}
