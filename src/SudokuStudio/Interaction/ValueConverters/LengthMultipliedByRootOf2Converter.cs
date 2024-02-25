namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Indicates a value converter that creates a new value, multiplied by root of 2.
/// </summary>
public sealed class LengthMultipliedByRootOf2Converter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type? targetType, object? parameter, string? language)
		=> value is double d ? d * Sqrt(2) : throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("InvalidBaseType"));

	/// <inheritdoc/>
	[DoesNotReturn]
	public object? ConvertBack(object? value, Type? targetType, object? parameter, string? language) => throw new NotImplementedException();
}
