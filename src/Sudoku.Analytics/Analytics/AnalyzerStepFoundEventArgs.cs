namespace Sudoku.Analytics;

/// <summary>
/// Provides extra data for event handler <see cref="AnalyzerStepFoundEventHandler"/>.
/// </summary>
/// <param name="step">Indicates the found step.</param>
/// <seealso cref="AnalyzerStepFoundEventHandler"/>
public sealed partial class AnalyzerStepFoundEventArgs([Property] Step step) : EventArgs;
