namespace Sudoku.Analytics;

/// <summary>
/// Provides extra data for event handler <see cref="AnalyzerExceptionThrownEventHandler"/>.
/// </summary>
/// <param name="exception">Indicates the exception thrown.</param>
/// <seealso cref="AnalyzerExceptionThrownEventHandler"/>
public sealed partial class AnalyzerExceptionThrownEventArgs([Property] Exception exception) : EventArgs;
