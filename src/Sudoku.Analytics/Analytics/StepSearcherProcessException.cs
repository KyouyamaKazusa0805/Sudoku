namespace Sudoku.Analytics;

/// <summary>
/// Represents an exception that will be thrown if an invalid case has been encountered during analyzing a sudoku puzzle.
/// </summary>
/// <typeparam name="TStepSearcher">The type of the step searcher.</typeparam>
public sealed class StepSearcherProcessException<TStepSearcher> : Exception where TStepSearcher: StepSearcher;
