namespace Sudoku.Diagnostics.CodeAnalysis;

partial class Constants
{
	/// <summary>
	/// Indicates the full names of some special types.
	/// </summary>
	public static class TypeNames
	{
		/// <summary>
		/// Indicates the type <c>Cells</c>.
		/// </summary>
		public const string Cells = "Sudoku.Data.Cells";

		/// <summary>
		/// Indicates the type <c>Candidates</c>.
		/// </summary>
		public const string Candidates = "Sudoku.Data.Candidates";

		/// <summary>
		/// Indicates the type <c>Grid</c>.
		/// </summary>
		public const string Grid = "Sudoku.Data.Grid";

		/// <summary>
		/// Indicates the type <c>SudokuGrid</c>.
		/// </summary>
		public const string SudokuGrid = "Sudoku.Data.SudokuGrid";

		/// <summary>
		/// Indicates the type <c>StepSearcherAttribute</c>.
		/// </summary>
		public const string StepSearcherAttribute = "Sudoku.Solving.Manual.StepSearcherAttribute";

		/// <summary>
		/// Indicates the type <c>IStepSearcher</c>.
		/// </summary>
		public const string IStepSearcher = "Sudoku.Solving.Manual.Searchers.IStepSearcher";

		/// <summary>
		/// Indicates the type <c>FastProperties</c>.
		/// </summary>
		public const string FastProperties = "Sudoku.Solving.Manual.FastProperties";

		/// <summary>
		/// Indicates the type <c>ProxyEqualityAttribute</c>.
		/// </summary>
		public const string ProxyEqualityAttribute = "Sudoku.CodeGenerating.ProxyEqualityAttribute";
	}
}
