namespace SudokuStudio.Markup;

/// <summary>
/// Defines a markup extension that can parse a <see cref="string"/> value, converting it into a valid <see cref="Grid"/>
/// or throwing exceptions when the code is invalid.
/// </summary>
[ContentProperty(Name = nameof(Text))]
[MarkupExtensionReturnType(ReturnType = typeof(Grid))]
public sealed class GridExtension : MarkupExtension
{
	/// <summary>
	/// Indicates whether the conversion ignores casing.
	/// </summary>
	public bool IgnoreCasing { get; set; } = false;

	/// <summary>
	/// Indicates the grid text that can be parsed as a valid <see cref="Grid"/> using <see cref="Grid.Parse(string)"/>.
	/// </summary>
	/// <seealso cref="Grid.Parse(string)"/>
	public string Text { get; set; } = string.Empty;

	/// <summary>
	/// Indicates the exact format string.
	/// </summary>
	public string ExactFormatString { get; set; } = string.Empty;


	/// <inheritdoc/>
	protected override object ProvideValue()
	{
		if (Text.Equals(nameof(Grid.Empty), IgnoreCasing ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
		{
			return Grid.Empty;
		}

		if (Text.Equals(nameof(Grid.Undefined), IgnoreCasing ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
		{
			return Grid.Undefined;
		}

		var targetGrid = Grid.Parse(Text);
		return !string.IsNullOrEmpty(ExactFormatString) && targetGrid.ToString(ExactFormatString) != Text
			? throw new FormatException(SR.ExceptionMessage("FormatInvalid"))
			: targetGrid;
	}
}
