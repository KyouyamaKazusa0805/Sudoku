namespace Sudoku.Gdip;

/// <summary>
/// The inner text rendering data.
/// </summary>
/// <param name="font">Indicates the font.</param>
/// <param name="extraHeight">Indicates the extra height.</param>
/// <param name="stringFormat">Indicates the string format.</param>
internal sealed partial class TextRenderingData(
	[Data] Font font,
	[Data] float extraHeight,
	[Data] StringFormat stringFormat
) : IDisposable
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Font font, out float extraHeight, out StringFormat stringFormat)
		=> (font, extraHeight, stringFormat) = (Font, ExtraHeight, StringFormat);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		Font.Dispose();
		StringFormat.Dispose();
	}
}
