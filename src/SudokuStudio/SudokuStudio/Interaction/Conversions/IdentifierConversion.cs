namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="Identifier"/> instances.
/// </summary>
/// <seealso cref="Identifier"/>
internal static class IdentifierConversion
{
	public static Color GetColor(Identifier id)
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		return id switch
		{
			{ Mode: IdentifierColorMode.Raw, A: var a, R: var r, G: var g, B: var b } => Color.FromArgb(a, r, g, b),
			{ Mode: IdentifierColorMode.Id } when getValueById(id, out var color) => color,
			{ Mode: IdentifierColorMode.Named, NamedKind: var namedKind }
				=> namedKind switch
				{
					DisplayColorKind.Normal => uiPref.NormalColor,
					DisplayColorKind.Assignment => uiPref.AssignmentColor,
					DisplayColorKind.OverlappedAssignment => uiPref.OverlappedAssignmentColor,
					DisplayColorKind.Elimination => uiPref.EliminationColor,
					DisplayColorKind.Cannibalism => uiPref.CannibalismColor,
					DisplayColorKind.Exofin => uiPref.ExofinColor,
					DisplayColorKind.Endofin => uiPref.EndofinColor,
					DisplayColorKind.Link => uiPref.ChainColor,
					DisplayColorKind.Auxiliary1 => uiPref.AuxiliaryColors[0],
					DisplayColorKind.Auxiliary2 => uiPref.AuxiliaryColors[1],
					DisplayColorKind.Auxiliary3 => uiPref.AuxiliaryColors[2],
					DisplayColorKind.AlmostLockedSet1 => uiPref.AlmostLockedSetsColors[0],
					DisplayColorKind.AlmostLockedSet2 => uiPref.AlmostLockedSetsColors[1],
					DisplayColorKind.AlmostLockedSet3 => uiPref.AlmostLockedSetsColors[2],
					DisplayColorKind.AlmostLockedSet4 => uiPref.AlmostLockedSetsColors[3],
					DisplayColorKind.AlmostLockedSet5 => uiPref.AlmostLockedSetsColors[4],
					_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
				},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


		bool getValueById(Identifier identifier, out Color result)
		{
			var palette = uiPref.UserDefinedColorPalette;
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
