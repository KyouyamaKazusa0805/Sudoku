namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="SudokuPane"/> instances.
/// </summary>
/// <seealso cref="SudokuPane"/>
[AttachedProperty<Analyzer>("Analyzer")]
[AttachedProperty<StepCollector>("StepCollector")]
public static partial class SudokuPaneBindable
{
	[Default]
	private static readonly Analyzer AnalyzerDefaultValue = PredefinedAnalyzers.Balanced;

	[Default]
	private static readonly StepCollector StepCollectorDefaultValue = new();
}
