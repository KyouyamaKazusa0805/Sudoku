namespace Sudoku.UI.Drawing.Xaml.Converters;

/// <summary>
/// Provides a type converter that can converts from a <see cref="string"/> into a <see cref="Size"/> instance.
/// </summary>
/// <seealso cref="Size"/>
public sealed class String2SizeConverter : TypeConverter
{
	/// <inheritdoc/>
	public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
		sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);

	/// <inheritdoc/>
	public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
		!(
			value is string s && s.Contains(',')
			&& s.Split(',', StringSplitOptions.RemoveEmptyEntries) is { Length: 2 } split
			&& double.TryParse(split[0], out double width) && double.TryParse(split[1], out double height)
		) ? base.ConvertFrom(context, culture, value) : new Point(width, height);
}
