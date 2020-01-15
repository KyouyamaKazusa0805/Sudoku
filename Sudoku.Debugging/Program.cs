namespace Sudoku.Debugging
{
	internal static class Program
	{
		private static void Main()
		{
			var grid = Sudoku.Data.Meta.Grid.Parse(
				"006000000800074090900800700000010305090000040102080000001009008040620001080000200");
			System.Console.WriteLine(grid.ToString("@"));
		}
	}
}
