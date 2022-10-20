namespace Sudoku.Gdip;

/// <summary>
/// The inner text rendering data.
/// </summary>
/// <param name="Font">The font used.</param>
/// <param name="ExtraHeight">The extra height.</param>
/// <param name="StringFormat">The string format.</param>
internal sealed record TextRenderingData(Font Font, float ExtraHeight, StringFormat StringFormat) : IDisposable
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	public void Dispose()
	{
		Font.Dispose();
		StringFormat.Dispose();
	}
}
