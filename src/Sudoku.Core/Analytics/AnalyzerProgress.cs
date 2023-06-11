namespace Sudoku.Analytics;

/// <summary>
/// Represents a progress used by <see cref="IAnalyzer{TSelf, TResult}.Analyze(in Grid, IProgress{AnalyzerProgress}?, CancellationToken)"/>.
/// </summary>
/// <seealso cref="IAnalyzer{TSelf, TResult}.Analyze(in Grid, IProgress{AnalyzerProgress}?, CancellationToken)"/>
public readonly record struct AnalyzerProgress(string StepSearcherName, double Percent);
