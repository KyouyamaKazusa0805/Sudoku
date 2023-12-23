namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Represents a value converter that can convert a <see cref="LibraryPuzzleTransformKinds"/> to a <see cref="bool"/> value.
/// </summary>
internal sealed class LibraryPuzzleTransformKindsToBooleanConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language)
		=> (value, parameter) is (LibraryPuzzleTransformKinds items, string rawFlag)
		&& Enum.TryParse(rawFlag, out LibraryPuzzleTransformKinds flag) && items.Flags(flag);

	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">Throws when <paramref name="parameter"/> or <paramref name="value"/> is invalid.</exception>
	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		const string error_Value = $"The target value '{nameof(value)}' is invalid - it must be a boolean value (true or false).";
		const string error_Parameter = $"The target value '{nameof(parameter)}' is invalid - it must be the string representation a field in type '{nameof(StepTooltipDisplayItems)}'.";

		return (((App)Application.Current).Preference.LibraryPreferences.LibraryPuzzleTransformations, parameter) switch
		{
			(var items, string rawFlag) when Enum.TryParse(rawFlag, out LibraryPuzzleTransformKinds flag) => value switch
			{
				true => items | flag,
				false => items & ~flag,
				_ => throw new InvalidOperationException(error_Value)
			},
			_ => throw new InvalidOperationException(error_Parameter)
		};
	}
}
