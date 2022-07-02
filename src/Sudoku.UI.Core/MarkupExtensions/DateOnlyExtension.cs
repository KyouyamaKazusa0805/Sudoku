namespace Sudoku.UI.MarkupExtensions;

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
	[SetsRequiredMembers]
	public DateOnlyExtension() : base() => RawDateText = DateOnly.FromDateTime(DateTime.Now.Date).ToShortDateString();


	/// <summary>
	/// Indicates the raw date text.
	/// </summary>
	public required string RawDateText { get; set; }

	/// <summary>
	/// Indicates the format.
	/// </summary>
	public string? Format { get; set; }


	/// <inheritdoc/>
	protected override object? ProvideValue()
		=> Format switch
		{
			null when DateOnly.TryParse(RawDateText, out var result) => result,
			not null when DateOnly.TryParseExact(RawDateText, Format, out var result) => result,
			_ => null
		};
}
