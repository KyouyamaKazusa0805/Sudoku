namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that can be used for binding as source, for a view unit.
/// </summary>
[DependencyProperty<Conclusion[]>("Conclusions", DocSummary = "Indicates the candidates as conclusions in a single <see cref=\"global::Sudoku.Analytics.Step\"/>.")]
[DependencyProperty<View>("View", DocSummary = "Indicates a view of highlight elements.")]
public sealed partial class ViewUnitBindableSource : DependencyObject
{
	/// <summary>
	/// Initializes a <see cref="ViewUnitBindableSource"/> instance.
	/// </summary>
	public ViewUnitBindableSource() : this([], [])
	{
	}

	/// <summary>
	/// Initializes a <see cref="ViewUnitBindableSource"/> instance via the specified values.
	/// </summary>
	/// <param name="conclusions">The conclusions.</param>
	/// <param name="view">The view.</param>
	public ViewUnitBindableSource(Conclusion[] conclusions, View view) => (Conclusions, View) = (conclusions, view);
}
