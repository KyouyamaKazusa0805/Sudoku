namespace Sudoku.Solving.BruteForces;

/// <summary>
/// Encapsulates a solver result information that is created by a brute force solver.
/// </summary>
/// <param name="OriginalPuzzle"><inheritdoc/></param>
public sealed record BruteForceSolverResult(in Grid OriginalPuzzle) : ISolverResult
{
	/// <inheritdoc/>
	public bool IsSolved { get; init; } = false;

	/// <inheritdoc/>
	public Grid Solution { get; init; } = Grid.Undefined;

	/// <inheritdoc/>
	public TimeSpan ElapsedTime { get; init; } = TimeSpan.Zero;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToDisplayString() => ToDisplayString(CountryCode.Default);

	/// <summary>
	/// Get the string representataion of the current instance using the specified formatting options
	/// and the country code to specify the country information.
	/// </summary>
	/// <param name="countryCode">The country code.</param>
	/// <returns>The string representation of the current instance.</returns>
	public string ToDisplayString(CountryCode countryCode)
	{
		TextResources.Current.ChangeLanguage(countryCode);

		// Print header.
		var sb = new ValueStringBuilder(300);
		sb.Append((string)TextResources.Current.AnalysisResultPuzzle);
		sb.AppendLine($"{OriginalPuzzle:#}");
		sb.Append((string)TextResources.Current.AnalysisResultSolvingTool);
		sb.AppendLine((string)TextResources.Current.Bitwise);

		// Print the solution (if not null).
		if (!Solution.IsUndefined)
		{
			sb.Append((string)TextResources.Current.AnalysisResultPuzzleSolution);
			sb.AppendLine($"{Solution:!}");
		}

		// Print the elapsed time.
		sb.Append((string)TextResources.Current.AnalysisResultPuzzleHas);
		sb.Append(IsSolved ? string.Empty : (string)TextResources.Current.AnalysisResultNot);
		sb.AppendLine((string)TextResources.Current.AnalysisResultBeenSolved);
		sb.Append((string)TextResources.Current.AnalysisResultTimeElapsed);
		sb.AppendLine($"{ElapsedTime:hh\\:mm\\:ss\\.ffffff}");

		return sb.ToString();
	}

	/// <inheritdoc/>
	public override string ToString() => ToDisplayString();
}
