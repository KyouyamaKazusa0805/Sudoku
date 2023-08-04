namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Defines a converter that converts an <see cref="int"/> value into a <see cref="CandidateViewNodeDisplayNode"/> field.
/// </summary>
public sealed class Int32ToCandidateViewNodeDisplayModeConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language) => (CandidateViewNodeDisplayNode)(int)value;

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
