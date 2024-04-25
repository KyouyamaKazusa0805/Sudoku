namespace Sudoku.Gdip;

/// <summary>
/// The inner text rendering data.
/// </summary>
/// <param name="font">Indicates the font.</param>
/// <param name="extraHeight">Indicates the extra height.</param>
/// <param name="stringFormat">Indicates the string format.</param>
internal sealed class TextRenderingData(Font font, float extraHeight, StringFormat stringFormat) : IDisposable
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Font f, out float e, out StringFormat s) => (f, e, s) = (font, extraHeight, stringFormat);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		font.Dispose();
		stringFormat.Dispose();
	}
}
