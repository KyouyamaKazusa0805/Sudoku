namespace Sudoku.Analytics;

/// <summary>
/// Provides an event handler that will be called when an exception is thrown in a certain <see cref="Analyzer"/>.
/// </summary>
/// <param name="sender">The object which triggers the event.</param>
/// <param name="e">The event arguments provided.</param>
public delegate void AnalyzerExceptionThrownEventHandler(Analyzer sender, AnalyzerExceptionThrownEventArgs e);
