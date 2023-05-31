namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Defines a converter that converts the value between <see cref="string"/> and <see cref="FontFamily"/>.
/// </summary>
public sealed class StringToFontFamilyConverter : IValueConverter
{
	/// <inheritdoc/>
	public object? Convert(object? value, Type? targetType, object? parameter, string? language)
		=> value is string s ? new FontFamily(s) : null;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object? ConvertBack(object? value, Type? targetType, object? parameter, string? language) => throw new NotImplementedException();
}
