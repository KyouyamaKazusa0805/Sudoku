namespace Sudoku.Solving.BruteForces;

/// <summary>
/// Encapsulates a solver result information that is created by a brute force solver.
/// </summary>
/// <param name="OriginalPuzzle"><inheritdoc/></param>
/// <param name="IsSolved">
/// <para><inheritdoc/></para>
/// <para>The default value is <see langword="true"/>.</para>
/// </param>
/// <param name="FailedReason">
/// <para><inheritdoc/></para>
/// <para>The default value is <see cref="FailedReason.Nothing"/>.</para>
/// </param>
/// <param name="Solution">
/// <para><inheritdoc/></para>
/// <para>The default value is <see cref="Grid.Undefined"/>.</para>
/// </param>
/// <param name="ElapsedTime">
/// <para><inheritdoc/></para>
/// <para>The default value is <see cref="TimeSpan.Zero"/>.</para>
/// </param>
public sealed record BruteForceSolverResult(
	in Grid OriginalPuzzle,
	bool IsSolved = true,
	FailedReason FailedReason = FailedReason.Nothing,
	in Grid Solution = default,
	TimeSpan ElapsedTime = default
) : ISolverResult
{
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
		var sb = new StringHandler();
		sb.Append((string)TextResources.Current.AnalysisResultPuzzle);
		sb.Append(OriginalPuzzle.ToString("#"));
		sb.AppendLine();

		// Print the solution (if not null).
		if (!Solution.IsUndefined)
		{
			sb.Append((string)TextResources.Current.AnalysisResultPuzzleSolution);
			sb.Append(Solution.ToString("!"));
			sb.AppendLine();
		}

		// Print the elapsed time.
		sb.Append((string)TextResources.Current.AnalysisResultPuzzleHas);
		sb.Append(IsSolved ? string.Empty : (string)TextResources.Current.AnalysisResultNot);
		sb.Append((string)TextResources.Current.AnalysisResultBeenSolved);
		sb.AppendLine();
		sb.Append((string)TextResources.Current.AnalysisResultTimeElapsed);
		sb.Append(ElapsedTime, @"hh\:mm\:ss\.ffffff");
		sb.AppendLine();

		return sb.ToStringAndClear();
	}

	/// <inheritdoc/>
	public override string ToString() => ToDisplayString();
}
