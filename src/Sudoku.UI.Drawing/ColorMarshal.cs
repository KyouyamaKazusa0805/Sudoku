namespace Sudoku.UI.Drawing;

/// <summary>
/// Provides with the color-related marshalling methods.
/// </summary>
internal static class ColorMarshal
{
	/// <summary>
	/// Converts an <see cref="Identifier"/> instance into a <see cref="Color"/> instance.
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> value.</param>
	/// <param name="userPreference">The user preference instance.</param>
	/// <returns>The <see cref="Color"/> result.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the property <see cref="Identifier.Mode"/>
	/// from the argument <paramref name="identifier"/> is not defined.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified ID or named kind value is invalid.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color AsColor(this Identifier identifier, IDrawingPreference userPreference)
		=> identifier switch
		{
			{ Mode: IdentifierColorMode.Raw, A: var a, R: var r, G: var g, B: var b } => Color.FromArgb(a, r, g, b),
			{ Mode: IdentifierColorMode.Id, Id: var id } => id switch
			{
				1 => userPreference.PaletteColor1,
				2 => userPreference.PaletteColor2,
				3 => userPreference.PaletteColor3,
				4 => userPreference.PaletteColor4,
				5 => userPreference.PaletteColor5,
				6 => userPreference.PaletteColor6,
				7 => userPreference.PaletteColor7,
				8 => userPreference.PaletteColor8,
				9 => userPreference.PaletteColor9,
				10 => userPreference.PaletteColor10,
				11 => userPreference.PaletteColor11,
				12 => userPreference.PaletteColor12,
				13 => userPreference.PaletteColor13,
				14 => userPreference.PaletteColor14,
				15 => userPreference.PaletteColor15,
				_ => throw new InvalidOperationException("Cannot fetch color due to the invalid ID value.")
			},
			{ Mode: IdentifierColorMode.Named, NamedKind: var namedKind } => namedKind switch
			{
				DisplayColorKind.Normal => userPreference.NormalColor,
				DisplayColorKind.Elimination => userPreference.EliminationColor,
				DisplayColorKind.Exofin => userPreference.ExofinColor,
				DisplayColorKind.Endofin => userPreference.EndofinColor,
				DisplayColorKind.Cannibalism => userPreference.CannibalismColor,
				DisplayColorKind.Link => userPreference.LinkColor,
				_ => throw new InvalidOperationException("Cannot fetch color due to the invalid named kind value."),
			},
			_ => throw new ArgumentOutOfRangeException(nameof(identifier))
		};
}
