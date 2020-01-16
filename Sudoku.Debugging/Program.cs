namespace Sudoku.Debugging
{
	internal static class Program
	{
		private static void Main()
		{
			var solver = new Sudoku.Solving.Manual.ManualSolver
			{
				OptimizedApplyingOrder = true,
				EnableFullHouse = true,
				EnableLastDigit = true
			};
			var grid = Sudoku.Data.Meta.Grid.Parse(
				"2.......3.8..3..5...34.21....12.54......9......93.86....25.69...9..2..7.4.......1");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
