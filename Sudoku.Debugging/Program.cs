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
				"2....358.....2...1...4.56.....9.4.68..5.1.9..61.8.2.....15.8...3...4.....263....4");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
