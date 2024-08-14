namespace Sudoku.Analytics;

/// <summary>
/// Represents a progress used by <see cref="IAnalyzer{TSelf, TContext, TResult}"/>
/// and <see cref="ICollector{TSelf, TContext, TResult}"/> instances.
/// </summary>
/// <param name="StepSearcherName">Indicates the currently used step searcher.</param>
/// <param name="Percent">The percent value.</param>
/// <seealso cref="IAnalyzer{TSelf, TContext, TResult}.Analyze(ref readonly TContext)"/>
/// <seealso cref="ICollector{TSelf, TContext, TResult}.Collect(ref readonly TContext)"/>
public readonly record struct AnalyzerOrCollectorProgressPresenter(string StepSearcherName, double Percent);
