using System.Collections.Generic;
using System.Drawing;
using Sudoku.Solving.Manual;

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
		/// Indicates whether the program use Sudoku Explainer mode to check
		/// the difficulty rating of each puzzle strictly.
		/// </para>
		/// <para>
		/// The value is <see langword="false"/> in default case. If the value
		/// is <see langword="false"/>, the mode will be Hodoku compatible mode,
		/// where all puzzles will be solved preferring techniques instead of
		/// their difficulty ratings.
		/// </para>
		/// </summary>
		public bool SeMode { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the searcher will use fast search to search all
		/// steps. This option is equivalent to <see cref="ManualSolver.FastSearch"/>.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		/// <seealso cref="ManualSolver.FastSearch"/>
		public bool FastSearch { get; set; } = false;

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

		/// <summary>
		/// Indicates the color dictionary.
		/// </summary>
		public IDictionary<int, Color> ColorDictionary
		{
			get
			{
				return new Dictionary<int, Color>
				{
					[-4] = Color8, [-3] = Color7, [-2] = Color6, [-1] = Color5,
					[0] = Color1, [1] = Color2, [2] = Color3,
					[3] = Color4, [4] = Color9, [5] = Color10,
					[6] = Color11, [7] = Color12, [8] = Color13,
					[9] = Color14, [10] = Color15,
				};
			}
		}

		/// <summary>
		/// Indicates the elimination color.
		/// </summary>
		public Color EliminationColor { get; set; } = Color.FromArgb(255, 255, 118, 132);

		/// <summary>
		/// Indicates the cannibalism color.
		/// </summary>
		public Color CannibalismColor { get; set; } = Color.FromArgb(255, 235, 0, 0);

		/// <summary>
		/// Indicates the color 1.
		/// </summary>
		public Color Color1 { get; set; } = Color.FromArgb(255, 63, 218, 101);

		/// <summary>
		/// Indicates the color 2.
		/// </summary>
		public Color Color2 { get; set; } = Color.FromArgb(255, 255, 192, 89);

		/// <summary>
		/// Indicates the color 3.
		/// </summary>
		public Color Color3 { get; set; } = Color.FromArgb(255, 127, 187, 255);

		/// <summary>
		/// Indicates the color 4.
		/// </summary>
		public Color Color4 { get; set; } = Color.FromArgb(255, 216, 178, 255);

		/// <summary>
		/// Indicates the color 5.
		/// </summary>
		public Color Color5 { get; set; } = Color.FromArgb(255, 197, 232, 140);

		/// <summary>
		/// Indicates the color 6.
		/// </summary>
		public Color Color6 { get; set; } = Color.FromArgb(255, 255, 203, 203);

		/// <summary>
		/// Indicates the color 7.
		/// </summary>
		public Color Color7 { get; set; } = Color.FromArgb(255, 178, 223, 223);

		/// <summary>
		/// Indicates the color 8.
		/// </summary>
		public Color Color8 { get; set; } = Color.FromArgb(255, 252, 220, 165);

		/// <summary>
		/// Indicates the color 9.
		/// </summary>
		public Color Color9 { get; set; } = Color.FromArgb(255, 255, 255, 150);

		/// <summary>
		/// Indicates the color 10.
		/// </summary>
		public Color Color10 { get; set; } = Color.FromArgb(255, 247, 222, 143);

		/// <summary>
		/// Indicates the color 11.
		/// </summary>
		public Color Color11 { get; set; } = Color.FromArgb(255, 220, 212, 252);

		/// <summary>
		/// Indicates the color 12.
		/// </summary>
		public Color Color12 { get; set; } = Color.FromArgb(255, 255, 210, 210);

		/// <summary>
		/// Indicates the color 13.
		/// </summary>
		public Color Color13 { get; set; } = Color.FromArgb(255, 206, 251, 237);

		/// <summary>
		/// Indicates the color 14.
		/// </summary>
		public Color Color14 { get; set; } = Color.FromArgb(255, 215, 255, 215);

		/// <summary>
		/// Indicates the color 15.
		/// </summary>
		public Color Color15 { get; set; } = Color.FromArgb(255, 192, 192, 192);
	}
}
