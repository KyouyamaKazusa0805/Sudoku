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
				"500000482030007000000000309690085000000020000000970035102000000000100050764000008");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
