namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Converts a <see cref="bool"/> value into a <see cref="Visibility"/> result.
/// </summary>
/// <seealso cref="Visibility"/>
public sealed class BooleanToVisibilityConverter : IValueConverter
{
	/// <summary>
	/// Indicates whether the converter logic is reverted.
	/// </summary>
	public bool IsReverted { get; set; }


	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language)
		=> (value, IsReverted) switch
		{
			(true, true) => Visibility.Collapsed,
			(true, false) => Visibility.Visible,
			(false, true) => Visibility.Visible,
			(false, false) => Visibility.Collapsed,
			_ => throw new InvalidOperationException($"The argument '{nameof(value)}' has a wrong type - it must be a boolean value.")
		};

	/// <inheritdoc/>
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> (value, IsReverted) switch
		{
			(Visibility.Visible, true) => false,
			(Visibility.Collapsed, true) => true,
			(Visibility.Visible, false) => true,
			(Visibility.Collapsed, false) => false,
			_ => throw new InvalidOperationException($"The argument '{nameof(value)}' has a wrong type - it must be a value of type '{nameof(Visibility)}'.")
		};
}
