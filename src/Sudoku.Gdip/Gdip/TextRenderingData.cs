namespace Sudoku.Gdip;

/// <summary>
/// The inner text rendering data.
/// </summary>
/// <param name="font">The font used.</param>
/// <param name="extraHeight">The extra height.</param>
/// <param name="stringFormat">The string format.</param>
internal sealed partial class TextRenderingData(Font font, float extraHeight, StringFormat stringFormat) : IDisposable
{
	/// <summary>
	/// Indicates the extra height.
	/// </summary>
	public float ExtraHeight { get; } = extraHeight;

	/// <summary>
	/// Indicates the font.
	/// </summary>
	public Font Font { get; } = font;

	/// <summary>
	/// Indicates the string format.
	/// </summary>
	public StringFormat StringFormat { get; } = stringFormat;


	[DeconstructionMethod]
	public partial void Deconstruct(out Font font, out float extraHeight, out StringFormat stringFormat);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		Font.Dispose();
		StringFormat.Dispose();
	}
}
