/////////////////////////////////////////////////////////
// ███████╗██╗   ██╗██████╗  ██████╗ ██╗  ██╗██╗   ██╗ //
// ██╔════╝██║   ██║██╔══██╗██╔═══██╗██║ ██╔╝██║   ██║ //
// ███████╗██║   ██║██║  ██║██║   ██║█████╔╝ ██║   ██║ //
// ╚════██║██║   ██║██║  ██║██║   ██║██╔═██╗ ██║   ██║ //
// ███████║╚██████╔╝██████╔╝╚██████╔╝██║  ██╗╚██████╔╝ //
// ╚══════╝ ╚═════╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝ ╚═════╝  //
/////////////////////////////////////////////////////////
// Here is Sunnie's debugging room!

// Global suppressions.
#pragma warning disable IDE0001
#pragma warning disable IDE0002
#pragma warning disable IDE0065

namespace Sudoku.Debugging
{
	using System.Diagnostics;
	using Sudoku.Diagnostics;
	using static System.Console;

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
			var w = new Stopwatch();

			var z = new CodeCounter(Solution.PathRoot, @".+\.cs$");

			w.Start();
			int codeLines = z.CountCodeLines(out int count);
			w.Stop();

			foreach (var fileName in z.FileList)
			{
				WriteLine(fileName);
			}

			WriteLine($"Code lines: {codeLines}, found files: {count}, time elapsed: {w.Elapsed:hh':'mm'.'ss'.'fff}");
		}
	}
}
