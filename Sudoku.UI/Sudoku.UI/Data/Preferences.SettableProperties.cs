namespace Sudoku.UI.Data
{
	partial class Preferences
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
	}
}
