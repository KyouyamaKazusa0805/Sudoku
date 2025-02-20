namespace Sudoku.Analytics;

/// <summary>
/// Provides extra data for event handler <see cref="AnalyzerFinishedEventHandler"/>.
/// </summary>
/// <param name="result">Indicates the result created.</param>
/// <seealso cref="AnalyzerFinishedEventHandler"/>
public sealed partial class AnalyzerFinishedEventArgs([Property] AnalysisResult result) : EventArgs;
