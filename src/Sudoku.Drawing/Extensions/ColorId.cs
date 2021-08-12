namespace Sudoku.Drawing.Extensions;

/// <summary>
/// Extracts the extension methods for color ID.
/// </summary>
public static class ColorId
{
	/// <summary>
	/// Convert the current quadruple to a custom color ID.
	/// </summary>
	/// <param name="a">The alpha.</param>
	/// <param name="r">The red.</param>
	/// <param name="g">The green.</param>
	/// <param name="b">The blue.</param>
	/// <returns>The custom color ID.</returns>
	public static long ToCustomColorId(byte a, byte r, byte g, byte b) =>
		0xDEADL << 32 | (long)a << 24 | (long)r << 16 | (long)g << 8 | b;

	/// <summary>
	/// Check whether the color ID holds a custom color.
	/// </summary>
	/// <param name="this">The color ID.</param>
	/// <param name="a">The alpha.</param>
	/// <param name="r">The red.</param>
	/// <param name="g">The green.</param>
	/// <param name="b">The blue.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	/// <remarks>
	/// <see cref="DrawingInfo"/> objects holds key and value two properties, where
	/// <see cref="DrawingInfo.Id"/> is the key. Here ID can save a color value quadruple
	/// if the higher 32 bits are 0xDEAD. The last 32 bits are A, R, G, B value from one color.
	/// Otherwise the ID value is only between -4 and 10 (at present, who knows
	/// whether the range will be extended larger or not).
	/// </remarks>
	/// <seealso cref="DrawingInfo"/>
	/// <seealso cref="DrawingInfo.Id"/>
	public static bool IsCustomColorId(long @this, out byte a, out byte r, out byte g, out byte b)
	{
		if ((@this >> 32 & 65535) == 0xDEAD)
		{
			a = (byte)(@this >> 24 & 255);
			r = (byte)(@this >> 16 & 255);
			g = (byte)(@this >> 8 & 255);
			b = (byte)(@this & 255);
			return true;
		}
		else
		{
			a = r = g = b = default;
			return false;
		}
	}
}
