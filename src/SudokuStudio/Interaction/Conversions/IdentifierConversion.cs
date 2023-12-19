using Microsoft.UI;
using Microsoft.UI.Xaml;
using Sudoku.Rendering;
using Windows.UI;

namespace SudokuStudio.Interaction.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="ColorIdentifier"/> instances.
/// </summary>
/// <seealso cref="ColorIdentifier"/>
internal static class IdentifierConversion
{
	public static Color GetColor(ColorIdentifier id)
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		return id switch
		{
			ColorColorIdentifier(var a, var r, var g, var b) => Color.FromArgb(a, r, g, b),
			PaletteIdColorIdentifier { Value: var idValue } when getValueById(idValue, out var color) => color,
			WellKnownColorIdentifier { Kind: var namedKind } => namedKind switch
			{
				WellKnownColorIdentifierKind.Normal => uiPref.NormalColor,
				WellKnownColorIdentifierKind.Assignment => uiPref.AssignmentColor,
				WellKnownColorIdentifierKind.OverlappedAssignment => uiPref.OverlappedAssignmentColor,
				WellKnownColorIdentifierKind.Elimination => uiPref.EliminationColor,
				WellKnownColorIdentifierKind.Cannibalism => uiPref.CannibalismColor,
				WellKnownColorIdentifierKind.Exofin => uiPref.ExofinColor,
				WellKnownColorIdentifierKind.Endofin => uiPref.EndofinColor,
				WellKnownColorIdentifierKind.Link => uiPref.ChainColor,
				WellKnownColorIdentifierKind.Auxiliary1 => uiPref.AuxiliaryColors[0],
				WellKnownColorIdentifierKind.Auxiliary2 => uiPref.AuxiliaryColors[1],
				WellKnownColorIdentifierKind.Auxiliary3 => uiPref.AuxiliaryColors[2],
				WellKnownColorIdentifierKind.AlmostLockedSet1 => uiPref.AlmostLockedSetsColors[0],
				WellKnownColorIdentifierKind.AlmostLockedSet2 => uiPref.AlmostLockedSetsColors[1],
				WellKnownColorIdentifierKind.AlmostLockedSet3 => uiPref.AlmostLockedSetsColors[2],
				WellKnownColorIdentifierKind.AlmostLockedSet4 => uiPref.AlmostLockedSetsColors[3],
				WellKnownColorIdentifierKind.AlmostLockedSet5 => uiPref.AlmostLockedSetsColors[4],
				_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
			},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


		bool getValueById(int idValue, out Color result)
		{
			var palette = uiPref.UserDefinedColorPalette;
			return (result = palette.Count > idValue ? palette[idValue] : Colors.Transparent) != Colors.Transparent;
		}
	}
}
