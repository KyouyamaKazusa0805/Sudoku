namespace Sudoku.Analytics;

/// <summary>
/// Provides an event handler that will be called when the whole analysis operation is finished in a certain <see cref="Analyzer"/>.
/// </summary>
/// <param name="sender">The object which triggers the event.</param>
/// <param name="e">The event arguments provided.</param>
public delegate void AnalyzerFinishedEventHandler(Analyzer sender, AnalyzerFinishedEventArgs e);
