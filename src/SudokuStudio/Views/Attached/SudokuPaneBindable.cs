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


	/// <summary>
	/// Indicates the backing analyzer.
	/// </summary>
	[DependencyProperty]
	public static partial Analyzer Analyzer { get; set; }

	/// <summary>
	/// Indicates the backing step collector.
	/// </summary>
	[DependencyProperty]
	public static partial Collector StepCollector { get; set; }
}
