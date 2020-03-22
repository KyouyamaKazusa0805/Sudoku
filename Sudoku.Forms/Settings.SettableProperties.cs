using System.Drawing;

namespace Sudoku.Forms
{
	partial class Settings
	{
		/// <summary>
		/// <para>Indicates whether the form shows candidates.</para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool ShowCandidates { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the program ask you "Do you want to quit?" while
		/// you clicked the close button.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool AskWhileQuitting { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates the grid line width of the sudoku grid to render.
		/// </para>
		/// <para>The value is <c>1.5F</c> in default case.</para>
		/// </summary>
		public float GridLineWidth { get; set; } = 1.5F;

		/// <summary>
		/// <para>
		/// Indicates the block line width of the sudoku grid to render.
		/// </para>
		/// <para>The value is <c>5F</c> in default case.</para>
		/// </summary>
		public float BlockLineWidth { get; set; } = 5F;

		/// <summary>
		/// <para>Indicates the scale of values.</para>
		/// <para>The value is <c>0.8M</c> in default case.</para>
		/// </summary>
		public decimal ValueScale { get; set; } = .8M;

		/// <summary>
		/// <para>Indicates the scale of candidates.</para>
		/// <para>The value is <c>0.3M</c> in default case.</para>
		/// </summary>
		public decimal CandidateScale { get; set; } = .3M;

		/// <summary>
		/// <para>
		/// Indicates the font of given digits to render.
		/// </para>
		/// <para>The value is <c>"Arial"</c> in default case.</para>
		/// </summary>
		public string GivenFontName { get; set; } = "Arial";

		/// <summary>
		/// <para>
		/// Indicates the font of modifiable digits to render.
		/// </para>
		/// <para>The value is <c>"Arial"</c> in default case.</para>
		/// </summary>
		public string ModifiableFontName { get; set; } = "Arial";

		/// <summary>
		/// <para>
		/// Indicates the font of candidate digits to render.
		/// </para>
		/// <para>The value is <c>"Arial"</c> in default case.</para>
		/// </summary>
		public string CandidateFontName { get; set; } = "Arial";

		/// <summary>
		/// <para>
		/// Indicates the background color of the sudoku grid to render.
		/// </para>
		/// <para>The value is <see cref="Color.White"/> in default case.</para>
		/// </summary>
		public Color BackgroundColor { get; set; } = Color.White;

		/// <summary>
		/// <para>
		/// Indicates the grid line color of the sudoku grid to render.
		/// </para>
		/// <para>The value is <see cref="Color.Black"/></para> in default case.
		/// </summary>
		public Color GridLineColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>
		/// Indicates the block line color of the sudoku grid to render.
		/// </para>
		/// <para>The value is <see cref="Color.Black"/> in default case.</para>
		/// </summary>
		public Color BlockLineColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>Indicates the given digits to render.</para>
		/// <para>The value is <see cref="Color.Black"/> in default case.</para>
		/// </summary>
		public Color GivenColor { get; set; } = Color.Black;

		/// <summary>
		/// <para>Indicates the modifiable digits to render.</para>
		/// <para>The value is <see cref="Color.Blue"/> in default case.</para>
		/// </summary>
		public Color ModifiableColor { get; set; } = Color.Blue;

		/// <summary>
		/// <para>Indicates the candidate digits to render.</para>
		/// <para>The value is <see cref="Color.DimGray"/> in default case.</para>
		/// </summary>
		public Color CandidateColor { get; set; } = Color.DimGray;

		/// <summary>
		/// <para>Indicates the color used for painting for focused cells.</para>
		/// <para>
		/// The value is <c>#20000000</c> (<see cref="Color.Black"/>
		/// with alpha <c>32</c>) in default case.
		/// </para>
		/// </summary>
		public Color FocusedCellColor { get; set; } = Color.FromArgb(32, Color.Black);
	}
}
