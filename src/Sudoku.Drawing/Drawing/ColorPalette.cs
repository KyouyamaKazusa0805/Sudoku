namespace Sudoku.Drawing;

/// <summary>
/// Represents a color palette instance.
/// </summary>
public sealed class ColorPalette : List<Color>, ISliceMethod<ColorPalette, Color>
{
	/// <inheritdoc cref="ISliceMethod{TSelf, TSource}.Slice(int, int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new ColorPalette Slice(int start, int count) => [.. this[start..(start + count)]];

	/// <inheritdoc/>
	IEnumerable<Color> ISliceMethod<ColorPalette, Color>.Slice(int start, int count) => Slice(start, count);
}
