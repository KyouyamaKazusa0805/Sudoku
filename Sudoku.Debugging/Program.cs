namespace Sudoku.Debugging
{
	internal static class Program
	{
		private static void Main()
		{
			var grid = Sudoku.Data.Meta.Grid.Parse(
				"500000482030007000000000309690085000000020000000970035102000000000100050764000008");
			var searcher = new Sudoku.Solving.Checking.BackdoorSearcher(1);
			var result = searcher.FindBackdoors(grid);

			foreach (var list in result)
			{
				foreach (var conclusion in list)
				{
					System.Console.Write($"{conclusion}, ");
				}
				System.Console.WriteLine();
			}

			System.Console.WriteLine("Finished");
		}
	}
}
