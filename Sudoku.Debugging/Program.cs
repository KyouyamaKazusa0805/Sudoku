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
			var grid = Sudoku.Data.Grid.Parse(".4...93......3..........8...9...5.1.8....47...5..7......23.749.7..4....858.......");
			var manual = new Sudoku.Solving.Manual.ManualSolver();
			var result = manual.Solve(grid);
			System.Console.WriteLine(result);
		}
	}
}
