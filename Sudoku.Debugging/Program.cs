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
				OptimizedApplyingOrder = true
			};
			var grid = Sudoku.Data.Meta.Grid.Parse(
				"503092000000007985000000000005008160640000092039600700000000000482700000000180407");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
