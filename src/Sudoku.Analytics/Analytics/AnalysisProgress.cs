namespace Sudoku.Analytics;

/// <summary>
/// Represents a progress used by
/// <see cref="IAnalyzer{TSelf, TResult}.Analyze(ref readonly AnalyzerContext)"/>.
/// </summary>
/// <param name="StepSearcherName">Indicates the currently used step searcher.</param>
/// <param name="Percent">The percent value.</param>
/// <seealso cref="IAnalyzer{TSelf, TResult}.Analyze(ref readonly AnalyzerContext)"/>
public readonly record struct AnalysisProgress(string StepSearcherName, double Percent);
