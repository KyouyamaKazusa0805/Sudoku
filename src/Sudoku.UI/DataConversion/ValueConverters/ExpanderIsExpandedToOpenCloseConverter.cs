namespace Sudoku.UI.DataConversion.ValueConverters;

/// <summary>
/// Defines a value converter that allows the one-way binding from the expanded property
/// to a <see cref="string"/> value indicating the open close case.
/// </summary>
public sealed class ExpanderIsExpandedToOpenCloseConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, string? language)
		=> value is bool isExpanded && isExpanded ? R["ExpanderCloseCase"]! : R["ExpanderOpenCase"]!;

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException();
}
