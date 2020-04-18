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
			var als1 = new Sudoku.Solving.Manual.Alses.Als(1, new[] { 3 }, new[] { 2, 4 });
			var als2 = new Sudoku.Solving.Manual.Alses.Als(1, new[] { 5 }, new[] { 2, 4 });

			System.Console.WriteLine(als1);
			System.Console.WriteLine(als2);
			System.Console.WriteLine(als1 == als2);
		}
	}
}
