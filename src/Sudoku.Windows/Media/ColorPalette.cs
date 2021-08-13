namespace Sudoku.Windows.Media;

/// <summary>
/// Extracts the fixed colors.
/// </summary>
public static class ColorPalette
{
	/// <summary>
	/// The color table for each difficulty level.
	/// </summary>
	public static readonly IReadOnlyDictionary<
		DifficultyLevel, (DColor Foreground, DColor Background)
	> DifficultyLevelColors = new Dictionary<DifficultyLevel, (DColor, DColor)>
	{
		[DifficultyLevel.Unknown] = (DColor.Black, DColor.Gray),
		[DifficultyLevel.Easy] = (DColor.FromArgb(0, 51, 204), DColor.FromArgb(204, 204, 255)),
		[DifficultyLevel.Moderate] = (DColor.FromArgb(0, 102, 0), DColor.FromArgb(100, 255, 100)),
		[DifficultyLevel.Hard] = (DColor.FromArgb(128, 128, 0), DColor.FromArgb(255, 255, 100)),
		[DifficultyLevel.Fiendish] = (DColor.FromArgb(102, 51, 0), DColor.FromArgb(255, 150, 80)),
		[DifficultyLevel.Nightmare] = (DColor.FromArgb(102, 0, 0), DColor.FromArgb(255, 100, 100)),
		[DifficultyLevel.LastResort] = (DColor.Black, DColor.FromArgb(255, 100, 100))
	};

	/// <summary>
	/// Indicates the color palette that is shown on the control.
	/// </summary>
	public static readonly IReadOnlyCollection<WColor> PaletteColors = new[]
	{
		Colors.Black,
		Colors.Red,
		Colors.DarkOrange,
		Colors.Yellow,
		Colors.LawnGreen,
		Colors.Blue,
		Colors.Purple,
		Colors.DeepPink,
		Colors.Aqua,
		Colors.SaddleBrown,
		Colors.Wheat,
		Colors.BurlyWood,
		Colors.Teal,

		Colors.White,
		Colors.OrangeRed,
		Colors.Orange,
		Colors.Gold,
		Colors.LimeGreen,
		Colors.DodgerBlue,
		Colors.Orchid,
		Colors.HotPink,
		Colors.Turquoise,
		Colors.SandyBrown,
		Colors.SeaGreen,
		Colors.SlateBlue,
		Colors.RoyalBlue,

		Colors.Tan,
		Colors.Peru,
		Colors.DarkBlue,
		Colors.DarkGreen,
		Colors.DarkSlateBlue,
		Colors.Navy,
		Colors.MistyRose,
		Colors.LemonChiffon,
		Colors.ForestGreen,
		Colors.Firebrick,
		Colors.DarkViolet,
		Colors.Aquamarine,
		Colors.CornflowerBlue,
		Colors.Bisque,
		Colors.WhiteSmoke,
		Colors.AliceBlue,

		WColor.FromArgb(255, 5, 5, 5),
		WColor.FromArgb(255, 15, 15, 15),
		WColor.FromArgb(255, 35, 35, 35),
		WColor.FromArgb(255, 55, 55, 55),
		WColor.FromArgb(255, 75, 75, 75),
		WColor.FromArgb(255, 95, 95, 95),
		WColor.FromArgb(255, 115, 115, 115),
		WColor.FromArgb(255, 135, 135, 135),
		WColor.FromArgb(255, 155, 155, 155),
		WColor.FromArgb(255, 175, 175, 175),
		WColor.FromArgb(255, 195, 195, 195),
		WColor.FromArgb(255, 215, 215, 215),
		WColor.FromArgb(255, 235, 235, 235),
	};
}
