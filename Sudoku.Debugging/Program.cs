namespace Sudoku.Debugging
{
	/// <summary>
	/// The class handling all program-level actions.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// The main function, the main entry point of the program.
		/// </summary>
		private static void Main()
		{
			var solver = new Sudoku.Solving.Manual.ManualSolver
			{
				IttoRyuWhenPossible = true,
				OptimizedApplyingOrder = false,
				EnableFullHouse = false,
				EnableLastDigit = false
			};
			var grid = Sudoku.Data.Meta.Grid.Parse(
				"206000140000000000410000020005030200000000004000816300000302601004051703150070000");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
			System.Console.WriteLine(Sudoku.Checking.PuzzleAttributeChecker.IsIttoRyu(grid));
		}
	}
}
