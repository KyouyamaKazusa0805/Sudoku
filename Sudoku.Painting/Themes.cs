using System.Drawing;

namespace Sudoku.Painting
{
	/// <summary>
	/// Provides the default themes used.
	/// </summary>
	public static class Themes
	{
		/// <summary>
		/// Indicates the default light theme.
		/// </summary>
		public static readonly Theme LightTheme = new()
		{
			BackgroundColor = Color.White,
			GridLineColor = Color.Black,
			BlockLineColor = Color.Black,
			FocusedCellsColor = Color.FromArgb(32, Color.Yellow),
			EliminationColor = Color.FromArgb(255, 118, 132),
			CannibalismColor = Color.FromArgb(235, 0, 0),
			GivenColor = Color.Black,
			ModifiableColor = Color.Blue,
			CandidateColor = Color.DimGray,
			ChainColor = Color.Red,
			CrosshatchingOutlineColor = Color.FromArgb(192, Color.Black),
			CrosshatchingInnerColor = Color.Transparent,
			CrossSignColor = Color.FromArgb(192, Color.Red),
			Color1 = Color.FromArgb(63, 218, 101),
			Color2 = Color.FromArgb(255, 192, 89),
			Color3 = Color.FromArgb(127, 187, 255),
			Color4 = Color.FromArgb(216, 178, 255),
			Color5 = Color.FromArgb(197, 232, 140),
			Color6 = Color.FromArgb(255, 203, 203),
			Color7 = Color.FromArgb(178, 223, 223),
			Color8 = Color.FromArgb(252, 220, 165),
			Color9 = Color.FromArgb(255, 255, 150),
			Color10 = Color.FromArgb(247, 222, 143),
			Color11 = Color.FromArgb(220, 212, 252),
			Color12 = Color.FromArgb(255, 118, 132),
			Color13 = Color.FromArgb(206, 251, 237),
			Color14 = Color.FromArgb(215, 255, 215),
			Color15 = Color.FromArgb(192, 192, 192)
		};

		/// <summary>
		/// Indicates the default dark theme.
		/// </summary>
		public static readonly Theme DarkTheme = new()
		{
			BackgroundColor = Color.Black,
			GridLineColor = Color.White,
			BlockLineColor = Color.White,
			FocusedCellsColor = Color.FromArgb(175, 175, 175),
			EliminationColor = Color.FromArgb(255, 118, 132),
			CannibalismColor = Color.FromArgb(235, 0, 0),
			GivenColor = Color.White,
			ModifiableColor = Color.FromArgb(100, 149, 237),
			CandidateColor = Color.FromArgb(215, 215, 215),
			ChainColor = Color.Red,
			CrosshatchingOutlineColor = Color.FromArgb(192, Color.Black),
			CrosshatchingInnerColor = Color.Transparent,
			CrossSignColor = Color.FromArgb(192, Color.Red),
			Color1 = Color.FromArgb(63, 218, 101),
			Color2 = Color.FromArgb(255, 192, 89),
			Color3 = Color.FromArgb(127, 187, 255),
			Color4 = Color.FromArgb(216, 178, 255),
			Color5 = Color.FromArgb(197, 232, 140),
			Color6 = Color.FromArgb(255, 203, 203),
			Color7 = Color.FromArgb(178, 223, 223),
			Color8 = Color.FromArgb(252, 220, 165),
			Color9 = Color.FromArgb(255, 255, 150),
			Color10 = Color.FromArgb(247, 222, 143),
			Color11 = Color.FromArgb(220, 212, 252),
			Color12 = Color.FromArgb(255, 118, 132),
			Color13 = Color.FromArgb(206, 251, 237),
			Color14 = Color.FromArgb(215, 255, 215),
			Color15 = Color.FromArgb(192, 192, 192)
		};
	}
}
