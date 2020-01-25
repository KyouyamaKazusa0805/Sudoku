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
				OptimizedApplyingOrder = true,
				EnableFullHouse = true,
				EnableLastDigit = true
			};
			var grid = Sudoku.Data.Meta.Grid.Parse(
				"090008001300010708002007069706000083900000000003020000000000000000005020080030615");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
