namespace Sudoku.Solving.Manual;

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

		var solver = new ManualSolver();

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
			var (_, _, total, max, pearl, diamond, _, _, _, stepCount, steps, _, _) = solver.Solve(grid);

			// Check the number of chains used in the whole technique.
			int chainingTechniquesCount = steps!.Count(static step => step.IsAlmostLockedSets || step.IsChaining);

			// Append the text.
			string textToAppend =
				$"{grid:0}\t{total:0.0} {max:0.0} {pearl:0.0} {diamond:0.0} " +
				$"{stepCount} {chainingTechniquesCount}\r\n";

			await File.AppendAllTextAsync(outputPath, textToAppend);

			// Then output the information (progress).
#if CONSOLE || DEBUG
			string info =
				$"Current: {i + 1}/{length} ({(i + 1) * 100M / length:0.000}%), " +
				$"Elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss\\.fff}";
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
