namespace Sudoku.UI.Data.MarkupExtensions;

/// <summary>
/// Defines a date-only markup extension.
/// </summary>
[MarkupExtensionReturnType(ReturnType = typeof(DateOnly?))]
public sealed class DateOnlyExtension : MarkupExtension
{
	/// <summary>
	/// Initializes a <see cref="DateOnlyExtension"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DateOnlyExtension() : base()
	{
	}


	/// <summary>
	/// Indicates the raw date text.
	/// </summary>
	public string? RawDateText { get; set; }

	/// <summary>
	/// Indicates the format.
	/// </summary>
	public string? Format { get; set; }


	/// <inheritdoc/>
	protected override object? ProvideValue()
		=> (RawDateText, Format) switch
		{
			(not null, null) when DateOnly.TryParse(RawDateText, out var result) => result,
			(not null, not null) when DateOnly.TryParseExact(RawDateText, Format, out var result) => result,
			_ => null,
		};
}
