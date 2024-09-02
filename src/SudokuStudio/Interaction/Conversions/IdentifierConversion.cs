namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="ColorIdentifier"/> instances.
/// </summary>
/// <seealso cref="ColorIdentifier"/>
internal static class IdentifierConversion
{
	public static Color GetColor(ColorIdentifier id)
	{
		var uiPref = Application.Current.AsApp().Preference.UIPreferences;
		return id switch
		{
			ColorColorIdentifier(var a, var r, var g, var b) => Color.FromArgb(a, r, g, b),
			PaletteIdColorIdentifier { Value: var idValue } when getValueById(idValue, out var color) => color,
			WellKnownColorIdentifier { Kind: var namedKind } => namedKind switch
			{
				ColorIdentifierKind.Normal => uiPref.NormalColor,
				ColorIdentifierKind.Assignment => uiPref.AssignmentColor,
				ColorIdentifierKind.OverlappedAssignment => uiPref.OverlappedAssignmentColor,
				ColorIdentifierKind.Elimination => uiPref.EliminationColor,
				ColorIdentifierKind.Cannibalism => uiPref.CannibalismColor,
				ColorIdentifierKind.Exofin => uiPref.ExofinColor,
				ColorIdentifierKind.Endofin => uiPref.EndofinColor,
				ColorIdentifierKind.Link => uiPref.ChainColor,
				ColorIdentifierKind.Auxiliary1 => uiPref.AuxiliaryColors[0],
				ColorIdentifierKind.Auxiliary2 => uiPref.AuxiliaryColors[1],
				ColorIdentifierKind.Auxiliary3 => uiPref.AuxiliaryColors[2],
				ColorIdentifierKind.AlmostLockedSet1 => uiPref.AlmostLockedSetsColors[0],
				ColorIdentifierKind.AlmostLockedSet2 => uiPref.AlmostLockedSetsColors[1],
				ColorIdentifierKind.AlmostLockedSet3 => uiPref.AlmostLockedSetsColors[2],
				ColorIdentifierKind.AlmostLockedSet4 => uiPref.AlmostLockedSetsColors[3],
				ColorIdentifierKind.AlmostLockedSet5 => uiPref.AlmostLockedSetsColors[4],
				_ => throw new InvalidOperationException(SR.ExceptionMessage("SuchColorCannotBeFound"))
			},
			_ => throw new InvalidOperationException(SR.ExceptionMessage("SuchInstanceIsInvalid"))
		};


		bool getValueById(int idValue, out Color result)
		{
			var palette = uiPref.UserDefinedColorPalette;
			return (result = palette.Count > idValue ? palette[idValue] : Colors.Transparent) != Colors.Transparent;
		}
	}
}
