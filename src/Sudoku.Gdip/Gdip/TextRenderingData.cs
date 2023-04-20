namespace Sudoku.Gdip;

/// <summary>
/// The inner text rendering data.
/// </summary>
/// <param name="font">Indicates the font.</param>
/// <param name="extraHeight">Indicates the extra height.</param>
/// <param name="stringFormat">Indicates the string format.</param>
internal sealed partial class TextRenderingData(
	[PrimaryConstructorParameter] Font font,
	[PrimaryConstructorParameter] float extraHeight,
	[PrimaryConstructorParameter] StringFormat stringFormat
) : IDisposable
{
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
