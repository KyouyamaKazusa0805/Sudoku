using System.Diagnostics;
using Sudoku.Data;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Define a batch rater.
	/// </summary>
	public static class BatchRater
	{
		/// <summary>
		/// To batch rating a series of puzzles being saved in <paramref name="inputPath"/>,
		/// and output the puzzles and their rating results to <paramref name="outputPath"/>.
		/// </summary>
		/// <param name="inputPath">The input path.</param>
		/// <param name="outputPath">The output path.</param>
		/// <returns>The task of this operation.</returns>
		public static async Task BatchRatingAsync(string inputPath, string outputPath)
		{
			// Check the existence of the file.
			if (!File.Exists(inputPath))
			{
				return;
			}

			// Then read all texts grouped them by new line.
			string[] lines = await File.ReadAllLinesAsync(inputPath);

			// Now rating.
#if CONSOLE || DEBUG
			var stopwatch = new Stopwatch();
			stopwatch.Start();
#endif
			for (int i = 0, length = lines.Length; i < length; i++)
			{
				// Check the validity of the puzzle.
				string puzzle = lines[i];
				if (!SudokuGrid.TryParse(puzzle, out var grid))
				{
					continue;
				}

				// Solve it.
				var (
					_, _, total, max, pearl, diamond, _, _, _, stepCount, steps, _, _
				) = new ManualSolver().Solve(grid);

				// Check the number of chains used in the whole technique.
				int chainingTechniquesCount = steps!.Count(
					static step => step.IsAlsTechnique() || step.IsChainingTechnique()
				);

				// Append the text.
				string textToAppend =
					$"{grid.ToString("0")}\t{total.ToString("0.0")} {max.ToString("0.0")} " +
					$"{pearl.ToString("0.0")} {diamond.ToString("0.0")} {stepCount.ToString()} " +
					$"{chainingTechniquesCount.ToString()}\r\n";

				await File.AppendAllTextAsync(outputPath, textToAppend);

				// Then output the information (progress).
#if CONSOLE || DEBUG
				string info =
					$"Current: {(i + 1).ToString()}/{length.ToString()} ({((i + 1) * 100M / length).ToString("0.000")}%), " +
					$"Elapsed: {stopwatch.Elapsed.ToString("hh\\:mm\\:ss\\.fff")}";
#if DEBUG
				Debug.Flush();
				Debug.WriteLine(info);
#elif CONSOLE
				System.Console.Clear();
				System.Console.WriteLine(info);
#endif
#endif
			}

#if CONSOLE || DEBUG
			// Stop the stopwatch.
			stopwatch.Stop();
#endif
		}
	}
}
