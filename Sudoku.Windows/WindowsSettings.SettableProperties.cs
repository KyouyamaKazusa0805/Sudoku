#pragma warning disable CA2235

using System.Collections.Generic;
using System.Drawing;
using Sudoku.Solving;
using Sudoku.Solving.Manual;
using static Sudoku.Solving.DifficultyLevel;

namespace Sudoku.Windows
{
	partial class WindowsSettings
	{
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
#if AUTHOR_RESERVED
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
#if AUTHOR_RESERVED
		public bool PmGridCompatible { get; set; } = true;
#else
		public bool PmGridCompatible { get; set; } = false;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver will show cross hatching information single
		/// techniques).
		/// </para>
		/// <para>The value is <see langword="true"/> in debug environment, and
		/// <see langword="false"/> in release environment in default case.</para>
		/// </summary>
#if AUTHOR_RESERVED
		public bool ShowDirectLines { get; set; } = true;
#else
		public bool ShowDirectLines { get; set; } = false;
#endif

		/// <summary>
		/// <para>Indicates the size for picture to save as image files.</para>
		/// <para>The default value is <c>800F</c>.</para>
		/// </summary>
		public float SavingPictureSize { get; set; } = 800F;

		/// <summary>
		/// <para>Indicates the size for the sudoku grid control.</para>
		/// <para>The default value is <c>600D</c>.</para>
		/// </summary>
		public double GridSize { get; set; } = 600D;

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
		/// Indicates which item you selected the difficulty level combo box.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingDifficultyLevelSelectedIndex { get; set; } = 0;

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
		/// <para>Indicates the current puzzle database used.</para>
		/// <para>The default value is <c><see langword="null"/></c>.</para>
		/// </summary>
		public string? CurrentPuzzleDatabase { get; set; } = null;

		/// <summary>
		/// <para>Indicates the language code string (i.e. globalization string).</para>
		/// <para>The default value is <c><see langword="null"/></c>, which is equivalent to "<c>en-us</c>".</para>
		/// </summary>
		public string? LanguageCode { get; set; } = "en-us";

		/// <summary>
		/// <para>Indicates the format text while saving picture.</para>
		/// <para>The default value is <see langword="null"/>.</para>
		/// </summary>
		public string? OutputPictureFormatText { get; set; } = null;

		/// <summary>
		/// The main manual solver.
		/// </summary>
		public ManualSolver MainManualSolver { get; set; } = new();

		/// <summary>
		/// The color table for each difficulty level.
		/// </summary>
		public IReadOnlyDictionary<DifficultyLevel, (Color Foreground, Color Background)> DiffColors =>
			new Dictionary<DifficultyLevel, (Color, Color)>
			{
				[Unknown] = (Color.Black, Color.Gray),
				[Easy] = (Color.FromArgb(0, 51, 204), Color.FromArgb(204, 204, 255)),
				[Moderate] = (Color.FromArgb(0, 102, 0), Color.FromArgb(100, 255, 100)),
				[Hard] = (Color.FromArgb(128, 128, 0), Color.FromArgb(255, 255, 100)),
				[Fiendish] = (Color.FromArgb(102, 51, 0), Color.FromArgb(255, 150, 80)),
				[Nightmare] = (Color.FromArgb(102, 0, 0), Color.FromArgb(255, 100, 100)),
				[LastResort] = (Color.Black, Color.FromArgb(255, 100, 100))
			};
	}
}
