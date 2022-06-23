namespace Sudoku.UI.Configuration;

partial class Preference
{
	/// <summary>
	/// Gets the color at the specified index of the palette color list.
	/// </summary>
	/// <param name="paletteColorIndex">The index.</param>
	/// <returns>The color result.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the argument <paramref name="paletteColorIndex"/> is below 1 or above 10.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Color GetColor(int paletteColorIndex)
		=> paletteColorIndex switch
		{
			1 => PaletteColor1,
			2 => PaletteColor2,
			3 => PaletteColor3,
			4 => PaletteColor4,
			5 => PaletteColor5,
			6 => PaletteColor6,
			7 => PaletteColor7,
			8 => PaletteColor8,
			9 => PaletteColor9,
			10 => PaletteColor10,
			_ => throw new InvalidOperationException("The specified index is invalid. The valid range must be [1, 10].")
		};

	/// <summary>
	/// Gets the color index via the specified color.
	/// </summary>
	/// <param name="color">The color.</param>
	/// <returns>The index result. If failed to found, -1.</returns>
	public int GetPaletteColorIndex(Color color)
	{
		for (int i = 1; i <= 10; i++)
		{
			if (GetColor(i) == color)
			{
				return i;
			}
		}

		return -1;
	}
}
