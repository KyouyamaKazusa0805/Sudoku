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
				"020900000000002040700500008006070100070103090008020500400005002050200000000006030");
			var analysisResult = solver.Solve(grid);
			System.Console.WriteLine(analysisResult);
		}
	}
}
