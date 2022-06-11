namespace Sudoku.UI.Drawing;

/// <summary>
/// Provides with the color-related marshalling methods.
/// </summary>
public static class ColorMarshal
{
	/// <summary>
	/// Converts an <see cref="Identifier"/> instance into a <see cref="Color"/> instance.
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> value.</param>
	/// <returns>The <see cref="Color"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color AsColor(this Identifier identifier)
		=> identifier switch
		{
			{ UseId: true, Id: _ } => throw new NotImplementedException(),
			{ UseId: false, A: var a, R: var r, G: var g, B: var b } => Color.FromArgb(a, r, g, b)
		};
}
