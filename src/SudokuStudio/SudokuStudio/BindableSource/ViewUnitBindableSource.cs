namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that can be used for binding as source, for a view unit.
/// </summary>
[DependencyProperty<Conclusion[]>("Conclusions", DocSummary = "Indicates the candidates as conclusions in a single <see cref=\"Step\"/>.")]
[DependencyProperty<View>("View", DocSummary = "Indicates a view of highlight elements.")]
public sealed partial class ViewUnitBindableSource : DependencyObject, IBindableSource;
