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
			var w = new System.Diagnostics.Stopwatch();

			var z = new Sudoku.Diagnostics.CodeCounter(Sudoku.Diagnostics.Solution.PathRoot, @".+\.cs$");

			w.Start();
			int codeLines = z.CountCodeLines(out int count);
			w.Stop();

			foreach (var fileName in z.FileList)
			{
				System.Console.WriteLine(fileName);
			}

			System.Console.WriteLine(
				$"Code lines: {codeLines}, found files: {count}, time elapsed: {w.Elapsed:hh':'mm'.'ss'.'fff}");
		}
	}
}
