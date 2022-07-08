namespace Sudoku.UI.DataConversion.ValueConverters;

/// <summary>
/// Defines a value converter that converts an is-loading property to visibility.
/// </summary>
public sealed class IsLoadingToVisibilityConverter : IValueConverter
{
	/// <inheritdoc/>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="value"/> is not a <see cref="bool"/>.
	/// </exception>
	[return: NotNullIfNotNull(nameof(value))]
	public object? Convert(object? value, Type targetType, object parameter, string language)
		=> value switch
		{
			bool isLoading => isLoading switch
			{
				true => Visibility.Visible,
				false => Visibility.Collapsed
			},
			_ => throw new ArgumentException($"The argument '{nameof(value)}' must be of type boolean.", nameof(value))
		};

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException();
}
