namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Represents a type that can convert values from <see cref="double"/> to <see cref="decimal"/>, or vice versa.
/// </summary>
public sealed class DoubleToDecimalConverter : IValueConverter
{
	/// <summary>
	/// Indicates whether the argument <c>value</c> from method <see cref="Convert(object?, Type?, object?, string?)"/>
	/// is a <see cref="decimal"/>. The default value is <see langword="true"/>.
	/// </summary>
	/// <seealso cref="Convert(object?, Type?, object?, string?)"/>
	public bool IsConvertFromDecimal { get; set; } = true;


	/// <inheritdoc/>
	public object Convert(object? value, Type? targetType, object? parameter, string? language)
		=> (IsConvertFromDecimal, value) switch
		{
			(true, decimal d) => (double)d,
			(false, double d) => (decimal)Round(d, 2),
			_ => 0
		};

	/// <inheritdoc/>
	public object ConvertBack(object? value, Type? targetType, object? parameter, string? language)
		=> (IsConvertFromDecimal, value) switch
		{
			(true, double d) => (decimal)Round(d, 2),
			(false, decimal d) => (double)d,
			_ => 0
		};
}
