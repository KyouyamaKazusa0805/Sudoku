using System.Collections.Generic;
using System.Drawing;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using static Sudoku.Solving.DifficultyLevel;

namespace Sudoku.Windows
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
		/// <para>
		/// The value is <see langword="false"/> in debug environment,
		/// and <see langword="true"/> in release environment.
		/// </para>
		/// </summary>
#if DEBUG
		public bool AskWhileQuitting { get; set; } = false;
#else
		public bool AskWhileQuitting { get; set; } = true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver will solve and analyze the puzzle
		/// from the current grid, if the current grid has applied some steps.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool SolveFromCurrent { get; set; } = false;

		/// <summary>
		/// <para>
		/// Indicates whether the text format of the sudoku grid will use zero
		/// character '<c>0</c>' to be placeholders instead of dot character
		/// '<c>.</c>'.
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in default case. If the value is
		/// <see langword="false"/>, the placeholders will be '<c>.</c>'.
		/// </para>
		/// </summary>
		public bool TextFormatPlaceholdersAreZero { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the text format of pencil-marked grid will use
		/// compatible mode to output (without given and modifiable values
		/// notations).
		/// </para>
		/// <para>
		/// The value is <see langword="true"/> in debug environment, and
		/// <see langword="false"/> in release environment.
		/// </para>
		/// </summary>
#if DEBUG
		public bool PmGridCompatible { get; set; } = true;
#else
		public bool PmGridCompatible { get; set; } = false;
#endif

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
		/// <para>Indicates the size for picture to save as image files.</para>
		/// <para>The default value is <c>800F</c>.</para>
		/// </summary>
		public float SavingPictureSize { get; set; } = 800F;

		/// <summary>
		/// <para>
		/// Indicates which item you selected the generating combo box.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingModeComboBoxSelectedIndex { get; set; } = 0;

		/// <summary>
		/// <para>
		/// Indicates which item you selected the generating symmetry combo box.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingSymmetryModeComboBoxSelectedIndex { get; set; } = 0;

		/// <summary>
		/// <para>
		/// Indicates the combo box of backdoor depth you selected.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingBackdoorSelectedIndex { get; set; } = 0;

		/// <summary>
		/// <para>
		/// Indicates the current puzzle number used in the database.
		/// </para>
		/// <para>
		/// The default value is <c>-1</c>, which means the database is unavailable.
		/// </para>
		/// </summary>
		public int CurrentPuzzleNumber { get; set; } = -1;

		/// <summary>
		/// <para>Indicates the scale of values.</para>
		/// <para>The value is <c>0.9M</c> in default case.</para>
		/// </summary>
		public decimal ValueScale { get; set; } = .9M;

		/// <summary>
		/// <para>Indicates the scale of candidates.</para>
		/// <para>The value is <c>0.3M</c> in default case.</para>
		/// </summary>
		public decimal CandidateScale { get; set; } = .3M;

		/// <summary>
		/// <para>
		/// Indicates the font of given digits to render.
		/// </para>
		/// <para>
		/// The value is <c>"Fira Code"</c> in debug environment,
		/// <c>"Arial"</c> in release environment.
		/// </para>
		/// </summary>
#if DEBUG
		public string GivenFontName { get; set; } = "Fira Code";
#else
		public string GivenFontName { get; set; } = "Arial";
#endif

		/// <summary>
		/// <para>
		/// Indicates the font of modifiable digits to render.
		/// </para>
		/// <para>
		/// The value is <c>"Fira Code"</c> in debug environment,
		/// <c>"Arial"</c> in release environment.
		/// </para>
		/// </summary>
#if DEBUG
		public string ModifiableFontName { get; set; } = "Fira Code";
#else
		public string ModifiableFontName { get; set; } = "Arial";
#endif

		/// <summary>
		/// <para>
		/// Indicates the font of candidate digits to render.
		/// </para>
		/// <para>
		/// The value is <c>"Fira Code"</c> in debug environment,
		/// <c>"Arial"</c> in release environment.
		/// </para>
		/// </summary>
#if DEBUG
		public string CandidateFontName { get; set; } = "Fira Code";
#else
		public string CandidateFontName { get; set; } = "Arial";
#endif

		/// <summary>
		/// <para>
		/// Indicates the current puzzle database used.
		/// </para>
		/// <para>The default value is <c><see langword="null"/></c>.</para>
		/// </summary>
		public string? CurrentPuzzleDatabase { get; set; } = null;

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
		/// The value is <c>#20FFFF00</c> (<see cref="Color.Yellow"/>
		/// with alpha <c>32</c>) in default case.
		/// </para>
		/// </summary>
		public Color FocusedCellColor { get; set; } = Color.FromArgb(32, Color.Yellow);

		/// <summary>
		/// Indicates the elimination color.
		/// </summary>
		public Color EliminationColor { get; set; } = Color.FromArgb(255, 118, 132);

		/// <summary>
		/// Indicates the cannibalism color.
		/// </summary>
		public Color CannibalismColor { get; set; } = Color.FromArgb(235, 0, 0);

		/// <summary>
		/// Indicates the chain color.
		/// </summary>
		public Color ChainColor { get; set; } = Color.Red;

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
		public Color Color12 { get; set; } = Color.FromArgb(255, 210, 210);

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

		/// <summary>
		/// The main manual solver.
		/// </summary>
		public ManualSolver MainManualSolver { get; set; } = new ManualSolver();

		/// <summary>
		/// The color table for each difficulty level.
		/// </summary>
		public IDictionary<DifficultyLevel, (Color _foreground, Color _background)> DiffColors =>
			new Dictionary<DifficultyLevel, (Color, Color)>
			{
				[Unknown] = (Color.Black, Color.Gray),
				[VeryEasy] = (Color.Black, Color.FromArgb(204, 204, 255)),
				[Easy] = (Color.Black, Color.FromArgb(204, 204, 255)),
				[Moderate] = (Color.Black, Color.FromArgb(100, 255, 100)),
				[Advanced] = (Color.Black, Color.FromArgb(100, 255, 100)),
				[Hard] = (Color.Black, Color.FromArgb(255, 255, 100)),
				[VeryHard] = (Color.Black, Color.FromArgb(255, 255, 100)),
				[Fiendish] = (Color.Black, Color.FromArgb(255, 150, 80)),
				[Diabolical] = (Color.Black, Color.FromArgb(255, 150, 80)),
				[Crazy] = (Color.Black, Color.FromArgb(255, 100, 100)),
				[Nightmare] = (Color.Black, Color.FromArgb(255, 100, 100)),
				[BeyondNightmare] = (Color.Black, Color.FromArgb(255, 100, 100)),
				[LastResort] = (Color.Black, Color.FromArgb(255, 100, 100))
			};

		/// <summary>
		/// Indicates the palette colors.
		/// </summary>
		public IDictionary<int, Color> PaletteColors =>
			new Dictionary<int, Color>
			{
				[-4] = Color8, [-3] = Color7, [-2] = Color6, [-1] = Color5,
				[0] = Color1, [1] = Color2, [2] = Color3,
				[3] = Color4, [4] = Color9, [5] = Color10,
				[6] = Color11, [7] = Color12, [8] = Color13,
				[9] = Color14, [10] = Color15,
			};
	}
}
