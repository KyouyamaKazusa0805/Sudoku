namespace Sudoku.Debugging
{
	internal static class Program
	{
		private static void Main()
		{
			var grid = Sudoku.Data.Meta.Grid.Parse(
				"500000482030007000000000309690085000000020000000970035102000000000100050764000008");
			var searcher = new Sudoku.Solving.Checking.BackdoorSearcher(grid, 1);
			System.Console.WriteLine(searcher);
		}
	}
}
