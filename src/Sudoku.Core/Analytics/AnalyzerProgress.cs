namespace Sudoku.Analytics;

/// <summary>
/// Represents a progress used by <see cref="IAnalyzer{TSelf, TResult}.Analyze(in Grid, IProgress{AnalyzerProgress}?, CancellationToken)"/>.
/// </summary>
/// <param name="StepSearcherName">Indicates the currently used step searcher.</param>
/// <param name="Percent">The percent value.</param>
/// <seealso cref="IAnalyzer{TSelf, TResult}.Analyze(in Grid, IProgress{AnalyzerProgress}?, CancellationToken)"/>
public readonly record struct AnalyzerProgress(string StepSearcherName, double Percent);
