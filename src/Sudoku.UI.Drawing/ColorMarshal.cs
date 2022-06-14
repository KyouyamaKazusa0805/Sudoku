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
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified ID or named kind value is invalid.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Color AsColor(this Identifier identifier, IDrawingPreference userPreference)
	{
		return identifier.Mode switch
		{
			IdentifierColorMode.Raw when identifier is { A: var alpha, R: var red, G: var green, B: var blue }
				=> Color.FromArgb(alpha, red, green, blue),
			IdentifierColorMode.Id when g($"PaletteColor{identifier.Id}") is { } propertyInfo
				=> (Color)propertyInfo.GetValue(userPreference)!,
			IdentifierColorMode.Named when g($"{identifier.NamedKind}Color") is { } propertyInfo
				=> (Color)propertyInfo.GetValue(userPreference)!,
			_ => throw new InvalidOperationException("The specified mode is not supported due to invalid inner value.")
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static PropertyInfo? g(string s) => typeof(IDrawingPreference).GetProperty(s);
	}
}
