using System.Drawing;

namespace Sudoku.Forms
{
	partial class Settings
	{
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
		/// <para>The value is <c>3F</c> in default case.</para>
		/// </summary>
		public float BlockLineWidth { get; set; } = 3F;

		/// <summary>
		/// <para>Indicates the scale of values.</para>
		/// <para>The value is <c>0.8F</c> in default case.</para>
		/// </summary>
		public float ValueScale { get; set; } = .8F;

		/// <summary>
		/// <para>Indicates the scale of candidates.</para>
		/// <para>The value is <c>0.3F</c> in default case.</para>
		/// </summary>
		public float CandidateScale { get; set; } = .3F;

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
	}
}
