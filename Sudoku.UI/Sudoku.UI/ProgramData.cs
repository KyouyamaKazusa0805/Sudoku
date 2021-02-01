namespace Sudoku.UI
{
	/// <summary>
	/// This class is used for solving some data passing problems.
	/// </summary>
	internal static class ProgramData
	{
		/// <summary>
		/// Indicates the base window.
		/// </summary>
		/// <remarks>This value will be initialized when the main window is created.</remarks>
		public static MainWindow BaseWindow { get; set; } = null!;
	}
}
