using Sudoku.Resources;

internal static class Program
{
	private static void Main()
	{
		TextResources.Current.GreetWith("Sunnie"); // SUDOKU009.
		TextResources.Current.Serialize(); // SUDOKU010.
		TextResources.Current.Serialize(10, 30); // SUDOKU011.
		_ = TextResources.Current.ChangeLanguage(); // SUDOKU012.
	}
}