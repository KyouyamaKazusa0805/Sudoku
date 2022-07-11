namespace Sudoku.UI.Drawing;

/// <summary>
/// Provides with the color-related marshaling methods.
/// </summary>
internal static class ColorMarshal
{
	/// <summary>
	/// Converts an <see cref="Identifier"/> instance into a <see cref="Color"/> instance.
	/// </summary>
	/// <param name="identifier">The <see cref="Identifier"/> value.</param>
	/// <param name="userPreference">The user preference instance.</param>
	/// <returns>The <see cref="Color"/> result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color AsColor(this Identifier identifier, IDrawingPreference userPreference)
		=> identifier.Mode switch
		{
			IdentifierColorMode.Raw when identifier is { A: var alpha, R: var red, G: var green, B: var blue }
				=> Color.FromArgb(alpha, red, green, blue),
			IdentifierColorMode.Id => userPreference.PaletteColors[identifier.Id],
			IdentifierColorMode.Named when identifier.NamedKind is var namedKind => namedKind switch
			{
				>= DisplayColorKind.Auxiliary1 and <= DisplayColorKind.Auxiliary3
					=> userPreference.AuxiliaryColors[namedKind - DisplayColorKind.Auxiliary1],
				>= DisplayColorKind.AlmostLockedSet1 and <= DisplayColorKind.AlmostLockedSet5
					=> userPreference.AlmostLockedSetColors[namedKind - DisplayColorKind.AlmostLockedSet1],
				_ when typeof(IDrawingPreference).GetProperty($"{identifier.NamedKind}Color") is { } propertyInfo
					=> (Color)propertyInfo.GetValue(userPreference)!
			}
		};

	/// <summary>
	/// Converts the specified color to the equivalent identifier instance.
	/// </summary>
	/// <param name="this">The color.</param>
	/// <returns>The identifier instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Identifier AsIdentifier(this Color @this) => (@this.A, @this.R, @this.G, @this.B);
}
