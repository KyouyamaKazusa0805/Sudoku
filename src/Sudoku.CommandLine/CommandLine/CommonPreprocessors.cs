namespace Sudoku.CommandLine;

/// <summary>
/// Provides a list of commonly-used preprocessor methods.
/// </summary>
internal static class CommonPreprocessors
{
	/// <summary>
	/// Output invalid puzzle on puzzle after having been checked.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="solver">The solver.</param>
	/// <param name="solution">The solution.</param>
	public static void OutputIfPuzzleNotUnique(in Grid grid, ISolver solver, out Grid solution)
	{
		var result = solver.Solve(grid, out solution);
		if (result is true)
		{
			return;
		}

		var text = result is false ? "The puzzle has multiple solutions." : "The puzzle has no valid solutions.";
		Console.WriteLine($"\e[31m{text}\e[0m");
		solution = Grid.Undefined;
	}

	/// <summary>
	/// Output the object of type <typeparamref name="T"/> to a certain text writer.
	/// </summary>
	/// <typeparam name="T">The type of object.</typeparam>
	/// <param name="obj">The object.</param>
	/// <param name="writer">The stream.</param>
	/// <param name="outputTextCreator">
	/// The method that converts the object <paramref name="obj"/> to <see cref="string"/> representation.
	/// </param>
	/// <param name="appendNewLine">Indicates whether the new line characters will be appended after the output text.</param>
	public static void OutputTextTo<T>(in T obj, TextWriter writer, Func<T, string> outputTextCreator, bool appendNewLine)
		where T : allows ref struct
	{
		writer.Write(outputTextCreator(obj));
		if (appendNewLine)
		{
			writer.WriteLine();
		}
	}

	/// <summary>
	/// Creates a <see cref="CancellationTokenSource"/> instance with a timeout.
	/// Set <see cref="Timeout.Infinite"/> (i.e. -1) to set infinity timeout.
	/// </summary>
	/// <param name="timeout">The timeout in milliseconds.</param>
	/// <returns>A <see cref="CancellationTokenSource"/> instance.</returns>
	/// <seealso cref="Timeout.Infinite"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static CancellationTokenSource CreateCancellationTokenSource(int timeout)
		=> timeout == Timeout.Infinite ? new() : new(timeout);
}
