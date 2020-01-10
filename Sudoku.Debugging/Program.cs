namespace Sudoku.Debugging
{
	internal static class Program
	{
		private static void Main()
		{
			var solver = new Sudoku.Solving.Manual.ManualSolver();
			var grid = Sudoku.Data.Meta.Grid.Parse("073004000950067000004000000028900100069473280007002940000000300000320091000600420");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
