[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE0001")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("", "IDE0002")]

namespace Sudoku.Debugging
{
	/// <summary>
	/// The class aiming to this console application.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// The main function, which is the main entry point
		/// of this console application.
		/// </summary>
		private static void Main()
		{
			var manual = new Sudoku.Solving.Manual.ManualSolver();
			//var hdg = new Sudoku.Solving.Generating.HardPatternPuzzleGenerator();
			//while (true)
			//{
			//	var grid = hdg.Generate();
			//	manual.Solve(grid);
			//	System.Console.WriteLine(grid);
			//}


			var grid = Sudoku.Data.Grid.Parse("009080270000700001020005008003000025000030000170000600400300090300006000082090300");
			var analysis = manual.Solve(grid);
			System.Console.WriteLine(analysis);
		}
	}
}
