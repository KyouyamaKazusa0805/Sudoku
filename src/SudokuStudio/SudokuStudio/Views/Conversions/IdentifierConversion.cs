namespace SudokuStudio.Views.Conversions;

/// <summary>
/// Provides with conversion methods used by XAML designer, about <see cref="Identifier"/> instances.
/// </summary>
/// <seealso cref="Identifier"/>
internal static class IdentifierConversion
{
	public static Color GetColor(Identifier id)
	{
		return id switch
		{
			{ Mode: IdentifierColorMode.Raw, A: var a, R: var r, G: var g, B: var b } => Color.FromArgb(a, r, g, b),
			{ Mode: IdentifierColorMode.Id } when getValueById(id, out var color) => color,
			{ Mode: IdentifierColorMode.Named, NamedKind: var namedKind } => namedKind switch
			{
				DisplayColorKind.Normal => Constants.ColorPalette[0],
				DisplayColorKind.Assignment => Constants.ColorPalette[0],
				DisplayColorKind.Elimination => Constants.EliminationColor,
				DisplayColorKind.Exofin => Constants.ColorPalette[1],
				DisplayColorKind.Endofin => Constants.ColorPalette[2],
				DisplayColorKind.Cannibalism => Constants.CannibalismColor,
				DisplayColorKind.Link => Constants.ChainColor,
				DisplayColorKind.Auxiliary1 => Constants.ColorPalette[1],
				DisplayColorKind.Auxiliary2 => Constants.ColorPalette[2],
				DisplayColorKind.Auxiliary3 => Constants.ColorPalette[3],
				DisplayColorKind.AlmostLockedSet1 => Constants.ColorPalette[^5],
				DisplayColorKind.AlmostLockedSet2 => Constants.ColorPalette[^4],
				DisplayColorKind.AlmostLockedSet3 => Constants.ColorPalette[^3],
				DisplayColorKind.AlmostLockedSet4 => Constants.ColorPalette[^2],
				DisplayColorKind.AlmostLockedSet5 => Constants.ColorPalette[^1],
				_ => throw new InvalidOperationException("Such displaying color kind is invalid.")
			},
			_ => throw new InvalidOperationException("Such identifier instance contains invalid value.")
		};


		static bool getValueById(Identifier identifier, out Color result)
		{
			var palette = Constants.ColorPalette;
			if (identifier is { Mode: IdentifierColorMode.Id, Id: var id })
			{
				return (result = palette.Length > id ? palette[id] : Colors.Transparent) != Colors.Transparent;
			}
			else
			{
				result = Colors.Transparent;
				return false;
			}
		}
	}
}

/// <include file='../../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="constant"]'/>
file static class Constants
{
	/// <summary>
	/// The color palette. This property stores a list of customized colors to be used as user-defined colors.
	/// </summary>
	public static readonly Color[] ColorPalette =
	{
		Color.FromArgb(255,  63, 218, 101), // Green (normal)
		Color.FromArgb(255, 255, 192,  89), // Orange (auxiliary)
		Color.FromArgb(255, 127, 187, 255), // Skyblue (exo-fin)
		Color.FromArgb(255, 216, 178, 255), // Purple (endo-fin)
		Color.FromArgb(255, 197, 232, 140), // Yellowgreen
		Color.FromArgb(255, 255, 203, 203), // Light red (eliminations)
		Color.FromArgb(255, 178, 223, 223), // Blue green
		Color.FromArgb(255, 252, 220, 165), // Light orange
		Color.FromArgb(255, 255, 255, 150), // Yellow
		Color.FromArgb(255, 247, 222, 143), // Golden yellow
		Color.FromArgb(255, 220, 212, 252), // Purple
		Color.FromArgb(255, 255, 118, 132), // Red
		Color.FromArgb(255, 206, 251, 237), // Light skyblue
		Color.FromArgb(255, 215, 255, 215), // Light green
		Color.FromArgb(255, 192, 192, 192) // Gray
	};

	/// <summary>
	/// Indicates the elimination color.
	/// </summary>
	public static readonly Color EliminationColor = Color.FromArgb(255, 255, 118, 132);

	/// <summary>
	/// Indicates the cannibalism color.
	/// </summary>
	public static readonly Color CannibalismColor = Color.FromArgb(255, 235, 0, 0);

	/// <summary>
	/// Indicates the chain color.
	/// </summary>
	public static readonly Color ChainColor = Colors.Red;
}
