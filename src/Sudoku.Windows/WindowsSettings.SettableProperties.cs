using Sudoku.Globalization;
using Sudoku.Solving.Manual;

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
		public bool AskWhileQuitting { get; set; }
#else
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver will solve and analyze the puzzle
		/// from the current grid, if the current grid has applied some steps.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool SolveFromCurrent { get; set; }

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
		public bool PmGridCompatible { get; set; }
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the solver will show cross hatching information single
		/// techniques).
		/// </para>
		/// <para>The value is <see langword="true"/> in debug environment, and
		/// <see langword="false"/> in release environment in default case.</para>
		/// </summary>
		public bool ShowDirectLines { get; set; }
#if AUTHOR_RESERVED
		= true;
#endif

		/// <summary>
		/// <para>
		/// Indicates whether the program will show the step label in solving path page.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool ShowStepLabel { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the program will show the step difficulty rating in solving path page.
		/// </para>
		/// <para>The value is <see langword="true"/> in default case.</para>
		/// </summary>
		public bool ShowStepDifficulty { get; set; } = true;

		/// <summary>
		/// <para>
		/// Indicates whether the program will display acronym rather than full name for a technique
		/// in analysis tab page.
		/// </para>
		/// <para>The value is <see langword="false"/> in default case.</para>
		/// </summary>
		public bool DisplayAcronymRatherThanFullNameOfSteps { get; set; }

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
		public int GeneratingModeComboBoxSelectedIndex { get; set; }

		/// <summary>
		/// <para>
		/// Indicates which item you selected the generating symmetry combo box.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingSymmetryModeComboBoxSelectedIndex { get; set; }

		/// <summary>
		/// <para>
		/// Indicates which item you selected the difficulty level combo box.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingDifficultyLevelSelectedIndex { get; set; }

		/// <summary>
		/// <para>
		/// Indicates the combo box of backdoor depth you selected.
		/// </para>
		/// <para>The default value is <c>0</c>.</para>
		/// </summary>
		public int GeneratingBackdoorSelectedIndex { get; set; }

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
		public string? CurrentPuzzleDatabase { get; set; }

		/// <summary>
		/// <para>Indicates the language code string (i.e. globalization string).</para>
		/// <para>
		/// The default value is <see cref="CountryCode.EnUs"/>.
		/// </para>
		/// </summary>
		/// <seealso cref="CountryCode.EnUs"/>
		public CountryCode LanguageCode { get; set; } = CountryCode.EnUs;

		/// <summary>
		/// <para>Indicates the format text while saving picture.</para>
		/// <para>The default value is <see langword="null"/>.</para>
		/// </summary>
		public string? OutputPictureFormatText { get; set; }

		/// <summary>
		/// The main manual solver.
		/// </summary>
		public ManualSolver MainManualSolver { get; set; } = new();
	}
}
