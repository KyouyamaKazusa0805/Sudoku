namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
public static partial class SudokuPaneBindable
{
	[Default]
	private static readonly Analyzer AnalyzerDefaultValue = Analyzer.Balanced;

	[Default]
	private static readonly Collector StepCollectorDefaultValue = new();


	[DependencyProperty]
	public static partial Analyzer Analyzer { get; set; }

	[DependencyProperty]
	public static partial Collector StepCollector { get; set; }
}
