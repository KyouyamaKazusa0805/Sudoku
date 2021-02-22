using Sudoku.Resources;

internal static class Program
{
	private static void Main()
	{
		_ = TextResources.Current.Hello;
		_ = TextResources.Current.Sdc;
		TextResources.Current.GreetWith("Sunnie"); // SUDOKU009.
		TextResources.Current.Serialize(); // SUDOKU010.
		TextResources.Current.Serialize(10, 30); // SUDOKU011.
		_ = TextResources.Current.ChangeLanguage(); // SUDOKU012.
	}
}