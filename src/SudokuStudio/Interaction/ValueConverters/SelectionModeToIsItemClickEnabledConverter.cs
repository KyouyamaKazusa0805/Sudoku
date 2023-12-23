namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Represents a value converter that can convert the value from <see cref="SelectionMode"/> to <see cref="bool"/>
/// indicating whether the item of type <see cref="ListViewItem"/> can be selected.
/// </summary>
/// <seealso cref="SelectionMode"/>
/// <seealso cref="ListViewItem"/>
public sealed class SelectionModeToIsItemClickEnabledConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language)
		=> value is >= ListViewSelectionMode.Single and <= ListViewSelectionMode.Extended;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
