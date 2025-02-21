namespace Sudoku.Analytics;

/// <summary>
/// Represents a progress used by <see cref="IStepGatherer{TSelf, TResult}"/> instances.
/// </summary>
/// <param name="StepSearcherName">Indicates the currently used step searcher.</param>
/// <param name="Percent">The percent value.</param>
/// <seealso cref="IStepGatherer{TSelf, TResult}"/>
public readonly record struct StepGathererProgressPresenter(string StepSearcherName, double Percent);
