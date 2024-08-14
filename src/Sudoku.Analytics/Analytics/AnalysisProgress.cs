namespace Sudoku.Analytics;

/// <summary>
/// Represents a progress used by
/// <see cref="IAnalyzer{TSelf, TContext, TResult}.Analyze(ref readonly TContext)"/>.
/// </summary>
/// <param name="StepSearcherName">Indicates the currently used step searcher.</param>
/// <param name="Percent">The percent value.</param>
/// <seealso cref="IAnalyzer{TSelf, TContext, TResult}.Analyze(ref readonly TContext)"/>
public readonly record struct AnalysisProgress(string StepSearcherName, double Percent);
