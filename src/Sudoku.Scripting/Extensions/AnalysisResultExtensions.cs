namespace Sudoku.Analytics;

/// <summary>
/// Represents extension methods on <see cref="AnalysisResult"/>.
/// </summary>
/// <seealso cref="AnalysisResult"/>
public static class AnalysisResultExtensions
{
	/// <summary>
	/// Loads external <see cref="Factor"/> source files, and take them into account,
	/// by inserting factors into <see cref="Step.ExternalFactors"/>.
	/// </summary>
	/// <param name="this">The <see cref="AnalysisResult"/> instance to be updated.</param>
	/// <param name="directoryPath">The directory path.</param>
	/// <seealso cref="Factor"/>
	/// <seealso cref="Step.ExternalFactors"/>
	public static void LoadExternalFactors(this AnalysisResult @this, string directoryPath)
	{
		throw new NotImplementedException();
	}
}
