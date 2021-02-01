using System.Drawing;

namespace Sudoku.Painting
{
	partial class Theme
	{
		/// <summary>
		/// Indicates the background color of the grid. The default value is <see cref="Color.White"/>.
		/// </summary>
		public Color BackgroundColor { get; set; } = Color.White;

		/// <summary>
		/// Indicates the grid line color of the grid. The default value is <see cref="Color.Gray"/>.
		/// </summary>
		public Color GridLineColor { get; set; } = Color.Black;

		/// <summary>
		/// Indicates the block line color of the grid. The default value is <see cref="Color.Gray"/>.
		/// </summary>
		public Color BlockLineColor { get; set; } = Color.Black;

		/// <summary>
		/// Indicates the focused cells color. The default value is <see cref="Color.Yellow"/>
		/// with the alpha 32.
		/// </summary>
		public Color FocusedCellsColor { get; set; } = Color.FromArgb(32, Color.Yellow);

		/// <summary>
		/// Indicates the elimination color.
		/// </summary>
		public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132);

		/// <summary>
		/// Indicates the cannibalism color.
		/// </summary>
		public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0);

		/// <summary>
		/// Indicates the given digits to render. The default value is <see cref="Color.Black"/>.
		/// </summary>
		public Color GivenColor { get; set; } = Color.Black;

		/// <summary>
		/// Indicates the modifiable digits to render. The default value is <see cref="Color.Blue"/>.
		/// </summary>
		public Color ModifiableColor { get; set; } = Color.Blue;

		/// <summary>
		/// Indicates the candidate digits to render. The default value is <see cref="Color.DimGray"/>.
		/// </summary>
		public Color CandidateColor { get; set; } = Color.DimGray;

		/// <summary>
		/// Indicates the chain color.
		/// </summary>
		public Color ChainColor { get; set; } = Color.Red;

		/// <summary>
		/// <para>
		/// Indicates the color of the crosshatching outline.
		/// </para>
		/// <para>The value is <see cref="Color.Black"/> with the alpha 192 in debugger mode,
		/// <see cref="Color.SkyBlue"/> with the alpha 192 in release mode in default case.</para>
		/// </summary>
		public Color CrosshatchingOutlineColor { get; set; } = Color.FromArgb(192, Color.Black);

		/// <summary>
		/// <para>
		/// Indicates the color of the crosshatching inner.
		/// </para>
		/// <para>
		/// The value is <see cref="Color.Transparent"/> in author-reserved environment,
		/// <see cref="Color.SkyBlue"/> in other ones in default case.
		/// </para>
		/// </summary>
		public Color CrosshatchingInnerColor { get; set; } = Color.Transparent;

		/// <summary>
		/// <para>
		/// Indicates the color of the cross sign.
		/// </para>
		/// <para>The value is <see cref="Color.Red"/> with the alpha 192 in default case.</para>
		/// </summary>
		public Color CrossSignColor { get; set; } = Color.FromArgb(192, Color.Red);

		/// <summary>
		/// Indicates the color 1.
		/// </summary>
		public Color Color1 { get; set; } = Color.FromArgb(63, 218, 101);

		/// <summary>
		/// Indicates the color 2.
		/// </summary>
		public Color Color2 { get; set; } = Color.FromArgb(255, 192, 89);

		/// <summary>
		/// Indicates the color 3.
		/// </summary>
		public Color Color3 { get; set; } = Color.FromArgb(127, 187, 255);

		/// <summary>
		/// Indicates the color 4.
		/// </summary>
		public Color Color4 { get; set; } = Color.FromArgb(216, 178, 255);

		/// <summary>
		/// Indicates the color 5.
		/// </summary>
		public Color Color5 { get; set; } = Color.FromArgb(197, 232, 140);

		/// <summary>
		/// Indicates the color 6.
		/// </summary>
		public Color Color6 { get; set; } = Color.FromArgb(255, 203, 203);

		/// <summary>
		/// Indicates the color 7.
		/// </summary>
		public Color Color7 { get; set; } = Color.FromArgb(178, 223, 223);

		/// <summary>
		/// Indicates the color 8.
		/// </summary>
		public Color Color8 { get; set; } = Color.FromArgb(252, 220, 165);

		/// <summary>
		/// Indicates the color 9.
		/// </summary>
		public Color Color9 { get; set; } = Color.FromArgb(255, 255, 150);

		/// <summary>
		/// Indicates the color 10.
		/// </summary>
		public Color Color10 { get; set; } = Color.FromArgb(247, 222, 143);

		/// <summary>
		/// Indicates the color 11.
		/// </summary>
		public Color Color11 { get; set; } = Color.FromArgb(220, 212, 252);

		/// <summary>
		/// Indicates the color 12.
		/// </summary>
		public Color Color12 { get; set; } = Color.FromArgb(255, 118, 132);

		/// <summary>
		/// Indicates the color 13.
		/// </summary>
		public Color Color13 { get; set; } = Color.FromArgb(206, 251, 237);

		/// <summary>
		/// Indicates the color 14.
		/// </summary>
		public Color Color14 { get; set; } = Color.FromArgb(215, 255, 215);

		/// <summary>
		/// Indicates the color 15.
		/// </summary>
		public Color Color15 { get; set; } = Color.FromArgb(192, 192, 192);
	}
}
