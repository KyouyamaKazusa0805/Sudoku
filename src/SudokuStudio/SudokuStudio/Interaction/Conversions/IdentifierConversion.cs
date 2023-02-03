namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="Identifier"/> instances.
/// </summary>
/// <seealso cref="Identifier"/>
internal static class IdentifierConversion
{
	public static Color GetColor(Identifier id)
	{
		var uiPref = ((App)Application.Current).ProgramPreference.UIPreferences;
		return id switch
		{
			{ Mode: IdentifierColorMode.Raw, A: var a, R: var r, G: var g, B: var b } => Color.FromArgb(a, r, g, b),
			{ Mode: IdentifierColorMode.Id } when getValueById(id, out var color) => color,
			{ Mode: IdentifierColorMode.Named, NamedKind: var namedKind } => namedKind switch
			{
				DisplayColorKind.Normal => uiPref.ColorPalette[0],
				DisplayColorKind.Assignment => uiPref.ColorPalette[0],
				DisplayColorKind.Elimination => uiPref.EliminationColor,
				DisplayColorKind.Exofin => uiPref.ColorPalette[1],
				DisplayColorKind.Endofin => uiPref.ColorPalette[2],
				DisplayColorKind.Cannibalism => uiPref.CannibalismColor,
				DisplayColorKind.Link => uiPref.ChainColor,
				DisplayColorKind.Auxiliary1 => uiPref.ColorPalette[1],
				DisplayColorKind.Auxiliary2 => uiPref.ColorPalette[2],
				DisplayColorKind.Auxiliary3 => uiPref.ColorPalette[3],
				DisplayColorKind.AlmostLockedSet1 => uiPref.ColorPalette[^5],
				DisplayColorKind.AlmostLockedSet2 => uiPref.ColorPalette[^4],
				DisplayColorKind.AlmostLockedSet3 => uiPref.ColorPalette[^3],
				DisplayColorKind.AlmostLockedSet4 => uiPref.ColorPalette[^2],
				DisplayColorKind.AlmostLockedSet5 => uiPref.ColorPalette[^1],
				_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
			},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


		bool getValueById(Identifier identifier, out Color result)
		{
			var palette = uiPref.ColorPalette;
			if (identifier is { Mode: IdentifierColorMode.Id, Id: var id })
			{
				return (result = palette.Count > id ? palette[id] : Colors.Transparent) != Colors.Transparent;
			}
			else
			{
				result = Colors.Transparent;
				return false;
			}
		}
	}
}
