using Sudoku.Data;
using Sudoku.Painting;

namespace Sudoku.UI.Data
{
	/// <summary>
	/// Provides the custom settings used in this program.
	/// </summary>
	public sealed class PagePreferences : PreferencesBase
	{
		/// <summary>
		/// Indicates whether the program will ask you "Do you want to quit?" after you clicked the close button.
		/// The default value is <see langword="false"/>.
		/// </summary>
		public bool AskBeforeQuitting { get; set; } = false;

		/// <summary>
		/// Indicates whether the program use zero character <c>'0'</c> as placeholders. The default value
		/// <see langword="true"/>.
		/// </summary>
		public bool UseZeroCharacterWhenCopyCode { get; set; } = true;

		/// <summary>
		/// Indicates whether the program will only display same-level techniques while searching for all steps.
		/// The default value is <see langword="true"/>.
		/// </summary>
		public bool OnlyDisplaySameLevelStepsWhenFindAllSteps { get; set; } = true;

		/// <summary>
		/// Indicates the last grid used. The default value is <see cref="SudokuGrid.Empty"/>.
		/// </summary>
		public SudokuGrid LastGrid { get; set; } = SudokuGrid.Empty;
	}
}
