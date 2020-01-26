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
				CheckRegularWingSize = 5,
				OptimizedApplyingOrder = true,
				EnableFullHouse = true,
				EnableLastDigit = true
			};
			var grid = Sudoku.Data.Meta.Grid.Parse(
				".....246.....6.871...7.......4.38..261..5..848..69.5.......5...245.1.....674.....");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
